using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.IO;

public class AssetWiringWizard : EditorWindow
{
    const string SRC_DIR = "Assets/Sprites/latest gen/bg removed";

    [MenuItem("Tools/Wire AI Sprites (latest gen)")]
    public static void Wire()
    {
        AssetDatabase.Refresh();

        var map = LoadAndMapSprites();
        if (map.Count == 0)
        {
            EditorUtility.DisplayDialog("No sprites found",
                "No PNGs found in:\n" + SRC_DIR +
                "\n\nDrop your AI-generated PNGs there and rerun this wizard.",
                "OK");
            return;
        }

        string sceneName = EditorSceneManager.GetActiveScene().name;
        Debug.Log("=== Wiring AI sprites into scene: '" + sceneName + "' — matched " + map.Count + " sprites ===");

        WireScene(map);

        MarkAndSave();
        EditorUtility.DisplayDialog("AI Sprites Wired",
            "Wired " + map.Count + " AI-generated sprites into scene '" + sceneName + "'.\n\n" +
            "Check the console for a full list of placements.\n" +
            "Press Play to see the result.",
            "Nice!");
    }

    // ============================================================
    // LOAD + MAP SPRITES BY FILENAME PATTERN
    // ============================================================
    static Dictionary<string, Sprite> LoadAndMapSprites()
    {
        var map = new Dictionary<string, Sprite>();
        var absDir = Application.dataPath + "/Sprites/latest gen/bg removed";
        if (!Directory.Exists(absDir)) return map;

        foreach (var f in Directory.GetFiles(absDir, "*.png"))
        {
            string fname = Path.GetFileName(f);
            string assetPath = SRC_DIR + "/" + fname;
            string key = MatchKey(fname);
            if (key == null) { Debug.LogWarning("Unmatched sprite (skipped): " + fname); continue; }

            var sprite = EnsureSpriteImport(assetPath);
            if (sprite == null) { Debug.LogWarning("Failed to load sprite: " + assetPath); continue; }
            map[key] = sprite;
            Debug.Log("  matched: " + key + "  <-  " + fname);
        }
        return map;
    }

    static Sprite EnsureSpriteImport(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null && importer.textureType != TextureImporterType.Sprite)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    static string MatchKey(string fname)
    {
        string f = fname.ToLower();
        if (f.Contains("alcohol_swab")) return "alcohol_swab";
        if (f.Contains("handwash")) return "handwash";
        if (f.Contains("heart_monitor_mounted")) return "heart_monitor_wall";
        if (f.Contains("heart_monitor")) return "heart_monitor_small";
        if (f.Contains("night_scene")) return "title_bg";
        if (f.Contains("game_over_panel")) return "outcome_panel";
        if (f.Contains("large_yellow_sharps")) return "sharps_bin_large";
        if (f.Contains("neutral_patient")) return "outcome_neutral";
        if (f.Contains("blue_medical_gloves")) return "gloves";
        if (f.Contains("iv_cannula")) return "cannula";
        if (f.Contains("tourniquet")) return "tourniquet";
        if (f.Contains("start_shift")) return "start_button";
        if (f.Contains("button_background_blue")) return "btn_normal";
        if (f.Contains("button_background_brig")) return "btn_active";
        if (f.Contains("button_background_mute")) return "btn_done";
        if (f.Contains("sad_patient")) return "outcome_hurt";
        if (f.Contains("saline_flush")) return "saline_flush";
        if (f.Contains("gold_star")) return "star";
        if (f.Contains("equipment_tray")) return "equipment_tray";
        if (f.Contains("sidebar_panel")) return "step_list_panel";
        if (f.Contains("iv_dressing")) return "dressing";
        if (f.Contains("cursor_handle")) return "timing_handle";
        if (f.Contains("instruction_banner")) return "instruction_banner";
        if (f.Contains("speech_bubble")) return "speech_bubble";
        if (f.Contains("sharps_disposal_bin")) return "sharps_bin";
        if (f.Contains("forearm_barely")) return "vein_medium";
        if (f.Contains("forearm_very_small")) return "vein_poor";
        if (f.Contains("forearm_visible")) return "vein_good";
        if (f.Contains("logo_text")) return "logo";
        if (f.Contains("privacy_curtain")) return "curtain";
        if (f.Contains("portrait_of_dr_olayinka")) return "dr_portrait";
        if (f.Contains("same_nurse_character_mo_wearing")) return "mo_gloved";
        if (f.Contains("face_giving_a_thumbs_up")) return "face_happy";
        if (f.Contains("face_with_neutral")) return "face_neutral";
        if (f.Contains("face_with_pained")) return "face_hurt";
        if (f.Contains("iv_stand")) return "iv_stand";
        if (f.Contains("hospital_bed")) return "hospital_bed";
        if (f.Contains("hospital_room_interior")) return "room_bg";
        if (f.Contains("hospital_wall")) return "wall_tile";
        if (f.Contains("nurse_character_named_mo")) return "mo_nurse";
        if (f.Contains("patient_lying_on_a_hospital_bed")) return "patient_lying";
        if (f.Contains("floor_tiles")) return "floor_tile";
        return null;
    }

