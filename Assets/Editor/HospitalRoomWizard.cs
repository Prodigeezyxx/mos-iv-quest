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
        // 1. HOSPITAL ROOM BACKGROUND
        // ============================================
        var roomObj = new GameObject("HospitalRoom");
        roomObj.transform.position = Vector3.zero;

        // Floor
        var floor = CreateWorldSprite("Floor", roomObj.transform,
            new Color(0.85f, 0.85f, 0.88f), new Vector3(0, -3, 0), new Vector2(20, 6));
        floor.transform.SetAsFirstSibling();

        // Back wall
        var wall = CreateWorldSprite("BackWall", roomObj.transform,
            new Color(0.7f, 0.78f, 0.82f), new Vector3(0, 2, 0), new Vector2(20, 8));

        // Wall trim
        var trim = CreateWorldSprite("WallTrim", roomObj.transform,
            new Color(0.5f, 0.6f, 0.65f), new Vector3(0, -1.5f, 0), new Vector2(20, 0.3f));

        // ============================================
        // 2. PATIENT BED
        // ============================================
        var bedObj = new GameObject("PatientBed");
        bedObj.transform.SetParent(roomObj.transform, false);
        bedObj.transform.position = new Vector3(2, -1.5f, 0);

        // Bed frame
        CreateWorldSprite("BedFrame", bedObj.transform,
            new Color(0.4f, 0.45f, 0.5f), Vector3.zero, new Vector2(5, 2.5f));

        // Mattress
        CreateWorldSprite("Mattress", bedObj.transform,
            new Color(0.95f, 0.95f, 0.98f), new Vector3(0, 0.3f, 0), new Vector2(4.5f, 1.5f));

        // Pillow
        CreateWorldSprite("Pillow", bedObj.transform,
            new Color(0.9f, 0.92f, 0.95f), new Vector3(-1.5f, 0.4f, 0), new Vector2(1.2f, 1f));

        // ============================================
        // 3. PATIENT (lying on bed)
        // ============================================
        var patient = CreateWorldSprite("Patient", bedObj.transform,
            new Color(0.95f, 0.75f, 0.7f), new Vector3(0.3f, 0.5f, -0.1f), new Vector2(3.5f, 1.2f));

        // Patient gown
        CreateWorldSprite("Gown", bedObj.transform,
            new Color(0.3f, 0.5f, 0.75f), new Vector3(0.8f, 0.45f, -0.15f), new Vector2(2.5f, 1.3f));

        // Patient head
        CreateWorldSprite("PatientHead", bedObj.transform,
            new Color(0.95f, 0.75f, 0.7f), new Vector3(-1.3f, 0.5f, -0.1f), new Vector2(0.8f, 0.8f));

        // ============================================
        // 4. IV STAND (next to bed)
        // ============================================
        var ivStand = new GameObject("IVStand");
        ivStand.transform.SetParent(roomObj.transform, false);
        ivStand.transform.position = new Vector3(-1.5f, -1f, 0);

        // Pole
        CreateWorldSprite("IVPole", ivStand.transform,
            new Color(0.6f, 0.6f, 0.62f), Vector3.zero, new Vector2(0.15f, 4f));

        // Bag
        CreateWorldSprite("IVBag", ivStand.transform,
            new Color(0.7f, 0.85f, 0.95f, 0.8f), new Vector3(0, 1.5f, 0), new Vector2(0.6f, 0.9f));

        // Base
        CreateWorldSprite("IVBase", ivStand.transform,
            new Color(0.4f, 0.4f, 0.42f), new Vector3(0, -1.8f, 0), new Vector2(1f, 0.2f));

        // ============================================
        // 5. REPOSITION MO next to the bed
        // ============================================
        var mo = GameObject.Find("MO");
        if (mo != null)
        {
            mo.transform.position = new Vector3(-3, -1.5f, 0);
            mo.transform.localScale = new Vector3(1.5f, 2.5f, 1f);
            var sr = mo.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(0.2f, 0.6f, 0.9f); // nurse blue
            // Make sure MO renders above the room
            sr.sortingOrder = 5;
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
            cam.backgroundColor = new Color(0.5f, 0.6f, 0.65f);
        }

        // ============================================
        // 7. DR. OLAYINKA PORTRAIT (in speech bubble)
        // ============================================
        var speechBubble = GameObject.Find("SpeechBubble");
        if (speechBubble != null)
        {
            // Add portrait image on the left of the speech bubble
            var portraitObj = new GameObject("MentorPortrait", typeof(RectTransform));
            portraitObj.transform.SetParent(speechBubble.transform, false);

            var portraitRect = portraitObj.GetComponent<RectTransform>();
            portraitRect.anchorMin = new Vector2(0, 0.5f);
            portraitRect.anchorMax = new Vector2(0, 0.5f);
            portraitRect.pivot = new Vector2(0, 0.5f);
            portraitRect.sizeDelta = new Vector2(100, 100);
            portraitRect.anchoredPosition = new Vector2(15, 0);

            var portraitImg = portraitObj.AddComponent<Image>();
            portraitImg.color = new Color(0.8f, 0.6f, 0.4f); // warm brown placeholder

            // Add "Dr. Olayinka" label
            var nameObj = new GameObject("MentorName", typeof(RectTransform));
            nameObj.transform.SetParent(speechBubble.transform, false);
            var nameRect = nameObj.GetComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 1);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.pivot = new Vector2(0, 1);
            nameRect.sizeDelta = new Vector2(-130, 30);
            nameRect.anchoredPosition = new Vector2(130, -5);

            var nameTmp = nameObj.AddComponent<TextMeshProUGUI>();
            nameTmp.text = "Dr. Olayinka";
            nameTmp.fontSize = 20;
            nameTmp.fontStyle = FontStyles.Bold;
            nameTmp.color = new Color(1f, 0.85f, 0.4f);
            nameTmp.alignment = TextAlignmentOptions.Left;

            // Shift the speech text to the right to make room for portrait
            var speechText = speechBubble.transform.Find("SpeechText");
            if (speechText != null)
            {
                var stRect = speechText.GetComponent<RectTransform>();
                stRect.offsetMin = new Vector2(130, 10);
                stRect.offsetMax = new Vector2(-20, -35);
            }
        }

        // ============================================
        // 8. SAVE SCENE
        // ============================================
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log("✅ Hospital room, patient, and Dr. Olayinka portrait added!");
        EditorUtility.DisplayDialog("Room Added",
            "Hospital room, patient bed, IV stand, and Dr. Olayinka portrait are all in place!\n\n" +
            "MO is now standing next to the bed in nurse blue.\n\n" +
            "Press Play to see the full scene.",
            "Nice!");
    }

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
}
