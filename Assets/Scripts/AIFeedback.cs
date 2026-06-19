using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Builds a JSON summary of MO's attempt and turns it into Dr. Olayinka's
/// feedback.
///
/// RIGHT NOW: works fully offline with a rule-based fallback so your demo
/// never depends on the network.
///
/// LATER: drop in your Claude API key (via Genspark) — see the clearly
/// marked "WIRE YOUR CLAUDE API HERE" region below. Put the key in
///   Assets/Resources/ai_config.json   (already in .gitignore)
/// shaped like: { "apiKey": "sk-...", "endpoint": "https://...", "model": "claude-..." }
/// </summary>
public class AIFeedback : MonoBehaviour
{
    [Header("Dr. Olayinka persona / prompt")]
    [TextArea(3, 8)]
    public string systemPrompt =
        "You are Dr. Olayinka, a friendly but sharp clinical preceptor. " +
        "Review the trainee's IV cannulation attempt and give 3 short bullets: " +
        "what they did well, what they missed, and one specific tip for next time. " +
        "Keep it warm but honest. 80 words max.";

    [Header("Toggle: try the real API if a config is present")]
    public bool useLiveApi = false;

    // ---- JSON shapes (Unity's JsonUtility needs concrete [Serializable] types) ----
    [Serializable]
    public class AttemptSummary
    {
        public List<string> completed_steps = new List<string>();
        public List<string> skipped_steps = new List<string>();
        public List<string> errors = new List<string>();
        public int vein_score;
        public int insertion_score;
        public int sharps_score;
        public int total_score;
    }

    [Serializable]
    private class AiConfig
    {
        public string apiKey;
        public string endpoint;
        public string model;
    }

    /// <summary>Build the structured summary from the current session.</summary>
    public AttemptSummary BuildSummary()
    {
        var gm = GameManager.Instance;
        var s = new AttemptSummary();
        if (gm == null) return s;

        foreach (var step in gm.CompletedSteps) s.completed_steps.Add(step.ToString());
        foreach (var step in gm.GetSkippedSteps()) s.skipped_steps.Add(step.ToString());
        s.errors = new List<string>(gm.Errors);
        s.vein_score = gm.VeinScore;
        s.insertion_score = gm.InsertionScore;
        s.sharps_score = gm.SharpsScore;
        s.total_score = gm.Score;
        return s;
    }

    /// <summary>
    /// Main entry point. TurnManager calls this at end of shift.
    /// Calls back with the feedback string (live API or local fallback).
    /// </summary>
    public void RequestFeedback(Action<string> onResult)
    {
        var summary = BuildSummary();
        string json = JsonUtility.ToJson(summary, true);
        Debug.Log("[AIFeedback] Attempt summary:\n" + json);

        var config = LoadConfig();
        if (useLiveApi && config != null && !string.IsNullOrEmpty(config.apiKey))
        {
            StartCoroutine(SendToClaude(config, summary, json, onResult));
        }
        else
        {
            onResult?.Invoke(LocalFallback(summary));
        }
    }

    private AiConfig LoadConfig()
    {
        // Looks for Assets/Resources/ai_config.json (without the .json extension).
        var asset = Resources.Load<TextAsset>("ai_config");
        if (asset == null) return null;
        try { return JsonUtility.FromJson<AiConfig>(asset.text); }
        catch { return null; }
    }