    // ============================================================
    // SCENE WIRING — works on whatever scene is currently open
    // ============================================================
    static void WireScene(Dictionary<string, Sprite> map)
    {
        // World/room sprites (always safe to add/replace)
        WireRoomSprites(map);
        WireMO(map);
        WireEquipmentTray(map);
        WireButtonIcons(map);
        WireButtonBackgrounds(map);
        WireUIPanels(map);
        WireMentorPortrait(map);
        WireMiniGames(map);
        WireOutcomePanel(map);
        WireTitleScreen(map);
    }

    // ---- ROOM: floor, walls, bg, curtain, heart monitor, bed, patient, IV stand ----
    static void WireRoomSprites(Dictionary<string, Sprite> map)
    {
        Transform room = EnsureContainer("AIHospitalRoom");

        // Background (top-down interior) — drawn behind everything
        if (map.TryGetValue("room_bg", out var bg))
            PlaceWorldSprite("RoomBG", room, bg, new Vector3(0, -0.5f, 0), 12f, -20);

        // Floor tile (cover ground)
        if (map.TryGetValue("floor_tile", out var floor))
            PlaceWorldSprite("Floor", room, floor, new Vector3(0, -2.5f, 0), 12f, -15);

        // Wall (back)
        if (map.TryGetValue("wall_tile", out var wall))
            PlaceWorldSprite("BackWall", room, wall, new Vector3(0, 2f, 0), 12f, -10);

        // Curtain (left corner)
        if (map.TryGetValue("curtain", out var curtain))
            PlaceWorldSprite("Curtain", room, curtain, new Vector3(-6f, 0f, 0), 3f, -5);

        // Heart monitor mounted on wall (right side)
        if (map.TryGetValue("heart_monitor_wall", out var monitor))
            PlaceWorldSprite("WallMonitor", room, monitor, new Vector3(5f, 1.8f, 0), 1.5f, -4);

        // Patient bed (center)
        Transform bedParent = EnsureChild("AIPatientBed", room);
        if (map.TryGetValue("hospital_bed", out var bedSpr))
            PlaceWorldSprite("Bed", bedParent, bedSpr, new Vector3(2f, -1.5f, 0), 4f, 0);

        // Patient lying on bed
        if (map.TryGetValue("patient_lying", out var patientSpr))
            PlaceWorldSprite("PatientLying", bedParent, patientSpr, new Vector3(2f, -1f, -0.1f), 3f, 1);

        // IV stand (left of bed)
        if (map.TryGetValue("iv_stand", out var ivSpr))
            PlaceWorldSprite("IVStand", room, ivSpr, new Vector3(-1f, -1.5f, 0), 2.5f, 2);

        // Handwash station (right wall)
        if (map.TryGetValue("handwash", out var washSpr))
            PlaceWorldSprite("HandwashStation", room, washSpr, new Vector3(6.5f, -1.5f, 0), 1.5f, 0);
    }

