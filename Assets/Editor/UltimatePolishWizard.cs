using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEditor.SceneManagement;

public class UltimatePolishWizard : EditorWindow
{
    [MenuItem("Tools/ULTIMATE Polish: Rebuild UI with Guide + Animations")]
    public static void Polish()
    {
        var scene = EditorSceneManager.GetActiveScene();

        // ============================================
        // 1. UITweener (animation system)
        // ============================================
        var tweenerObj = GameObject.Find("UITweener");
        if (tweenerObj == null)
        {
            tweenerObj = new GameObject("UITweener");
            tweenerObj.AddComponent<UITweener>();
        }

        // ============================================
        // 2. GameFlowGuide
        // ============================================
        var guideObj = GameObject.Find("GameFlowGuide");
        if (guideObj == null)
        {
            guideObj = new GameObject("GameFlowGuide");
        }
        var guide = guideObj.GetComponent<GameFlowGuide>();
        if (guide == null) guide = guideObj.AddComponent<GameFlowGuide>();

        var canvas = GameObject.Find("Canvas");
        if (canvas == null) { Debug.LogError("Canvas not found!"); return; }

        // ============================================
        // 3. PROGRESS BAR (top of screen)
        // ============================================
        var progressBg = CreatePanel("ProgressBg", canvas.transform);
        var progressBgRect = progressBg.GetComponent<RectTransform>();
        progressBgRect.anchorMin = new Vector2(0.2f, 0.94f);
        progressBgRect.anchorMax = new Vector2(0.8f, 0.98f);
        progressBgRect.offsetMin = Vector2.zero;
        progressBgRect.offsetMax = Vector2.zero;
        progressBg.GetComponent<Image>().color = new Color(0.15f, 0.15f, 0.2f, 0.9f);

        var progressFillObj = CreateImage("ProgressFill", progressBg.transform, new Color(0.2f, 0.8f, 0.4f));
        var progressFillRect = progressFillObj.GetComponent<RectTransform>();
        progressFillRect.anchorMin = Vector2.zero;
        progressFillRect.anchorMax = Vector2.one;
        progressFillRect.offsetMin = Vector2.zero;
        progressFillRect.offsetMax = Vector2.zero;
        var progressFillImg = progressFillObj.GetComponent<Image>();
        progressFillImg.type = Image.Type.Filled;
        progressFillImg.fillMethod = Image.FillMethod.Horizontal;
        progressFillImg.fillAmount = 0f;

        var progressTextObj = CreateTMP("ProgressText", progressBg.transform, "Step 1 of 12", 16, TextAlignmentOptions.Center);
        var progressTextRect = progressTextObj.GetComponent<RectTransform>();
        progressTextRect.anchorMin = Vector2.zero;
        progressTextRect.anchorMax = Vector2.one;
        progressTextRect.offsetMin = Vector2.zero;
        progressTextRect.offsetMax = Vector2.zero;
        progressTextObj.GetComponent<TextMeshProUGUI>().color = Color.white;

        guide.progressFill = progressFillImg;
        guide.progressText = progressTextObj.GetComponent<TextMeshProUGUI>();

        // ============================================
        // 4. INSTRUCTION BANNER (below progress bar)
        // ============================================
        var bannerObj = CreatePanel("InstructionBanner", canvas.transform);
        var bannerRect = bannerObj.GetComponent<RectTransform>();
        bannerRect.anchorMin = new Vector2(0.1f, 0.86f);
        bannerRect.anchorMax = new Vector2(0.9f, 0.93f);
        bannerRect.offsetMin = Vector2.zero;
        bannerRect.offsetMax = Vector2.zero;
        bannerObj.GetComponent<Image>().color = new Color(0.1f, 0.3f, 0.15f, 0.92f);

        var bannerTextObj = CreateTMP("InstructionText", bannerObj.transform, "Click the GREEN button to start!", 22, TextAlignmentOptions.Center);
        var bannerTextRect = bannerTextObj.GetComponent<RectTransform>();
        bannerTextRect.anchorMin = Vector2.zero;
        bannerTextRect.anchorMax = Vector2.one;
        bannerTextRect.offsetMin = new Vector2(20, 5);
        bannerTextRect.offsetMax = new Vector2(-20, -5);
        bannerTextObj.GetComponent<TextMeshProUGUI>().color = Color.white;

        guide.instructionBanner = bannerObj;
        guide.instructionText = bannerTextObj.GetComponent<TextMeshProUGUI>();

        // ============================================
        // 5. STEP LIST (right side panel)
        // ============================================
        var stepListPanel = CreatePanel("StepListPanel", canvas.transform);
        var stepListRect = stepListPanel.GetComponent<RectTransform>();
        stepListRect.anchorMin = new Vector2(0.78f, 0.1f);
        stepListRect.anchorMax = new Vector2(0.98f, 0.84f);
        stepListRect.offsetMin = Vector2.zero;
        stepListRect.offsetMax = Vector2.zero;
        stepListPanel.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.88f);

