using UnityEngine;

/// <summary>
/// Generates medical-themed sprites procedurally (pixel by pixel).
/// No external files needed. All sprites are 64x64 unless noted.
/// Call any DrawXxx() method to get a Sprite you can assign to a SpriteRenderer.
/// </summary>
public static class MedicalSpriteFactory
{
    const int SIZE = 64;

    // ===================================================================
    // PUBLIC API — call these to get sprites
    // ===================================================================

    public static Sprite DrawNurse()
    {
        var tex = NewTex();
        int cx = SIZE / 2;

        // Hair (top)
        FillRect(tex, cx - 10, 48, 20, 8, Hex("3B2210"));

        // Head (skin tone)
        FillRect(tex, cx - 8, 38, 16, 14, Hex("E8B894"));
        // Eyes
        SetPixelSafe(tex, cx - 4, 45, Color.black);
        SetPixelSafe(tex, cx + 3, 45, Color.black);

        // Body / scrubs (teal blue)
        FillRect(tex, cx - 14, 14, 28, 24, Hex("1A8FB2"));
        // Collar (V-neck)
        FillRect(tex, cx - 4, 34, 8, 4, Color.white);

        // Arms
        FillRect(tex, cx - 20, 16, 6, 20, Hex("1A8FB2"));
        FillRect(tex, cx + 14, 16, 6, 20, Hex("1A8FB2"));
        // Hands
        FillRect(tex, cx - 20, 12, 6, 5, Hex("E8B894"));
        FillRect(tex, cx + 14, 12, 6, 5, Hex("E8B894"));

        // Stethoscope (two dots + line)
        SetPixelSafe(tex, cx - 6, 36, Hex("2A2A2A"));
        SetPixelSafe(tex, cx + 5, 36, Hex("2A2A2A"));
        FillRect(tex, cx - 1, 30, 2, 6, Hex("2A2A2A"));

        // Legs / pants
        FillRect(tex, cx - 10, 2, 8, 14, Hex("2A3A4A"));
        FillRect(tex, cx + 2, 2, 8, 14, Hex("2A3A4A"));

        return ToSprite(tex);
    }

    public static Sprite DrawPatientLying()
    {
        // 64x32 — horizontal lying figure
        var tex = NewTex(64, 32);

        // Head (left side)
        FillRect(tex, 2, 14, 10, 12, Hex("E8B894"));
        // Hair
        FillRect(tex, 2, 22, 10, 4, Hex("3B2210"));

        // Body / hospital gown (pale blue)
        FillRect(tex, 12, 12, 40, 16, Hex("A8C8E0"));
        // Gown pattern dots
        SetPixelSafe(tex, 20, 20, Hex("7AA8D0"));
        SetPixelSafe(tex, 30, 16, Hex("7AA8D0"));
        SetPixelSafe(tex, 40, 22, Hex("7AA8D0"));

        // Arm
        FillRect(tex, 12, 24, 30, 4, Hex("E8B894"));

        // Blanket (lower half, white)
        FillRect(tex, 30, 8, 30, 6, Hex("F0F0F4"));
        FillRect(tex, 30, 8, 30, 1, Hex("D0D0D8")); // edge

        return ToSprite(tex, 64, 32);
    }

    public static Sprite DrawHospitalBed()
    {
        // 96x48
        var tex = NewTex(96, 48);

        // Bed frame (dark grey metal)
        FillRect(tex, 4, 8, 88, 6, Hex("4A4A52"));
        FillRect(tex, 4, 36, 88, 6, Hex("4A4A52"));
        // Legs
        FillRect(tex, 6, 0, 6, 10, Hex("3A3A42"));
        FillRect(tex, 84, 0, 6, 10, Hex("3A3A42"));
        FillRect(tex, 6, 38, 6, 10, Hex("3A3A42"));
        FillRect(tex, 84, 38, 6, 10, Hex("3A3A42"));

        // Mattress (white)
        FillRect(tex, 6, 14, 84, 22, Hex("F5F5F8"));
        // Mattress seam
        FillRect(tex, 6, 25, 84, 1, Hex("E0E0E5"));

        // Pillow
        FillRect(tex, 8, 28, 14, 6, Hex("E8E8EC"));

        // Headboard rail
        FillRect(tex, 2, 8, 4, 34, Hex("5A5A62"));
        // Footboard rail
        FillRect(tex, 90, 8, 4, 34, Hex("5A5A62"));

        // Side rail (raised)
        FillRect(tex, 20, 32, 56, 4, Hex("6A6A72"));
        FillRect(tex, 20, 32, 56, 1, Hex("8A8A92"));

        return ToSprite(tex, 96, 48);
    }