    // ---- MO character sprite ----
    static void WireMO(Dictionary<string, Sprite> map)
    {
        var mo = GameObject.Find("MO");
        if (mo == null) return;
        var sr = mo.GetComponent<SpriteRenderer>();
        if (sr == null) sr = mo.AddComponent<SpriteRenderer>();

        Sprite preferred = map.ContainsKey("mo_gloved") ? map["mo_gloved"] :
                            (map.ContainsKey("mo_nurse") ? map["mo_nurse"] : null);
        if (preferred != null)
        {
            sr.sprite = preferred;
            sr.color = Color.white;
            sr.sortingOrder = 5;
            mo.transform.localScale = new Vector3(3f, 3f, 1f);
            mo.transform.position = new Vector3(-3.5f, -1.5f, 0);
            Debug.Log("  MO sprite set: " + preferred.name);
        }
    }

    // ---- Equipment tray with item sprites positioned on it ----
    static void WireEquipmentTray(Dictionary<string, Sprite> map)
    {
        Transform trayParent = EnsureContainer("AIEquipmentTray");
        if (map.TryGetValue("equipment_tray", out var traySpr))
            PlaceWorldSprite("TrayBase", trayParent, traySpr, new Vector3(-5f, 0f, 0), 3f, 3);

        // Items arranged on tray (small, front of tray)
        Vector3 basePos = new Vector3(-6f, 0.2f, -0.1f);
        float spacing = 0.7f;
        string[] items = { "tourniquet", "cannula", "alcohol_swab", "dressing", "saline_flush", "gloves" };
        for (int i = 0; i < items.Length; i++)
        {
            if (map.TryGetValue(items[i], out var spr))
                PlaceWorldSprite("TrayItem_" + items[i], trayParent, spr,
                    basePos + new Vector3(i * spacing, 0, 0), 0.8f, 4 + i);
        }

        // Sharps bin (small) beside tray
        if (map.TryGetValue("sharps_bin", out var binSpr))
            PlaceWorldSprite("SharpsBin", trayParent, binSpr, new Vector3(-3.5f, -0.5f, 0), 1.2f, 5);
    }

    // ---- Button icons (12 action buttons) ----
    static void WireButtonIcons(Dictionary<string, Sprite> map)
    {
        var iconMap = new Dictionary<string, string>
        {
            { "Btn_HandHygiene", "handwash" },
            { "Btn_Gloves", "gloves" },
            { "Btn_PatientCheck", "patient_lying" },
            { "Btn_PrepSkin", "alcohol_swab" },
            { "Btn_VeinSelection", "cannula" },
            { "Btn_Tourniquet", "tourniquet" },
            { "Btn_AnchorVein", "iv_stand" },
            { "Btn_InsertCannula", "cannula" },
            { "Btn_SecureDressing", "dressing" },
            { "Btn_FlushSaline", "saline_flush" },
            { "Btn_DisposeSharps", "sharps_bin" },
            { "Btn_Document", "heart_monitor_small" },
        };

        foreach (var kvp in iconMap)
        {
            var btn = GameObject.Find(kvp.Key);
            if (btn == null) continue;
            if (!map.TryGetValue(kvp.Value, out var icon)) continue;
            AssignButtonIcon(btn, icon);
        }
    }

    static void AssignButtonIcon(GameObject btn, Sprite icon)
    {
        Transform iconT = btn.transform.Find("Icon");
        GameObject iconObj;
        if (iconT == null)
        {
            iconObj = new GameObject("Icon", typeof(RectTransform));
            iconObj.transform.SetParent(btn.transform, false);
            var r = iconObj.GetComponent<RectTransform>();
            r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
            r.sizeDelta = new Vector2(40, 40);
            r.anchoredPosition = new Vector2(0, 8);
        }
        else iconObj = iconT.gameObject;

        var img = iconObj.GetComponent<Image>();
        if (img == null) img = iconObj.AddComponent<Image>();
        img.sprite = icon; img.color = Color.white; img.preserveAspect = true;
    }

