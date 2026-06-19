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
        // LOAD KENNEY SPRITES
        // ============================================
        // Tiny Town tilesheet (12 cols x 11 rows, 16px tiles, 1px margin)
        var tilemapPath = "Assets/Sprites/Kenney/TinyTown/Tilemap/tilemap_packed.png";
        var tilemap = AssetDatabase.LoadAssetAtPath<Texture2D>(tilemapPath);

        // Roguelike Characters spritesheet (16px tiles, 1px margin)
        var charPath = "Assets/Sprites/Kenney/RoguelikeCharacters/Spritesheet/roguelikeChar_transparent.png";
        var charSheet = AssetDatabase.LoadAssetAtPath<Texture2D>(charPath);

        if (tilemap == null)
        {
            EditorUtility.DisplayDialog("Missing Assets",
                "Kenney sprites not found!\n\nExpected at:\n" + tilemapPath + "\n\n" +
                "Make sure you ran the download. Falling back to colored shapes.",
                "OK");
            SetupRoomFallback();
            return;
        }

        // Helper: extract a tile from the Tiny Town tilesheet
        // Grid is 12 cols x 11 rows. Tile (col,row) with 1px margins.
        Sprite GetTile(int col, int row)
        {
            int tileSize = 16;
            int margin = 1;
            int x = col * (tileSize + margin) + margin;
            int y = row * (tileSize + margin) + margin;
            var pixels = tilemap.GetPixels(x, tilemap.height - y - tileSize, tileSize, tileSize);
            var tex = new Texture2D(tileSize, tileSize);
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, tileSize, tileSize), new Vector2(0.5f, 0.5f), 16);
        }

        // Helper: extract a character from the Roguelike Characters spritesheet
        Sprite GetChar(int col, int row)
        {
            if (charSheet == null) return null;
            int tileSize = 16;
            int margin = 1;
            int x = col * (tileSize + margin) + margin;
            int y = row * (tileSize + margin) + margin;
            var pixels = charSheet.GetPixels(x, charSheet.height - y - tileSize, tileSize, tileSize);
            var tex = new Texture2D(tileSize, tileSize);
            tex.SetPixels(pixels);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, tileSize, tileSize), new Vector2(0.5f, 0.5f), 16);
        }

        // ============================================
        // 1. HOSPITAL ROOM FLOOR + WALLS (using Tiny Town tiles)
        // ============================================
        var roomObj = new GameObject("HospitalRoom");
        roomObj.transform.position = Vector3.zero;

        // Tiny Town tile indices (col, row) — approximate:
        // Floor tiles ~ (0,0)-(2,2), wall tiles ~ (3,0)-(5,2)
        // We'll use tile (0,0) for floor, (1,0) for wall

        // Build a floor grid
        int floorCols = 16;
        int floorRows = 8;
        Sprite floorTile = GetTile(0, 0);
        for (int x = 0; x < floorCols; x++)
        {
            for (int y = 0; y < floorRows; y++)
            {
                var tile = new GameObject($"Floor_{x}_{y}");
                tile.transform.SetParent(roomObj.transform, false);
                tile.transform.position = new Vector3(x - floorCols / 2f, y - floorRows / 2f - 2f, 0);
                var sr = tile.AddComponent<SpriteRenderer>();
                sr.sprite = floorTile;
                sr.sortingOrder = -10;
            }
        }

        // Back wall (top row of darker tiles)
        Sprite wallTile = GetTile(1, 0);
        for (int x = 0; x < floorCols; x++)
        {
            var tile = new GameObject($"Wall_{x}");
            tile.transform.SetParent(roomObj.transform, false);
            tile.transform.position = new Vector3(x - floorCols / 2f, floorRows / 2f - 1f, 0);
            var sr = tile.AddComponent<SpriteRenderer>();
            sr.sprite = wallTile;
            sr.sortingOrder = -9;
        }

        // ============================================
        // 2. PATIENT BED (built from tiles)
        // ============================================
        var bedObj = new GameObject("PatientBed");
        bedObj.transform.SetParent(roomObj.transform, false);
        bedObj.transform.position = new Vector3(2, -1.5f, 0);

        // Bed frame — use a darker tile
        var bedFrame = CreateWorldSpriteFromTile("BedFrame", bedObj.transform, GetTile(5, 1),
            Vector3.zero, new Vector2(3f, 1.5f));
        bedFrame.GetComponent<SpriteRenderer>().color = new Color(0.4f, 0.45f, 0.5f);

        // Mattress — lighter tile
        var mattress = CreateWorldSpriteFromTile("Mattress", bedObj.transform, GetTile(0, 2),
            new Vector3(0, 0.3f, 0), new Vector2(2.5f, 1f));
        mattress.GetComponent<SpriteRenderer>().color = new Color(0.95f, 0.95f, 0.98f);

        // Pillow
        var pillow = CreateWorldSpriteFromTile("Pillow", bedObj.transform, GetTile(0, 2),
            new Vector3(-0.9f, 0.4f, 0), new Vector2(0.7f, 0.6f));
        pillow.GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.92f, 0.95f);

        // ============================================
        // 3. PATIENT (character sprite on the bed)
        // ============================================
        Sprite patientSprite = GetChar(0, 0); // top-left character
        if (patientSprite != null)
        {
            var patient = CreateWorldSpriteFromSprite("Patient", bedObj.transform, patientSprite,
                new Vector3(0.3f, 0.5f, -0.1f), 4f);
            patient.GetComponent<SpriteRenderer>().color = new Color(1f, 0.9f, 0.85f);
        }
        else
        {
            // Fallback: colored shape
            CreateWorldSprite("Patient", bedObj.transform, new Color(0.95f, 0.75f, 0.7f),
                new Vector3(0.3f, 0.5f, -0.1f), new Vector2(2f, 0.8f));
        }

        // ============================================
        // 4. IV STAND (next to bed)
        // ============================================
        var ivStand = new GameObject("IVStand");
        ivStand.transform.SetParent(roomObj.transform, false);
        ivStand.transform.position = new Vector3(-1.5f, -1f, 0);

        // Pole
        CreateWorldSprite("IVPole", ivStand.transform,
            new Color(0.6f, 0.6f, 0.62f), Vector3.zero, new Vector2(0.1f, 3f));

        // Bag
        CreateWorldSprite("IVBag", ivStand.transform,
            new Color(0.7f, 0.85f, 0.95f, 0.8f), new Vector3(0, 1.2f, 0), new Vector2(0.5f, 0.7f));

        // Base
        CreateWorldSprite("IVBase", ivStand.transform,
            new Color(0.4f, 0.4f, 0.42f), new Vector3(0, -1.4f, 0), new Vector2(0.8f, 0.15f));

        // ============================================
        // 5. REPOSITION MO (use a Kenney character sprite)
        // ============================================
        var mo = GameObject.Find("MO");
        if (mo != null)
        {
            mo.transform.position = new Vector3(-3, -1.5f, 0);
            mo.transform.localScale = new Vector3(4f, 4f, 1f);

            var sr = mo.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                // Use a different character sprite for MO (col 1, row 0)
                Sprite moSprite = GetChar(1, 0);
                if (moSprite != null)
                {
                    sr.sprite = moSprite;
                    sr.color = Color.white; // use the sprite's own colors
                }
                else
                {
                    sr.color = new Color(0.2f, 0.6f, 0.9f); // nurse blue fallback
                }
                sr.sortingOrder = 5;
            }
        }

        // ============================================
        // 6. ADJUST CAMERA
        // ============================================
        var cam = Camera.main;
        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = 5f;
            cam.transform.position = new Vector3(0, -0.5f, -10);
            cam.backgroundColor = new Color(0.3f, 0.3f, 0.35f);
        }

        // ============================================
        // 7. DR. OLAYINKA PORTRAIT
        // ============================================
        var speechBubble = GameObject.Find("SpeechBubble");
        if (speechBubble != null)
        {
            var portraitObj = new GameObject("MentorPortrait", typeof(RectTransform));
            portraitObj.transform.SetParent(speechBubble.transform, false);

            var portraitRect = portraitObj.GetComponent<RectTransform>();
            portraitRect.anchorMin = new Vector2(0, 0.5f);
            portraitRect.anchorMax = new Vector2(0, 0.5f);
            portraitRect.pivot = new Vector2(0, 0.5f);
            portraitRect.sizeDelta = new Vector2(100, 100);
            portraitRect.anchoredPosition = new Vector2(15, 0);

            var portraitImg = portraitObj.AddComponent<Image>();
            // Use a character sprite for Dr. Olayinka too
            Sprite mentorSprite = GetChar(2, 0);
            if (mentorSprite != null)
            {
                portraitImg.sprite = mentorSprite;
                portraitImg.color = Color.white;
            }
            else
            {
                portraitImg.color = new Color(0.8f, 0.6f, 0.4f);
            }

            // Name label
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

        Debug.Log("✅ Hospital room built with real Kenney sprites!");
        EditorUtility.DisplayDialog("Room Added",
            "Hospital room built with real Kenney sprites!\n\n" +
            "• Tiny Town tiles for floor and walls\n" +
            "• Roguelike Characters sprites for MO, patient, and Dr. Olayinka\n" +
            "• Bed, IV stand, and all scene elements in place\n\n" +
            "Press Play to see the full scene.",
            "Nice!");
    }

    // Fallback if Kenney assets aren't present
    static void SetupRoomFallback()
    {
        var roomObj = new GameObject("HospitalRoom");
        roomObj.transform.position = Vector3.zero;

        CreateWorldSprite("Floor", roomObj.transform,
            new Color(0.85f, 0.85f, 0.88f), new Vector3(0, -3, 0), new Vector2(20, 6));
        CreateWorldSprite("BackWall", roomObj.transform,
            new Color(0.7f, 0.78f, 0.82f), new Vector3(0, 2, 0), new Vector2(20, 8));
        CreateWorldSprite("WallTrim", roomObj.transform,
            new Color(0.5f, 0.6f, 0.65f), new Vector3(0, -1.5f, 0), new Vector2(20, 0.3f));

        var bedObj = new GameObject("PatientBed");
        bedObj.transform.SetParent(roomObj.transform, false);
        bedObj.transform.position = new Vector3(2, -1.5f, 0);
        CreateWorldSprite("BedFrame", bedObj.transform, new Color(0.4f, 0.45f, 0.5f), Vector3.zero, new Vector2(5, 2.5f));
        CreateWorldSprite("Mattress", bedObj.transform, new Color(0.95f, 0.95f, 0.98f), new Vector3(0, 0.3f, 0), new Vector2(4.5f, 1.5f));
        CreateWorldSprite("Pillow", bedObj.transform, new Color(0.9f, 0.92f, 0.95f), new Vector3(-1.5f, 0.4f, 0), new Vector2(1.2f, 1f));
        CreateWorldSprite("Patient", bedObj.transform, new Color(0.95f, 0.75f, 0.7f), new Vector3(0.3f, 0.5f, -0.1f), new Vector2(3.5f, 1.2f));

        var mo = GameObject.Find("MO");
        if (mo != null)
        {
            mo.transform.position = new Vector3(-3, -1.5f, 0);
            var sr = mo.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = new Color(0.2f, 0.6f, 0.9f);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
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

    static GameObject CreateWorldSpriteFromTile(string name, Transform parent, Sprite tile, Vector3 pos, Vector2 size)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.transform.position = pos;
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = tile;
        sr.sortingOrder = 1;
        obj.transform.localScale = new Vector3(size.x / 16f, size.y / 16f, 1f);
        return obj;
    }

    static GameObject CreateWorldSpriteFromSprite(string name, Transform parent, Sprite sprite, Vector3 pos, float scale)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        obj.transform.position = pos;
        var sr = obj.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.sortingOrder = 2;
        obj.transform.localScale = new Vector3(scale, scale, 1f);
        return obj;
    }
}
