using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class SceneSetupWizard : EditorWindow
{
    [MenuItem("Tools/Setup MO's IV Quest Scene")]
    public static void SetupScene()
    {
        // Create new scene
        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
            UnityEditor.SceneManagement.NewSceneMode.Single);

        // ============================================
        // 1. CREATE MANAGER GAMEOBJECTS
        // ============================================
        var gameManager = new GameObject("GameManager");
        gameManager.AddComponent<GameManager>();

        var turnManager = new GameObject("TurnManager");
        turnManager.AddComponent<TurnManager>();

        var miniGameController = new GameObject("MiniGameController");
        miniGameController.AddComponent<MiniGameController>();

        var aiFeedback = new GameObject("AIFeedback");
        aiFeedback.AddComponent<AIFeedback>();

        var uiManager = new GameObject("UIManager");
        uiManager.AddComponent<UIManager>();

        // ============================================
        // 2. CREATE MO (PLAYER)
        // ============================================
        var mo = GameObject.CreatePrimitive(PrimitiveType.Quad);
        mo.name = "MO";
        mo.transform.position = Vector3.zero;
        mo.GetComponent<MeshRenderer>().material.color = Color.cyan;

        // Add PlayerController
        var playerController = mo.AddComponent<PlayerController>();
        playerController.moveSpeed = 4f;

        // Add Rigidbody2D with no gravity
        var rb = mo.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0;

        // Add Camera
        var camObj = new GameObject("Main Camera");
        var cam = camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5;
        cam.transform.position = new Vector3(0, 0, -10);
        camObj.AddComponent<AudioListener>();

        // ============================================
        // 3. CREATE UI CANVAS
        // ============================================
        var canvasObj = new GameObject("Canvas");
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;

        var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);

        canvasObj.AddComponent<GraphicRaycaster>();

        // EventSystem
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();

        // ============================================
        // 4. CREATE UI ELEMENTS
        // ============================================

        // --- Score Text ---
        var scoreTextObj = CreateTMPText("ScoreText", canvasObj.transform, "Score: 0");
        var scoreRect = scoreTextObj.GetComponent<RectTransform>();
        scoreRect.anchorMin = new Vector2(0, 1);
        scoreRect.anchorMax = new Vector2(0, 1);
        scoreRect.pivot = new Vector2(0, 1);
        scoreRect.anchoredPosition = new Vector2(20, -20);
        scoreTextObj.GetComponent<TMP_Text>().fontSize = 32;

        // --- Speech Bubble ---
        var speechBubbleObj = CreatePanel("SpeechBubble", canvasObj.transform);
        var speechRect = speechBubbleObj.GetComponent<RectTransform>();
        speechRect.anchorMin = new Vector2(0, 0);
        speechRect.anchorMax = new Vector2(1, 0);
        speechRect.pivot = new Vector2(0.5f, 0);
        speechRect.sizeDelta = new Vector2(0, 150);
        speechRect.anchoredPosition = new Vector2(0, 20);
        speechBubbleObj.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.9f);

        var speechTextObj = CreateTMPText("SpeechText", speechBubbleObj.transform, "Dr. Olayinka: Welcome to your shift!");
        var speechTextRect = speechTextObj.GetComponent<RectTransform>();
        speechTextRect.anchorMin = Vector2.zero;
        speechTextRect.anchorMax = Vector2.one;
        speechTextRect.offsetMin = new Vector2(20, 20);
        speechTextRect.offsetMax = new Vector2(-20, -20);
        speechTextObj.GetComponent<TMP_Text>().fontSize = 24;
        speechTextObj.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.TopLeft;

        // --- 12 Action Buttons ---
        var buttonPanel = CreatePanel("ButtonPanel", canvasObj.transform);
        var buttonPanelRect = buttonPanel.GetComponent<RectTransform>();
        buttonPanelRect.anchorMin = new Vector2(0, 0);
        buttonPanel.anchorMax = new Vector2(1, 0);
        buttonPanelRect.pivot = new Vector2(0.5f, 0);
        buttonPanelRect.sizeDelta = new Vector2(0, 200);
        buttonPanelRect.anchoredPosition = new Vector2(0, 180);
        buttonPanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        var gridLayout = buttonPanel.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(150, 50);
        gridLayout.spacing = new Vector2(10, 10);
        gridLayout.padding = new RectOffset(10, 10, 10, 10);

        string[] buttonNames = {
            "Btn_Introduce", "Btn_HandHygiene", "Btn_GatherEquipment", "Btn_Tourniquet",
            "Btn_VeinSelection", "Btn_Gloves", "Btn_CleanSite", "Btn_InsertCannula",
            "Btn_Flashback", "Btn_AdvanceRelease", "Btn_SecureFlush", "Btn_DisposeSharps"
        };

        ClinicalStep[] steps = {
            ClinicalStep.IntroduceAndConsent, ClinicalStep.HandHygiene, ClinicalStep.GatherEquipment,
            ClinicalStep.ApplyTourniquet, ClinicalStep.VeinSelection, ClinicalStep.DonGloves,
            ClinicalStep.CleanSite, ClinicalStep.InsertCannula, ClinicalStep.ConfirmFlashback,
            ClinicalStep.AdvanceAndRelease, ClinicalStep.SecureAndFlush, ClinicalStep.DisposeSharps
        };

        for (int i = 0; i < 12; i++)
        {
            var btnObj = CreateButton(buttonNames[i], buttonPanel.transform, ClinicalStepInfo.Name(steps[i]));
            var actionBtn = btnObj.AddComponent<ActionButton>();
            actionBtn.step = steps[i];
            actionBtn.turnManager = turnManager.GetComponent<TurnManager>();

            // Get the child text
            var btnText = btnObj.GetComponentInChildren<TMP_Text>();
            if (btnText != null)
            {
                actionBtn.label = btnText;
            }
        }

        // --- Mini-Game Panels (disabled by default) ---

        // VeinPanel
        var veinPanelObj = CreatePanel("VeinPanel", canvasObj.transform);
        veinPanelObj.SetActive(false);
        var veinPanelRect = veinPanelObj.GetComponent<RectTransform>();
        veinPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
        veinPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
        veinPanelRect.sizeDelta = new Vector2(600, 400);
        veinPanelRect.anchoredPosition = Vector2.zero;

        string[] veinNames = { "Vein_Best", "Vein_Mid", "Vein_Poor" };
        int[] veinQualities = { 90, 55, 20 };

        for (int i = 0; i < 3; i++)
        {
            var veinBtn = CreateButton(veinNames[i], veinPanelObj.transform, $"Vein {i + 1}");
            var veinBtnRect = veinBtn.GetComponent<RectTransform>();
            veinBtnRect.anchoredPosition = new Vector2(0, 100 - i * 100);

            var veinOption = veinBtn.AddComponent<VeinOption>();
            veinOption.quality = veinQualities[i];
            veinOption.miniGames = miniGameController.GetComponent<MiniGameController>();
        }

        // InsertionPanel
        var insertionPanelObj = CreatePanel("InsertionPanel", canvasObj.transform);
        insertionPanelObj.SetActive(false);
        var insertionPanelRect = insertionPanelObj.GetComponent<RectTransform>();
        insertionPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
        insertionPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
        insertionPanelRect.sizeDelta = new Vector2(600, 200);
        insertionPanelRect.anchoredPosition = Vector2.zero;

        var trackObj = CreateImage("Track", insertionPanelObj.transform, new Color(0.3f, 0.3f, 0.3f));
        var trackRect = trackObj.GetComponent<RectTransform>();
        trackRect.anchorMin = new Vector2(0.1f, 0.3f);
        trackRect.anchorMax = new Vector2(0.9f, 0.7f);
        trackRect.offsetMin = Vector2.zero;
        trackRect.offsetMax = Vector2.zero;

        var handleObj = CreateImage("Handle", trackObj.transform, Color.yellow);
        var handleRect = handleObj.GetComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0, 0);
        handleRect.anchorMax = new Vector2(0, 1);
        handleRect.sizeDelta = new Vector2(20, 0);
        handleRect.anchoredPosition = Vector2.zero;

        // SharpsPanel
        var sharpsPanelObj = CreatePanel("SharpsPanel", canvasObj.transform);
        sharpsPanelObj.SetActive(false);
        var sharpsPanelRect = sharpsPanelObj.GetComponent<RectTransform>();
        sharpsPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
        sharpsPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
        sharpsPanelRect.sizeDelta = new Vector2(600, 400);
        sharpsPanelRect.anchoredPosition = Vector2.zero;

        var needleObj = CreateImage("Needle", sharpsPanelObj.transform, Color.red);
        var needleRect = needleObj.GetComponent<RectTransform>();
        needleRect.anchorMin = new Vector2(0.2f, 0.5f);
        needleRect.anchorMax = new Vector2(0.2f, 0.5f);
        needleRect.sizeDelta = new Vector2(50, 50);
        needleRect.anchoredPosition = Vector2.zero;

        var sharpsDraggable = needleObj.AddComponent<SharpsDraggable>();

        var binObj = CreateImage("Bin", sharpsPanelObj.transform, new Color(0.5f, 0.3f, 0.1f));
        var binRect = binObj.GetComponent<RectTransform>();
        binRect.anchorMin = new Vector2(0.8f, 0.5f);
        binRect.anchorMax = new Vector2(0.8f, 0.5f);
        binRect.sizeDelta = new Vector2(80, 80);
        binRect.anchoredPosition = Vector2.zero;

        sharpsDraggable.bin = binRect;
        sharpsDraggable.miniGames = miniGameController.GetComponent<MiniGameController>();

        // OutcomePanel
        var outcomePanelObj = CreatePanel("OutcomePanel", canvasObj.transform);
        outcomePanelObj.SetActive(false);
        var outcomePanelRect = outcomePanelObj.GetComponent<RectTransform>();
        outcomePanelRect.anchorMin = new Vector2(0.5f, 0.5f);
        outcomePanelRect.anchorMax = new Vector2(0.5f, 0.5f);
        outcomePanelRect.sizeDelta = new Vector2(600, 400);
        outcomePanelRect.anchoredPosition = Vector2.zero;

        var outcomeTextObj = CreateTMPText("OutcomeText", outcomePanelObj.transform, "Shift Complete!");
        var outcomeTextRect = outcomeTextObj.GetComponent<RectTransform>();
        outcomeTextRect.anchorMin = new Vector2(0.1f, 0.6f);
        outcomeTextRect.anchorMax = new Vector2(0.9f, 0.9f);
        outcomeTextRect.offsetMin = Vector2.zero;
        outcomeTextRect.offsetMax = Vector2.zero;
        outcomeTextObj.GetComponent<TMP_Text>().fontSize = 36;
        outcomeTextObj.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

        var outcomeFaceObj = CreateImage("OutcomeFace", outcomePanelObj.transform, Color.green);
        var outcomeFaceRect = outcomeFaceObj.GetComponent<RectTransform>();
        outcomeFaceRect.anchorMin = new Vector2(0.3f, 0.1f);
        outcomeFaceRect.anchorMax = new Vector2(0.7f, 0.5f);
        outcomeFaceRect.offsetMin = Vector2.zero;
        outcomeFaceRect.offsetMax = Vector2.zero;

        // ============================================
        // 5. WIRE EVERYTHING TOGETHER
        // ============================================

        // TurnManager references
        var turnMgr = turnManager.GetComponent<TurnManager>();
        turnMgr.ui = uiManager.GetComponent<UIManager>();
        turnMgr.miniGames = miniGameController.GetComponent<MiniGameController>();
        turnMgr.aiFeedback = aiFeedback.GetComponent<AIFeedback>();

        // MiniGameController references
        var miniGames = miniGameController.GetComponent<MiniGameController>();
        miniGames.veinPanel = veinPanelObj;
        miniGames.insertionPanel = insertionPanelObj;
        miniGames.sharpsPanel = sharpsPanelObj;
        miniGames.insertionHandle = handleRect;

        // UIManager references
        var uiMgr = uiManager.GetComponent<UIManager>();
        uiMgr.scoreText = scoreTextObj.GetComponent<TMP_Text>();
        uiMgr.speechBubble = speechBubbleObj;
        uiMgr.speechText = speechTextObj.GetComponent<TMP_Text>();
        uiMgr.outcomePanel = outcomePanelObj;
        uiMgr.outcomeText = outcomeTextObj.GetComponent<TMP_Text>();
        uiMgr.outcomeFace = outcomeFaceObj.GetComponent<Image>();

        // ============================================
        // 6. SAVE SCENE
        // ============================================
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");

        Debug.Log("✅ MO's IV Quest scene setup complete! Press Play to test.");
        EditorUtility.DisplayDialog("Setup Complete",
            "Your scene is ready!\n\nClick the Play button at the top to test the game.\n\nClick the 12 action buttons in order to complete the IV cannulation procedure.",
            "Awesome!");
    }

    // Helper methods
    static GameObject CreateTMPText(string name, Transform parent, string text)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 24;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;
        return obj;
    }

    static GameObject CreatePanel(string name, Transform parent)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        var img = obj.AddComponent<Image>();
        img.color = new Color(0.2f, 0.2f, 0.2f, 0.9f);
        return obj;
    }

    static GameObject CreateImage(string name, Transform parent, Color color)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        var img = obj.AddComponent<Image>();
        img.color = color;
        return obj;
    }

    static GameObject CreateButton(string name, Transform parent, string label)
    {
        var btnObj = new GameObject(name, typeof(RectTransform));
        btnObj.transform.SetParent(parent, false);

        var img = btnObj.AddComponent<Image>();
        img.color = new Color(0.3f, 0.5f, 0.8f);

        var btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        var textObj = new GameObject("Text", typeof(RectTransform));
        textObj.transform.SetParent(btnObj.transform, false);

        var textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 18;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        return btnObj;
    }
}
