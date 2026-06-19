using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Builds a JSON summary of MO's attempt and turns it into Dr. Olayinka's
/// feedback via the OpenRouter API (Qwen 3.7).
///
/// The endpoint, model, and key are HARDCODED below — no config file needed.
/// If the API call fails (no network / bad key), it falls back to a
/// rule-based offline response so the game is always playable.
/// </summary>
public class AIFeedback : MonoBehaviour
{
    // ============================================================
    // ==========  HARDCODED API CONFIG (edit here)  ==============
    // ============================================================
    // Paste your OpenRouter API key between the quotes.
    // Get one at https://openrouter.ai/keys
    private const string OPENROUTER_API_KEY = "PASTE_YOUR_OPENROUTER_KEY_HERE";
    private const string OPENROUTER_ENDPOINT = "https://openrouter.ai/api/v1/chat/completions";
    private const string OPENROUTER_MODEL = "qwen/qwen-2.5-72b-instruct";
    // ============================================================

    [Header("Dr. Olayinka persona / prompt")]
    [TextArea(3, 8)]
    public string systemPrompt =
        "You are Dr. Olayinka, a friendly but sharp clinical preceptor. " +
        "Review the trainee's IV cannulation attempt and give 3 short bullets: " +
        "what they did well, what they missed, and one specific tip for next time. " +
        "Keep it warm but honest. 80 words max.";

    [Header("Live API (untick to force offline fallback)")]
    public bool useLiveApi = true;

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

        bool hasKey = !string.IsNullOrEmpty(OPENROUTER_API_KEY)
                      && OPENROUTER_API_KEY != "PASTE_YOUR_OPENROUTER_KEY_HERE";

        if (useLiveApi && hasKey)
        {
            StartCoroutine(SendToOpenRouter(summary, json, onResult));
        }
        else
        {
            Debug.Log("[AIFeedback] No API key set — using offline fallback.");
            onResult?.Invoke(LocalFallback(summary));
        }
    }

    // ============================================================
    // =============  OPENROUTER (Qwen 3.7) INTEGRATION  ==========
    // ============================================================
    // Uses the hardcoded constants at the top of this file.
    private IEnumerator SendToOpenRouter(AttemptSummary summary, string summaryJson, Action<string> onResult)
    {
        string userContent =
            "Here is the trainee's IV cannulation attempt as JSON. " +
            "Give your feedback as Dr. Olayinka.\n\n" + summaryJson;

        // OpenAI-compatible request body (OpenRouter format).
        string body =
            "{" +
            "\"model\":\"" + OPENROUTER_MODEL + "\"," +
            "\"max_tokens\":400," +
            "\"messages\":[" +
            "{\"role\":\"system\",\"content\":" + JsonString(systemPrompt) + "}," +
            "{\"role\":\"user\",\"content\":" + JsonString(userContent) + "}" +
            "]" +
            "}";

        using (var req = new UnityWebRequest(OPENROUTER_ENDPOINT, "POST"))
        {
            byte[] raw = Encoding.UTF8.GetBytes(body);
            req.uploadHandler = new UploadHandlerRaw(raw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");
            req.SetRequestHeader("Authorization", "Bearer " + OPENROUTER_API_KEY);
            req.SetRequestHeader("HTTP-Referer", "https://github.com/Prodigeezyxx/mos-iv-quest");
            req.SetRequestHeader("X-Title", "MO's IV Quest");

            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                Debug.LogWarning("[AIFeedback] API call failed: " + req.error + " — using local fallback.");
                onResult?.Invoke(LocalFallback(summary));
                yield break;
            }

            // OpenRouter/OpenAI response: {"choices":[{"message":{"content":"..."}}], ...}
            string text = ExtractContent(req.downloadHandler.text);
            Debug.Log("[AIFeedback] API response: " + req.downloadHandler.text);
            onResult?.Invoke(string.IsNullOrEmpty(text) ? LocalFallback(summary) : text);
        }
    }
    // ============================================================
    // ===========  END OPENROUTER API INTEGRATION  ===============
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

    // Extracts "content":"..." from OpenRouter/OpenAI chat completions response.
    private static string ExtractContent(string responseJson)
    {
        const string key = "\"content\":\"";
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