    public static Sprite DrawIVStand()
    {
        // 32x64 — tall narrow
        var tex = NewTex(32, 64);
        int cx = 16;

        // Base (wider at bottom)
        FillRect(tex, cx - 10, 0, 20, 4, Hex("3A3A42"));
        FillRect(tex, cx - 6, 4, 12, 3, Hex("4A4A52"));

        // Pole
        FillRect(tex, cx - 1, 7, 2, 45, Hex("8A8A92"));

        // Hook at top
        FillRect(tex, cx - 1, 52, 6, 2, Hex("8A8A92"));

        // IV bag (hanging)
        FillRect(tex, cx + 4, 38, 8, 14, Hex("B0D8F0"));
        // Bag cap
        FillRect(tex, cx + 5, 36, 6, 3, Hex("4A4A52"));
        // Fluid level line
        FillRect(tex, cx + 4, 44, 8, 1, Hex("8AB8D8"));
        // Drip
        SetPixelSafe(tex, cx + 7, 37, Hex("8AB8D8"));

        // Tube (going down from bag)
        FillRect(tex, cx + 7, 30, 1, 8, Hex("B0D8F0"));

        return ToSprite(tex, 32, 64);
    }

    public static Sprite DrawSyringe()
    {
        var tex = NewTex(64, 24);

        // Barrel (white/translucent)
        FillRect(tex, 8, 6, 36, 12, Hex("E8F0F4"));
        FillRect(tex, 8, 6, 36, 1, Hex("B8C8CC"));
        FillRect(tex, 8, 17, 36, 1, Hex("B8C8CC"));

        // Plunger
        FillRect(tex, 40, 4, 6, 16, Hex("4A8AB0"));
        FillRect(tex, 46, 7, 10, 10, Hex("3A6A88"));

        // Needle
        FillRect(tex, 0, 10, 8, 4, Hex("C0C0C8"));
        FillRect(tex, 0, 11, 8, 2, Hex("9A9AA2"));

        // Measurement marks
        for (int i = 12; i < 38; i += 6)
            SetPixelSafe(tex, i, 7, Hex("9A9AA2"));

        return ToSprite(tex, 64, 24);
    }

    public static Sprite DrawGloves()
    {
        var tex = NewTex(48, 48);

        // Left glove
        DrawHand(tex, 6, 8, Hex("4AB0D8"));
        // Right glove
        DrawHand(tex, 26, 8, Hex("4AB0D8"));

        return ToSprite(tex, 48, 48);
    }

    public static Sprite DrawTourniquet()
    {
        var tex = NewTex(64, 24);

        // Strap (red/orange band)
        FillRect(tex, 4, 8, 52, 8, Hex("D04040"));
        FillRect(tex, 4, 8, 52, 2, Hex("F06060"));
        FillRect(tex, 4, 14, 52, 2, Hex("A03030"));

        // Buckle
        FillRect(tex, 28, 6, 8, 12, Hex("8A8A92"));
        FillRect(tex, 30, 8, 4, 8, Hex("6A6A72"));

        return ToSprite(tex, 64, 24);
    }

    public static Sprite DrawSwab()
    {
        var tex = NewTex(48, 24);

        // Handle
        FillRect(tex, 2, 10, 30, 4, Hex("F0F0F0"));
        FillRect(tex, 2, 10, 30, 1, Hex("C8C8CC"));

        // Cotton tip
        FillRect(tex, 32, 6, 10, 12, Hex("FAFAFA"));
        SetPixelSafe(tex, 34, 8, Hex("E8E8EC"));
        SetPixelSafe(tex, 38, 14, Hex("E8E8EC"));

        return ToSprite(tex, 48, 24);
    }

    public static Sprite DrawSharpsBin()
    {
        var tex = NewTex(48, 56);

        // Bin body (yellow)
        FillRect(tex, 6, 8, 36, 44, Hex("F0C020"));
        FillRect(tex, 6, 8, 36, 2, Hex("FFE040"));
        FillRect(tex, 6, 48, 36, 4, Hex("D0A010"));

        // Lid (dark)
        FillRect(tex, 4, 4, 40, 6, Hex("4A4A52"));
        FillRect(tex, 4, 4, 40, 1, Hex("6A6A72"));

        // Opening slot
        FillRect(tex, 16, 5, 16, 2, Hex("1A1A22"));

        // Biohazard-ish symbol (simplified circle)
        FillRect(tex, 18, 24, 12, 12, Hex("1A1A22"));
        FillRect(tex, 20, 26, 8, 8, Hex("F0C020"));
        FillRect(tex, 23, 29, 2, 2, Hex("1A1A22"));

        return ToSprite(tex, 48, 56);
    }