        var stepListTitle = CreateTMP("StepListTitle", stepListPanel.transform, "CLINICAL STEPS", 18, TextAlignmentOptions.Center);
        var stepListTitleRect = stepListTitle.GetComponent<RectTransform>();
        stepListTitleRect.anchorMin = new Vector2(0, 0.92f);
        stepListTitleRect.anchorMax = new Vector2(1, 1);
        stepListTitleRect.offsetMin = Vector2.zero;
        stepListTitleRect.offsetMax = Vector2.zero;
        stepListTitle.GetComponent<TextMeshProUGUI>().color = new Color(1f, 0.85f, 0.4f);
        stepListTitle.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Step list items container with scroll-like layout
        var listContainer = new GameObject("StepListContainer", typeof(RectTransform), typeof(VerticalLayoutGroup));
        listContainer.transform.SetParent(stepListPanel.transform, false);
        var listContainerRect = listContainer.GetComponent<RectTransform>();
        listContainerRect.anchorMin = new Vector2(0, 0);
        listContainerRect.anchorMax = new Vector2(1, 0.9f);
        listContainerRect.offsetMin = new Vector2(5, 5);
        listContainerRect.offsetMax = new Vector2(-5, -5);
        var vlg = listContainer.GetComponent<VerticalLayoutGroup>();
        vlg.spacing = 2;
        vlg.childAlignment = TextAnchor.UpperLeft;
        vlg.childControlWidth = true;
        vlg.childControlHeight = false;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        guide.stepListContainer = listContainer.transform;

