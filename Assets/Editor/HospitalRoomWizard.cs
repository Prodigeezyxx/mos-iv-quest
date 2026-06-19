using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class HospitalRoomWizard : EditorWindow
{
    [MenuItem("Tools/Add Hospital Room & Patient")]
    public static void SetupRoom()
    {
        // ============================================
        // 1. HOSPITAL ROOM BACKGROUND (colored shapes as walls/floor)
        // ============================================
        var roomObj = new GameObject("HospitalRoom");
        roomObj.transform.position = Vector3.zero;

        // Floor
        CreateWorldSprite("Floor", roomObj.transform,
            new Color(0.82f, 0.84f, 0.86f), new Vector3(0, -3f, 0), new Vector2(20, 6));
        // Back wall
        CreateWorldSprite("BackWall", roomObj.transform,
            new Color(0.68f, 0.76f, 0.80f), new Vector3(0, 2.5f, 0), new Vector2(20, 7));
        // Wall trim
        CreateWorldSprite("WallTrim", roomObj.transform,
            new Color(0.5f, 0.6f, 0.65f), new Vector3(0, -0.8f, 0), new Vector2(20, 0.25f));

        // ============================================
        // 2. PATIENT BED (procedural sprite)
        // ============================================
        var bedObj = new GameObject("PatientBed");
        bedObj.transform.SetParent(roomObj.transform, false);
        bedObj.transform.position = new Vector3(2f, -1.5f, 0);

        var bedSprite = MedicalSpriteFactory.DrawHospitalBed();
        var bed = CreateWorldSpriteFromSprite("Bed", bedObj.transform, bedSprite,
            new Vector3(0, 0, 0), 6f);
        bed.GetComponent<SpriteRenderer>().sortingOrder = 1;

        // ============================================
        // 3. PATIENT (lying on bed)
        // ============================================
        var patientSprite = MedicalSpriteFactory.DrawPatientLying();
        var patient = CreateWorldSpriteFromSprite("Patient", bedObj.transform, patientSprite,
            new Vector3(0.2f, 0.6f, -0.1f), 4f);
        patient.GetComponent<SpriteRenderer>().sortingOrder = 2;

        // ============================================
        // 4. IV STAND (procedural sprite)
        // ============================================
        var ivSprite = MedicalSpriteFactory.DrawIVStand();
        var ivStand = CreateWorldSpriteFromSprite("IVStand", roomObj.transform, ivSprite,
            new Vector3(-1.8f, -1.2f, 0), 3.5f);
        ivStand.GetComponent<SpriteRenderer>().sortingOrder = 3;

        // ============================================
        // 5. MO — use procedural nurse sprite
        // ============================================
        var mo = GameObject.Find("MO");
        if (mo != null)
        {
            mo.transform.position = new Vector3(-3.5f, -1.5f, 0);
            var sr = mo.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = MedicalSpriteFactory.DrawNurse();
                sr.color = Color.white;
                sr.sortingOrder = 5;
            }
            mo.transform.localScale = new Vector3(3f, 3f, 1f);
        }

        // ============================================
        // 6. ADJUST CAMERA
        // ============================================
        var cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 4.5f;
            cam.transform.position = new Vector3(0, -0.5f, -10);
            cam.backgroundColor = new Color(0.45f, 0.52f, 0.58f);
        }

        // ============================================
        // 7. DR. OLAYINKA PORTRAIT (procedural)
        // ============================================
        var speechBubble = GameObject.Find("SpeechBubble");
        if (speechBubble != null)
        {
            // Portrait image
            var portraitObj = new GameObject("MentorPortrait", typeof(RectTransform));
            portraitObj.transform.SetParent(speechBubble.transform, false);

            var portraitRect = portraitObj.GetComponent<RectTransform>();
            portraitRect.anchorMin = new Vector2(0, 0.5f);
            portraitRect.anchorMax = new Vector2(0, 0.5f);
            portraitRect.pivot = new Vector2(0, 0.5f);
            portraitRect.sizeDelta = new Vector2(110, 110);
            portraitRect.anchoredPosition = new Vector2(15, 0);

            var portraitImg = portraitObj.AddComponent<Image>();
            portraitImg.sprite = MedicalSpriteFactory.DrawMentorPortrait();
            portraitImg.color = Color.white;

            // Name label
            var nameObj = new GameObject("MentorName", typeof(RectTransform));
            nameObj.transform.SetParent(speechBubble.transform, false);
            var nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 1);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.pivot = new Vector2(0, 1);
            nameRect.sizeDelta = new Vector2(-140, 30);
            nameRect.anchoredPosition = new Vector2(140, -5);

            var nameTmp = nameObj.AddComponent<TextMeshProUGUI>();
            nameTmp.text = "Dr. Olayinka";
            nameTmp.fontSize = 20;
            nameTmp.fontStyle = FontStyles.Bold;
            nameTmp.color = new Color(1f, 0.85f, 0.4f);
            nameTmp.alignment = TextAlignmentOptions.Left;

            // Shift speech text to make room for portrait
            var speechText = speechBubble.transform.Find("SpeechText");
            if (speechText != null)
            {
                var stRect = speechText.GetComponent<RectTransform>();
                stRect.offsetMin = new Vector2(140, 10);
                stRect.offsetMax = new Vector2(-20, -35);
            }
        }

        // ============================================
        // 8. UPDATE ACTION BUTTON ICONS (optional medical icons)
        // ============================================
        TryAssignButtonIcon("Btn_VeinSelection", MedicalSpriteFactory.DrawSwab());
        TryAssignButtonIcon("Btn_InsertCannula", MedicalSpriteFactory.DrawSyringe());
        TryAssignButtonIcon("Btn_DisposeSharps", MedicalSpriteFactory.DrawSharpsBin());
        TryAssignButtonIcon("Btn_Gloves", MedicalSpriteFactory.DrawGloves());
        TryAssignButtonIcon("Btn_Tourniquet", MedicalSpriteFactory.DrawTourniquet());

        // ============================================
        // 9. SAVE SCENE
        // ============================================
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log("✅ Hospital room built with procedural medical sprites!");
        EditorUtility.DisplayDialog("Room Added",
            "Hospital room built with custom-drawn medical sprites!\n\n" +
            "• Nurse character for MO\n" +
            "• Patient lying on hospital bed\n" +
            "• IV stand with bag and drip\n" +
            "• Dr. Olayinka portrait\n" +
            "• Medical icons on action buttons (syringe, gloves, sharps bin, etc.)\n\n" +
            "Press Play to see the full scene.",
            "Nice!");
    }

    static void TryAssignButtonIcon(string buttonName, Sprite icon)
    {
        var btn = GameObject.Find(buttonName);
        if (btn == null || icon == null) return;

        // Find or create an icon Image as the first child
        Transform iconT = btn.transform.Find("Icon");
        GameObject iconObj;
        if (iconT == null)
        {
            iconObj = new GameObject("Icon", typeof(RectTransform));
            iconObj.transform.SetParent(btn.transform, false);
            var r = iconObj.GetComponent<RectTransform>();
            r.anchorMin = new Vector2(0.5f, 0.5f);
            r.anchorMax = new Vector2(0.5f, 0.5f);
            r.sizeDelta = new Vector2(40, 40);
            r.anchoredPosition = new Vector2(0, 8);
        }
        else
        {
            iconObj = iconT.gameObject;
        }

        var img = iconObj.GetComponent<Image>();
        if (img == null) img = iconObj.AddComponent<Image>();
        img.sprite = icon;
        img.color = Color.white;
        img.preserveAspect = true;

        // Push the label text down to make room for the icon
        var textT = btn.transform.Find("Text");
        if (textT != null)
        {
            var r = textT.GetComponent<RectTransform>();
            r.offsetMin = new Vector2(0, -8);
            r.offsetMax = new Vector2(0, -20);
        }
    }

    // --- Helpers ---
    static GameObject CreateWorldSprite(string name, Transform parent, Color color, Vector3 pos, Vector2 size)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.transform.position = pos;
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        sr.color = color;
        obj.transform.localScale = new Vector3(size.x, size.y, 1f);
        return obj;
    }

    static GameObject CreateWorldSpriteFromSprite(string name, Transform parent, Sprite sprite, Vector3 pos, float scale)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.transform.position = pos;
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = Color.white;
        obj.transform.localScale = new Vector3(scale, scale, 1f);
        return obj;
    }
}