    // ============================================================
    // ================  WIRE YOUR CLAUDE API HERE  ===============
    // ============================================================
    // This is set up for Anthropic's Messages API shape. If Genspark
    // gives you an OpenAI-compatible endpoint instead, adjust the
    // headers/body to match (it's only a few lines).
    private IEnumerator SendToClaude(AiConfig cfg, AttemptSummary summary, string summaryJson, Action<string> onResult)
    {
        string userContent =
            "Here is the trainee's IV cannulation attempt as JSON. " +
            "Give your feedback as Dr. Olayinka.\n\n" + summaryJson;

        // Build request body (Anthropic Messages API format).
        string body =
            "{" +
            "\"model\":\"" + (string.IsNullOrEmpty(cfg.model) ? "claude-3-5-sonnet-latest" : cfg.model) + "\"," +
            "\"max_tokens\":300," +
            "\"system\":" + JsonString(systemPrompt) + "," +
            "\"messages\":[{\"role\":\"user\",\"content\":" + JsonString(userContent) + "}]" +
            "}";

        string url = string.IsNullOrEmpty(cfg.endpoint) ? "https://api.anthropic.com/v1/messages" : cfg.endpoint;

        using (var req = new UnityWebRequest(url, "POST"))
        {
            byte[] raw = Encoding.UTF8.GetBytes(body);
            req.uploadHandler = new UploadHandlerRaw(raw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("x-api-key", cfg.apiKey);
            req.SetRequestHeader("anthropic-version", "2023-06-01");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("[AIFeedback] API call failed: " + req.error + " — using local fallback.");
                onResult?.Invoke(LocalFallback(summary));
                yield break;
            }

            // Response: {"content":[{"type":"text","text":"..."}], ...}
            string text = ExtractFirstText(req.downloadHandler.text);
            onResult?.Invoke(string.IsNullOrEmpty(text) ? LocalFallback(summary) : text);
        }
    }
    // ============================================================
    // ==============  END CLAUDE API INTEGRATION  ================
    // ============================================================

    /// <summary>Offline, rule-based feedback so the game is always playable.</summary>
    private string LocalFallback(AttemptSummary s)
    {
        var sb = new StringBuilder();
        sb.Append("Here's my read on that one, MO.\n");

        if (s.skipped_steps.Count == 0 && s.errors.Count == 0)
            sb.Append("• Strong work — clean sequence, nothing skipped.\n");
        else
            sb.Append($"• Solid effort: you completed {s.completed_steps.Count}/{ClinicalStepInfo.TotalSteps} steps.\n");

        if (s.errors.Count > 0)
            sb.Append($"• Watch the order — {s.errors.Count} step(s) came at the wrong time.\n");
        if (s.skipped_steps.Count > 0)
            sb.Append($"• You missed: {string.Join(", ", s.skipped_steps)}.\n");

        int lowest = LowestMiniGame(s, out string which);
        if (lowest >= 0 && lowest < 80)
            sb.Append($"• Tip: practise your {which} — that's where most of the points slipped.");
        else
            sb.Append("• Tip: keep that pace and double-check hand hygiene timing.");

        return sb.ToString();
    }

    private int LowestMiniGame(AttemptSummary s, out string which)
    {
        which = "technique";
        int lowest = int.MaxValue;

        if (s.vein_score >= 0 && s.vein_score < lowest)
        {
            lowest = s.vein_score; which = "vein selection";
        }
        if (s.insertion_score >= 0 && s.insertion_score < lowest)
        {
            lowest = s.insertion_score; which = "insertion timing";
        }
        if (s.sharps_score >= 0 && s.sharps_score < lowest)
        {
            lowest = s.sharps_score; which = "sharps disposal";
        }

        return lowest == int.MaxValue ? -1 : lowest;
    }

    // Minimal JSON string escaper for the request body.
    private static string JsonString(string value)
    {
        var sb = new StringBuilder("\"");
        foreach (char c in value)
        {
            switch (c)
            {
                case '"': sb.Append("\\\""); break;
                case '\\': sb.Append("\\\\"); break;
                case '\n': sb.Append("\\n"); break;
                case '\r': sb.Append("\\r"); break;
                case '\t': sb.Append("\\t"); break;
                default: sb.Append(c); break;
            }
        }
        sb.Append('"');
        return sb.ToString();
    }

    // Very small extractor for the first "text":"..." in the Claude response.
    private static string ExtractFirstText(string responseJson)
    {
        const string key = "\"text\":\"";
        int i = responseJson.IndexOf(key, StringComparison.Ordinal);
        if (i < 0) return null;
        i += key.Length;
        var sb = new StringBuilder();
        for (; i < responseJson.Length; i++)
        {
            char c = responseJson[i];
            if (c == '\\' && i + 1 < responseJson.Length)
            {
                char n = responseJson[++i];
                sb.Append(n == 'n' ? '\n' : n);
            }
            else if (c == '"') break;
            else sb.Append(c);
        }
        return sb.ToString();
    }
}