    public static Sprite DrawMentorPortrait()
    {
        var tex = NewTex(64, 64);
        int cx = 32;

        // Background circle
        FillCircle(tex, cx, 32, 30, Hex("2A3A5A"));

        // Hair
        FillRect(tex, cx - 12, 44, 24, 10, Hex("1A1010"));

        // Head
        FillRect(tex, cx - 10, 32, 20, 16, Hex("6A4A30"));
        // Eyes (glasses)
        FillRect(tex, cx - 8, 40, 6, 3, Color.white);
        FillRect(tex, cx + 2, 40, 6, 3, Color.white);
        FillRect(tex, cx - 7, 41, 2, 1, Color.black);
        FillRect(tex, cx + 3, 41, 2, 1, Color.black);
        // Glasses bridge
        FillRect(tex, cx - 2, 41, 4, 1, Hex("2A2A2A"));

        // White coat
        FillRect(tex, cx - 16, 8, 32, 24, Color.white);
        FillRect(tex, cx - 2, 8, 4, 24, Hex("E0E0E4")); // coat opening
        // Stethoscope
        FillRect(tex, cx - 8, 22, 2, 8, Hex("2A2A2A"));
        FillRect(tex, cx + 6, 22, 2, 8, Hex("2A2A2A"));
        FillRect(tex, cx - 1, 14, 2, 10, Hex("2A2A2A"));
        SetPixelSafe(tex, cx - 8, 21, Hex("D0A020"));
        SetPixelSafe(tex, cx + 7, 21, Hex("D0A020"));

        // Smile
        FillRect(tex, cx - 4, 36, 8, 1, Hex("4A2A1A"));
        FillRect(tex, cx - 3, 35, 1, 1, Hex("4A2A1A"));
        FillRect(tex, cx + 2, 35, 1, 1, Hex("4A2A1A"));

        return ToSprite(tex);
    }

    // ===================================================================
    // INTERNAL HELPERS
    // ===================================================================

    static void DrawHand(Texture2D tex, int x, int y, Color color)
    {
        // Palm
        FillRect(tex, x, y, 14, 12, color);
        // Thumb
        FillRect(tex, x + 12, y + 4, 4, 4, color);
        // Fingers
        FillRect(tex, x + 2, y + 12, 2, 6, color);
        FillRect(tex, x + 6, y + 12, 2, 7, color);
        FillRect(tex, x + 10, y + 12, 2, 6, color);
        // Cuff
        FillRect(tex, x, y - 2, 14, 3, Hex("3A90B8"));
    }

    static Texture2D NewTex(int w = SIZE, int h = SIZE)
    {
        var tex = new Texture2D(w, h) { filterMode = FilterMode.Point };
        var px = new Color[w * h];
        for (int i = 0; i < px.Length; i++) px[i] = new Color(0, 0, 0, 0);
        tex.SetPixels(px);
        return tex;
    }

    static void FillRect(Texture2D tex, int x, int y, int w, int h, Color c)
    {
        for (int dx = 0; dx < w; dx++)
            for (int dy = 0; dy < h; dy++)
                SetPixelSafe(tex, x + dx, y + dy, c);
    }

    static void FillCircle(Texture2D tex, int cx, int cy, int r, Color c)
    {
        for (int x = -r; x <= r; x++)
            for (int y = -r; y <= r; y++)
                if (x * x + y * y <= r * r)
                    SetPixelSafe(tex, cx + x, cy + y, c);
    }

    static void SetPixelSafe(Texture2D tex, int x, int y, Color c)
    {
        if (x >= 0 && x < tex.width && y >= 0 && y < tex.height)
            tex.SetPixel(x, y, c);
    }

    static Sprite ToSprite(Texture2D tex, int w = SIZE, int h = SIZE)
    {
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f), tex.width);
    }

    static Color Hex(string hex)
    {
        hex = hex.Replace("#", "");
        byte r = System.Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = System.Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = System.Convert.ToByte(hex.Substring(4, 2), 16);
        return new Color32(r, g, b, 255);
    }
}