        // ============================================
        // 6. REBUILD ACTION BUTTONS with medical theme
        // ============================================
        // Find existing ButtonPanel or create one
        var buttonPanel = GameObject.Find("ButtonPanel");
        if (buttonPanel == null)
        {
            buttonPanel = CreatePanel("ButtonPanel", canvas.transform);
            var bpRect = buttonPanel.GetComponent<RectTransform>();
            bpRect.anchorMin = new Vector2(0.02f, 0.02f);
            bpRect.anchorMax = new Vector2(0.76f, 0.4f);
            bpRect.offsetMin = Vector2.zero;
            bpRect.offsetMax = Vector2.zero;
            buttonPanel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.14f, 0.85f);
        }

        // Clear existing children
        for (int i = buttonPanel.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(buttonPanel.transform.GetChild(i).gameObject);

        // Add grid layout
        var grid = buttonPanel.GetComponent<GridLayoutGroup>();
        if (grid == null) grid = buttonPanel.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(170, 70);
        grid.spacing = new Vector2(8, 8);
        grid.padding = new RectOffset(10, 10, 10, 10);
        grid.childAlignment = TextAnchor.UpperLeft;

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

        System.Action<int, Sprite> assignIcon = (idx, icon) => { };
        var turnManager = FindAnyObjectByType<TurnManager>();

        for (int i = 0; i < 12; i++)
        {
            var btnObj = CreateActionButton(buttonNames[i], buttonPanel.transform,
                ClinicalStepInfo.Name(steps[i]), steps[i], turnManager);

            // Register with flow guide
            var btn = btnObj.GetComponent<Button>();
            guide.RegisterButton(steps[i], btn);

            // Assign medical icon
            Sprite icon = GetIconForStep(steps[i]);
            if (icon != null)
            {
                var iconObj = new GameObject("Icon", typeof(RectTransform));
                iconObj.transform.SetParent(btnObj.transform, false);
                var iconRect = iconObj.GetComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0, 0.5f);
                iconRect.anchorMax = new Vector2(0, 0.5f);
                iconRect.pivot = new Vector2(0, 0.5f);
                iconRect.sizeDelta = new Vector2(36, 36);
                iconRect.anchoredPosition = new Vector2(8, 0);
                var iconImg = iconObj.AddComponent<Image>();
                iconImg.sprite = icon;
                iconImg.preserveAspect = true;
            }
        }

        // ============================================
        // 7. WIRE TURNMANAGER → FLOW GUIDE
        // ============================================
        if (turnManager != null)
        {
            turnManager.flowGuide = guide;
        }

        // ============================================
        // 8. BUILD STEP LIST
        // ============================================
        guide.BuildStepList();

        // ============================================
        // 9. POLISH SPEECH BUBBLE with enhanced portrait
        // ============================================
        var speechBubble = GameObject.Find("SpeechBubble");
        if (speechBubble != null)
        {
            // Update portrait
            var portraitObj = speechBubble.transform.Find("MentorPortrait");
            if (portraitObj != null)
            {
                var portraitImg = portraitObj.GetComponent<Image>();
                if (portraitImg != null)
                {
                    portraitImg.sprite = EnhancedSprites.DrawMentorPortrait();
                    portraitImg.color = Color.white;
                }
            }
        }

        // ============================================
        // 10. UPDATE MO SPRITE
        // ============================================
        var mo = GameObject.Find("MO");
        if (mo != null)
        {
            var sr = mo.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = EnhancedSprites.DrawNurse();
                sr.color = Color.white;
            }
        }

        // ============================================
        // 11. UPDATE PATIENT + BED + IV SPRITES
        // ============================================
        var bedObj = GameObject.Find("PatientBed");
        if (bedObj != null)
        {
            // Find existing sprites and update them
            var bedSprite = bedObj.transform.Find("Bed");
            if (bedSprite != null)
            {
                var sr = bedSprite.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = EnhancedSprites.DrawHospitalBed();
            }
            var patientSprite = bedObj.transform.Find("Patient");
            if (patientSprite != null)
            {
                var sr = patientSprite.GetComponent<SpriteRenderer>();
                if (sr != null) sr.sprite = EnhancedSprites.DrawPatientLying();
            }
        }

        var ivStand = GameObject.Find("IVStand");
        if (ivStand != null)
        {
            var sr = ivStand.GetComponent<SpriteRenderer>();
            if (sr != null) sr.sprite = EnhancedSprites.DrawIVStand();
        }

        // ============================================
        // 12. SAVE
        // ============================================
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);

        Debug.Log("✅ ULTIMATE POLISH complete!");
        EditorUtility.DisplayDialog("ULTIMATE POLISH Complete!",
            "Your game now has:\n\n" +
            "✅ Progress bar (Step X of 12)\n" +
            "✅ Instruction banner with specific guidance per step\n" +
            "✅ Step list on the right (✓ done, ▶ current, future greyed)\n" +
            "✅ Green highlight on the correct next button\n" +
            "✅ Red flash + shake on wrong button clicks\n" +
            "✅ Pop-in animations on panels\n" +
            "✅ Enhanced medical sprites (nurse, patient, bed, IV stand)\n" +
            "✅ Medical icons on action buttons\n\n" +
            "Press Play to see the difference!",
            "Awesome!");
    }

    static Sprite GetIconForStep(ClinicalStep step)
    {
        switch (step)
        {
            case ClinicalStep.IntroduceAndConsent: return EnhancedSprites.DrawClipboardIcon();
            case ClinicalStep.HandHygiene: return EnhancedSprites.DrawHandwashIcon();
            case ClinicalStep.VeinSelection: return EnhancedSprites.DrawSwabIcon();
            case ClinicalStep.InsertCannula: return EnhancedSprites.DrawSyringeIcon();
            case ClinicalStep.DonGloves: return EnhancedSprites.DrawGlovesIcon();
            case ClinicalStep.ApplyTourniquet: return EnhancedSprites.DrawTourniquetIcon();
            case ClinicalStep.DisposeSharps: return EnhancedSprites.DrawSharpsBinIcon();
            default: return null;
        }
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

    static GameObject CreateImage(string name, Transform parent, Color color)
    {
        var obj = new GameObject(name, typeof(RectTransform));
        obj.transform.SetParent(parent, false);
        var img = obj.AddComponent<Image>();
        img.color = color;
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

    static GameObject CreateActionButton(string name, Transform parent, string label, ClinicalStep step, TurnManager turnManager)
    {
        var btnObj = new GameObject(name, typeof(RectTransform));
        btnObj.transform.SetParent(parent, false);

        var img = btnObj.AddComponent<Image>();
        img.color = new Color(0.25f, 0.3f, 0.4f); // default blue-grey

        var btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        var actionBtn = btnObj.AddComponent<ActionButton>();
        actionBtn.step = step;
        if (turnManager != null) actionBtn.turnManager = turnManager;

        // Label text (offset to make room for icon)
        var textObj = new GameObject("Text", typeof(RectTransform));
        textObj.transform.SetParent(btnObj.transform, false);
        var tr = textObj.GetComponent<RectTransform>();
        tr.anchorMin = Vector2.zero;
        tr.anchorMax = Vector2.one;
        tr.offsetMin = new Vector2(45, 2);
        tr.offsetMax = new Vector2(-5, -2);
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 16;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Left;
        tmp.enableWordWrapping = true;

        actionBtn.label = tmp;

        return btnObj;
    }
}