    // ---- Button backgrounds (Image.sprite on each button root) ----
    static void WireButtonBackgrounds(Dictionary<string, Sprite> map)
    {
        // Default normal; assign "active"/"done" variants at runtime by TurnManager
        if (!map.TryGetValue("btn_normal", out var normalSpr)) return;

        var buttons = GameObject.FindObjectsOfType<Button>();
        foreach (var b in buttons)
        {
            if (b.name == null || !b.name.StartsWith("Btn_")) continue;
            var img = b.GetComponent<Image>();
            if (img == null) continue;
            img.sprite = normalSpr;
            img.type = Image.Type.Sliced;
            img.color = Color.white;
            // Push label text below the icon
            var label = b.transform.Find("Text");
            if (label != null)
            {
                var r = label.GetComponent<RectTransform>();
                r.offsetMin = new Vector2(0, -8);
                r.offsetMax = new Vector2(0, -22);
            }
        }
        Debug.Log("  Button backgrounds wired (normal variant). TurnManager can swap to active/done at runtime.");
    }

    // ---- UI panels: speech bubble, instruction banner, step list, outcome ----
    static void WireUIPanels(Dictionary<string, Sprite> map)
    {
        TryAssignUIImage("SpeechBubble", map, "speech_bubble");
        TryAssignUIImage("InstructionBanner", map, "instruction_banner");
        TryAssignUIImage("StepListPanel", map, "step_list_panel");
    }

    static void TryAssignUIImage(string goName, Dictionary<string, Sprite> map, string key)
    {
        var go = GameObject.Find(goName);
        if (go == null) return;
        if (!map.TryGetValue(key, out var spr)) return;
        var img = go.GetComponent<Image>();
        if (img == null) img = go.AddComponent<Image>();
        img.sprite = spr; img.type = Image.Type.Sliced; img.color = Color.white;
        Debug.Log("  UI Image wired: " + goName + " <- " + key);
    }

    // ---- Dr. Olayinka portrait in speech bubble ----
    static void WireMentorPortrait(Dictionary<string, Sprite> map)
    {
        if (!map.TryGetValue("dr_portrait", out var portrait)) return;
        var bubble = GameObject.Find("SpeechBubble");
        if (bubble == null) return;

        Transform existing = bubble.transform.Find("MentorPortrait");
        GameObject portraitObj;
        if (existing == null)
        {
            portraitObj = new GameObject("MentorPortrait", typeof(RectTransform));
            portraitObj.transform.SetParent(bubble.transform, false);
            var r = portraitObj.GetComponent<RectTransform>();
            r.anchorMin = r.anchorMax = new Vector2(0, 0.5f);
            r.pivot = new Vector2(0, 0.5f);
            r.sizeDelta = new Vector2(110, 110);
            r.anchoredPosition = new Vector2(15, 0);
            portraitObj.AddComponent<Image>();
        }
        else portraitObj = existing.gameObject;

        var img = portraitObj.GetComponent<Image>();
        img.sprite = portrait; img.color = Color.white; img.preserveAspect = true;

        // Add Dr. name label if missing
        if (bubble.transform.Find("MentorName") == null)
        {
            var nameObj = new GameObject("MentorName", typeof(RectTransform));
            nameObj.transform.SetParent(bubble.transform, false);
            var nr = nameObj.GetComponent<RectTransform>();
            nr.anchorMin = new Vector2(0, 1); nr.anchorMax = new Vector2(1, 1);
            nr.pivot = new Vector2(0, 1); nr.sizeDelta = new Vector2(-140, 30);
            nr.anchoredPosition = new Vector2(140, -5);
            var tmp = nameObj.AddComponent<TMPro.TextMeshProUGUI>();
            tmp.text = "Dr. Olayinka"; tmp.fontSize = 20; tmp.fontStyle = TMPro.FontStyles.Bold;
            tmp.color = new Color(1f, 0.85f, 0.4f); tmp.alignment = TMPro.TextAlignmentOptions.Left;
        }
        Debug.Log("  Dr. Olayinka portrait wired into SpeechBubble.");
    }

