using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class PolishWizard : EditorWindow
{
    [MenuItem("Tools/Polish: Title Screen + Audio + Restart + Build")]
    public static void Polish()
    {
        var scene = EditorSceneManager.GetActiveScene();

        // ============================================
        // 1. AUDIO MANAGER
        // ============================================
        var audioObj = GameObject.Find("AudioManager");
        if (audioObj == null)
        {
            audioObj = new GameObject("AudioManager");
            audioObj.AddComponent<AudioManager>();
            Debug.Log("✅ AudioManager created");
        }

        // ============================================
        // 2. TITLE SCREEN PANEL
        // ============================================
        var canvas = GameObject.Find("Canvas");
        if (canvas == null) { Debug.LogError("Canvas not found!"); return; }

        var titlePanel = CreatePanel("TitlePanel", canvas.transform);
        var titleRect = titlePanel.GetComponent<RectTransform>();
        titleRect.anchorMin = Vector2.zero;
        titleRect.anchorMax = Vector2.one;
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        titlePanel.GetComponent<Image>().color = new Color(0.05f, 0.08f, 0.15f, 0.98f);

        // Title text
        var titleTextObj = CreateTMP("TitleText", titlePanel.transform, "MO's IV Quest", 72, TextAlignmentOptions.Center);
        var titleTextRect = titleTextObj.GetComponent<RectTransform>();
        titleTextRect.anchorMin = new Vector2(0.1f, 0.65f);
        titleTextRect.anchorMax = new Vector2(0.9f, 0.85f);
        titleTextRect.offsetMin = Vector2.zero;
        titleTextRect.offsetMax = Vector2.zero;
        titleTextObj.GetComponent<TextMeshProUGUI>().color = new Color(0.4f, 0.9f, 1f);

        // Subtitle
        var subObj = CreateTMP("Subtitle", titlePanel.transform, "A 48-Hour Night Shift at St. Pixel General", 28, TextAlignmentOptions.Center);
        var subRect = subObj.GetComponent<RectTransform>();
        subRect.anchorMin = new Vector2(0.1f, 0.55f);
        subRect.anchorMax = new Vector2(0.9f, 0.62f);
        subRect.offsetMin = Vector2.zero;
        subRect.offsetMax = Vector2.zero;
        subObj.GetComponent<TextMeshProUGUI>().color = Color.white;

        // Learning outcomes
        string outcomes = "After playing, you will be able to:\n" +
                          "• Sequence the 12 steps of IV cannulation\n" +
                          "• Identify suitable veins by visibility & palpability\n" +
                          "• Recognise common procedural errors\n" +
                          "• Recall the role of flashback & saline flush\n" +
                          "• Reflect on performance via AI feedback";
        var outcomesObj = CreateTMP("Outcomes", titlePanel.transform, outcomes, 22, TextAlignmentOptions.Left);
        var outcomesRect = outcomesObj.GetComponent<RectTransform>();
        outcomesRect.anchorMin = new Vector2(0.15f, 0.28f);
        outcomesRect.anchorMax = new Vector2(0.85f, 0.5f);
        outcomesRect.offsetMin = Vector2.zero;
        outcomesRect.offsetMax = Vector2.zero;
        outcomesObj.GetComponent<TextMeshProUGUI>().color = new Color(0.8f, 0.85f, 0.9f);

        // Start button
        var startBtn = CreateButton("StartButton", titlePanel.transform, "START SHIFT", 32);
        var startBtnRect = startBtn.GetComponent<RectTransform>();
        startBtnRect.anchorMin = new Vector2(0.35f, 0.1f);
        startBtnRect.anchorMax = new Vector2(0.65f, 0.22f);
        startBtnRect.offsetMin = Vector2.zero;
        startBtnRect.offsetMax = Vector2.zero;
        startBtn.GetComponent<Image>().color = new Color(0.2f, 0.7f, 0.4f);

        // ============================================
        // 3. TITLE SCREEN MANAGER
        // ============================================
        var tsmObj = GameObject.Find("TitleScreenManager");
        if (tsmObj == null)
        {
            tsmObj = new GameObject("TitleScreenManager");
        }
        var tsm = tsmObj.GetComponent<TitleScreenManager>();
        if (tsm == null) tsm = tsmObj.AddComponent<TitleScreenManager>();
        tsm.titlePanel = titlePanel;
        tsm.titleText = titleTextObj.GetComponent<TextMeshProUGUI>();
        tsm.subtitleText = subObj.GetComponent<TextMeshProUGUI>();
        tsm.startButton = startBtn.GetComponent<Button>();
        tsm.learningOutcomes = outcomes;

        // Wire the Start button to call StartGame
        var startBtnComp = startBtn.GetComponent<Button>();
        startBtnComp.onClick.RemoveAllListeners();
        startBtnComp.onClick.AddListener(tsm.StartGame);

        // ============================================
        // 4. WIRE TITLE SCREEN INTO TURNMANAGER
        // ============================================
        var tm = FindFirstObjectByType<TurnManager>();
        if (tm != null)
        {
            tm.titleScreen = titlePanel;
        }

        // ============================================
        // 5. RESTART BUTTON ON OUTCOME PANEL
        // ============================================
        var outcomePanel = GameObject.Find("OutcomePanel");
        if (outcomePanel != null)
        {
            var restartBtn = CreateButton("RestartButton", outcomePanel.transform, "RESTART SHIFT", 28);
            var restartRect = restartBtn.GetComponent<RectTransform>();
            restartRect.anchorMin = new Vector2(0.3f, 0.02f);
            restartRect.anchorMax = new Vector2(0.7f, 0.12f);
            restartRect.offsetMin = Vector2.zero;
            restartRect.offsetMax = Vector2.zero;
            restartBtn.GetComponent<Image>().color = new Color(0.2f, 0.5f, 0.8f);

            var restartBtnComp = restartBtn.GetComponent<Button>();
            restartBtnComp.onClick.AddListener(() =>
            {
                if (tsm != null) tsm.RestartGame();
            });
        }

        // ============================================
        // 6. BUILD SETTINGS — add scene
        // ============================================
        var buildScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        bool found = false;
        foreach (var bs in buildScenes)
        {
            if (bs.path == scene.path) { bs.enabled = true; found = true; break; }
        }
        if (!found && !string.IsNullOrEmpty(scene.path))
        {
            buildScenes.Add(new EditorBuildSettingsScene(scene.path, true));
            EditorBuildSettings.scenes = buildScenes.ToArray();
            Debug.Log("✅ Scene added to Build Settings: " + scene.path);
        }

        // ============================================
        // 7. SAVE
        // ============================================
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("✅ Polish complete: title screen, audio, restart button, build settings!");
        EditorUtility.DisplayDialog("Polish Complete",
            "Added:\n" +
            "• Title screen with learning outcomes + Start Shift button\n" +
            "• AudioManager (procedural SFX, no files needed)\n" +
            "• Restart button on outcome panel\n" +
            "• Scene added to Build Settings\n\n" +
            "Press Play to test the full flow!",
            "Let's go!");
    }

    // --- Helpers ---
    static GameObject CreatePanel(string name, Transform parent)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        var img = obj.AddComponent<Image>();
        img.color = new Color(0.1f, 0.1f, 0.1f, 0.95f);
        return obj;
    }

    static GameObject CreateTMP(string name, Transform parent, string text, int size, TextAlignmentOptions align)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = size;
        tmp.color = Color.white;
        tmp.alignment = align;
        return obj;
    }

    static GameObject CreateButton(string name, Transform parent, string label, int fontSize)
    {
        var btnObj = new GameObject(name, typeof(RectTransform));
        btnObj.transform.SetParent(parent, false);
        var img = btnObj.AddComponent<Image>();
        img.color = new Color(0.3f, 0.5f, 0.8f);
        var btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        var textObj = new GameObject("Text", typeof(RectTransform));
        textObj.transform.SetParent(btnObj.transform, false);
        var tr = textObj.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero; tr.anchorMax = Vector2.one;
        tr.offsetMin = Vector2.zero; tr.offsetMax = Vector2.zero;
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = fontSize;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
        return btnObj;
    }
}