    // ---- Mini-games: veins, timing bar, sharps bin ----
    static void WireMiniGames(Dictionary<string, Sprite> map)
    {
        // Vein options
        var veinMap = new Dictionary<string, string>
        {
            { "VeinGood", "vein_good" },
            { "VeinMedium", "vein_medium" },
            { "VeinPoor", "vein_poor" },
        };
        foreach (var kvp in veinMap)
        {
            var go = GameObject.Find(kvp.Key);
            if (go == null) continue;
            if (!map.TryGetValue(kvp.Value, out var spr)) continue;
            var sr = go.GetComponent<SpriteRenderer>();
            if (sr == null) sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spr; sr.color = Color.white; sr.sortingOrder = 5;
        }

        // Timing bar handle
        var handle = GameObject.Find("TimingHandle");
        if (handle != null && map.TryGetValue("timing_handle", out var hSpr))
        {
            var sr = handle.GetComponent<SpriteRenderer>();
            if (sr == null) sr = handle.AddComponent<SpriteRenderer>();
            sr.sprite = hSpr; sr.color = Color.white; sr.sortingOrder = 6;
        }

        // Sharps bin (large) for dispose mini-game
        var bin = GameObject.Find("SharpsDisposal");
        if (bin != null && map.TryGetValue("sharps_bin_large", out var bSpr))
        {
            var sr = bin.GetComponent<SpriteRenderer>();
            if (sr == null) sr = bin.AddComponent<SpriteRenderer>();
            sr.sprite = bSpr; sr.color = Color.white; sr.sortingOrder = 4;
        }
    }

    // ---- Outcome panel: 3 patient faces + star ----
    static void WireOutcomePanel(Dictionary<string, Sprite> map)
    {
        var panel = GameObject.Find("OutcomePanel");
        if (panel == null) return;
        if (map.TryGetValue("outcome_panel", out var bgSpr))
        {
            var img = panel.GetComponent<Image>();
            if (img == null) img = panel.AddComponent<Image>();
            img.sprite = bgSpr; img.type = Image.Type.Sliced; img.color = Color.white;
        }

        // Big patient face in center (use neutral by default; runtime swaps based on score)
        EnsureOutcomeFace(panel.transform, "OutcomeFaceGood", "face_happy", map, new Vector2(0, 30));
        EnsureOutcomeFace(panel.transform, "OutcomeFaceNeutral", "face_neutral", map, new Vector2(0, 30));
        EnsureOutcomeFace(panel.transform, "OutcomeFaceHurt", "face_hurt", map, new Vector2(0, 30));

        // Star (top-right)
        if (map.TryGetValue("star", out var starSpr))
        {
            Transform starT = panel.transform.Find("ScoreStar1");
            GameObject starObj;
            if (starT == null)
            {
                starObj = new GameObject("ScoreStar1", typeof(RectTransform));
                starObj.transform.SetParent(panel.transform, false);
                var r = starObj.GetComponent<RectTransform>();
                r.anchorMin = r.anchorMax = new Vector2(1, 1);
                r.pivot = new Vector2(1, 1); r.sizeDelta = new Vector2(64, 64);
                r.anchoredPosition = new Vector2(-30, -30);
                starObj.AddComponent<Image>();
            }
            else starObj = starT.gameObject;
            var img = starObj.GetComponent<Image>();
            img.sprite = starSpr; img.color = Color.white; img.preserveAspect = true;
        }
    }

    static void EnsureOutcomeFace(Transform parent, string name, string key, Dictionary<string, Sprite> map, Vector2 anchoredPos)
    {
        if (!map.TryGetValue(key, out var spr)) return;
        Transform t = parent.Find(name);
        GameObject go;
        if (t == null)
        {
            go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var r = go.GetComponent<RectTransform>();
            r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.5f);
            r.pivot = new Vector2(0.5f, 0.5f);
            r.sizeDelta = new Vector2(160, 160);
            r.anchoredPosition = anchoredPos;
            go.AddComponent<Image>();
        }
        else go = t.gameObject;
        var img = go.GetComponent<Image>();
        img.sprite = spr; img.color = Color.white; img.preserveAspect = true;
    }

    // ---- Title screen (if active scene is Title or has title UI) ----
    static void WireTitleScreen(Dictionary<string, Sprite> map)
    {
        // Try world-space title elements first
        if (map.TryGetValue("title_bg", out var bgSpr))
        {
            var bg = GameObject.Find("TitleBackground");
            if (bg != null)
            {
                var sr = bg.GetComponent<SpriteRenderer>();
                if (sr == null) sr = bg.AddComponent<SpriteRenderer>();
                sr.sprite = bgSpr; sr.color = Color.white; sr.sortingOrder = -20;
                bg.transform.localScale = new Vector3(20f, 10f, 1f);
                bg.transform.position = Vector3.zero;
                Debug.Log("  TitleBackground wired with night scene.");
            }
            // Also try UI version
            var uiBg = GameObject.Find("TitleBGPanel");
            if (uiBg != null)
            {
                var img = uiBg.GetComponent<Image>();
                if (img == null) img = uiBg.AddComponent<Image>();
                img.sprite = bgSpr; img.color = Color.white; img.preserveAspect = true;
            }
        }

        if (map.TryGetValue("logo", out var logoSpr))
        {
            var logo = GameObject.Find("TitleLogo");
            if (logo != null)
            {
                var img = logo.GetComponent<Image>();
                if (img == null) img = logo.AddComponent<Image>();
                img.sprite = logoSpr; img.color = Color.white; img.preserveAspect = true;
                Debug.Log("  TitleLogo wired.");
            }
        }

        if (map.TryGetValue("start_button", out var startSpr))
        {
            var btn = GameObject.Find("StartShiftButton");
            if (btn != null)
            {
                var img = btn.GetComponent<Image>();
                if (img == null) img = btn.AddComponent<Image>();
                img.sprite = startSpr; img.color = Color.white; img.preserveAspect = true;
                Debug.Log("  StartShiftButton wired.");
            }
        }

        if (map.TryGetValue("heart_monitor_small", out var monSpr))
        {
            var deco = GameObject.Find("TitleHeartMonitor");
            if (deco != null)
            {
                var img = deco.GetComponent<Image>();
                if (img == null) img = deco.AddComponent<Image>();
                img.sprite = monSpr; img.color = Color.white; img.preserveAspect = true;
            }
        }
    }

    // ============================================================
    // HELPERS — container/sprite creation
    // ============================================================
    static Transform EnsureContainer(string name)
    {
        var go = GameObject.Find(name);
        if (go == null) { go = new GameObject(name); go.transform.position = Vector3.zero; }
        return go.transform;
    }

    static Transform EnsureChild(string name, Transform parent)
    {
        var t = parent.Find(name);
        if (t == null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            t = go.transform;
        }
        return t;
    }

    static GameObject PlaceWorldSprite(string name, Transform parent, Sprite sprite, Vector3 pos, float scale, int sortOrder)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, true);
        go.transform.position = pos;
        go.transform.localScale = new Vector3(scale, scale, 1f);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = Color.white;
        sr.sortingOrder = sortOrder;
        return go;
    }

    static void MarkAndSave()
    {
        var scene = EditorSceneManager.GetActiveScene();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
    }
}
