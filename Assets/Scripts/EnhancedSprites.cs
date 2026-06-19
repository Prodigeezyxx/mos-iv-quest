using UnityEngine;

/// <summary>
/// Enhanced procedural sprites at 128px with proper shading, outlines, and detail.
/// Every sprite has: base color, highlight (top-left lighter), shadow (bottom-right darker),
/// and a subtle outline for visibility.
/// </summary>
public static class EnhancedSprites
{
    const int S = 128; // base resolution

    // ===================================================================
    // NURSE (MO) — 128x128, top-down character with scrubs
    // ===================================================================
    public static Sprite DrawNurse()
    {
        var t = NewTex();
        int cx = S / 2;

        // Shadow under feet
        FillEllipse(t, cx, 8, 22, 6, new Color(0, 0, 0, 0.3f));

        // Legs (dark blue pants)
        FillRect(t, cx - 16, 10, 12, 24, Hex("1A2840"));
        FillRect(t, cx + 4, 10, 12, 24, Hex("1A2840"));
        // Shoes (white)
        FillRect(t, cx - 17, 6, 14, 6, Hex("F0F0F0"));
        FillRect(t, cx + 3, 6, 14, 6, Hex("F0F0F0"));
        FillRect(t, cx - 17, 6, 14, 2, Hex("D0D0D0")); // shoe shadow

        // Body / scrubs (teal with shading)
        FillRect(t, cx - 24, 34, 48, 36, Hex("0E8C9E")); // base
        FillRect(t, cx - 24, 62, 48, 8, Hex("0A6E7C")); // bottom shadow
        FillRect(t, cx - 24, 34, 48, 4, Hex("1AAAB8")); // top highlight
        // V-neck collar
        FillTriangle(t, cx - 6, 70, cx + 6, 70, cx, 50, Hex("F8F8F8"));
        // Name badge
        FillRect(t, cx + 8, 58, 10, 8, Hex("FFD700"));
        FillRect(t, cx + 8, 58, 10, 2, Hex("E6C200"));

        // Arms (same teal, slightly darker on right for depth)
        FillRect(t, cx - 34, 36, 10, 30, Hex("0E8C9E"));
        FillRect(t, cx + 24, 36, 10, 30, Hex("0A7A8A"));
        // Hands
        FillEllipse(t, cx - 29, 34, 6, 5, Hex("E8B894"));
        FillEllipse(t, cx + 29, 34, 6, 5, Hex("D8A884"));

        // Stethoscope
        DrawLine(t, cx - 8, 64, cx - 12, 56, Hex("2A2A3A"), 2);
        DrawLine(t, cx + 8, 64, cx + 12, 56, Hex("2A2A3A"), 2);
        DrawLine(t, cx - 12, 56, cx + 12, 56, Hex("2A2A3A"), 2);
        FillEllipse(t, cx - 12, 54, 4, 4, Hex("1A1A2A")); // chest piece
        FillEllipse(t, cx - 12, 54, 2, 2, Hex("3A3A5A")); // highlight

        // Neck
        FillRect(t, cx - 6, 70, 12, 6, Hex("E8B894"));

        // Head (skin with shading)
        FillEllipse(t, cx, 86, 16, 18, Hex("E8B894")); // base
        FillEllipse(t, cx - 4, 90, 10, 12, Hex("F0C8A4")); // highlight
        // Hair (short, dark brown)
        FillEllipse(t, cx, 98, 18, 10, Hex("2A1A10"));
        FillRect(t, cx - 16, 92, 32, 8, Hex("2A1A10"));
        FillRect(t, cx - 14, 96, 28, 4, Hex("1A0E08")); // hair shadow
        // Eyes
        FillRect(t, cx - 7, 86, 4, 3, Color.white);
        FillRect(t, cx + 3, 86, 4, 3, Color.white);
        SetPixel(t, cx - 6, 87, Hex("1A1A2A"));
        SetPixel(t, cx + 4, 87, Hex("1A1A2A"));
        // Smile
        DrawArc(t, cx, 82, 6, 0, 180, Hex("8A5A3A"), 1);

        // Cap (surgical cap)
        FillEllipse(t, cx, 102, 18, 8, Hex("0E8C9E"));
        FillRect(t, cx - 18, 98, 36, 6, Hex("0E8C9E"));
        FillRect(t, cx - 18, 98, 36, 2, Hex("1AAAB8")); // cap highlight

        return ToSprite(t);
    }

    // ===================================================================
    // PATIENT (lying on bed) — 128x64
    // ===================================================================
    public static Sprite DrawPatientLying()
    {
        var t = NewTex(128, 64);

        // Hair (left side, on pillow)
        FillEllipse(t, 20, 40, 14, 12, Hex("5A3A20"));
        FillEllipse(t, 18, 42, 10, 8, Hex("6A4A30"));

        // Head
        FillEllipse(t, 24, 38, 12, 13, Hex("E8B894"));
        FillEllipse(t, 28, 40, 8, 9, Hex("F0C8A4")); // highlight
        // Closed eyes (sleeping lines)
        DrawLine(t, 20, 38, 26, 38, Hex("8A5A3A"), 1);
        DrawLine(t, 30, 38, 36, 38, Hex("8A5A3A"), 1);
        // Mouth
        DrawLine(t, 26, 34, 30, 34, Hex("8A5A3A"), 1);

        // Hospital gown (pale blue with pattern)
        FillRect(t, 36, 24, 70, 24, Hex("A8D0E8"));
        FillRect(t, 36, 44, 70, 4, Hex("8AB8D0")); // shadow
        FillRect(t, 36, 24, 70, 3, Hex("C0D8F0")); // highlight
        // Gown pattern (small crosses)
        for (int i = 48; i < 100; i += 16)
        {
            SetPixel(t, i, 34, Hex("8AB8D0"));
            SetPixel(t, i + 1, 33, Hex("8AB8D0"));
            SetPixel(t, i + 1, 35, Hex("8AB8D0"));
            SetPixel(t, i - 1, 33, Hex("8AB8D0"));
            SetPixel(t, i - 1, 35, Hex("8AB8D0"));
        }

        // Arm (resting on gown)
        FillRect(t, 36, 42, 40, 6, Hex("E8B894"));
        FillRect(t, 36, 42, 40, 2, Hex("F0C8A4")); // arm highlight

        // Blanket (white, covering lower half)
        FillRect(t, 70, 12, 50, 16, Hex("F8F8FA"));
        FillRect(t, 70, 12, 50, 3, Hex("E0E0E4")); // blanket shadow
        FillRect(t, 70, 25, 50, 3, Hex("E8E8EC")); // fold line

        return ToSprite(t, 128, 64);
    }

    // ===================================================================
    // HOSPITAL BED — 192x96
    // ===================================================================
    public static Sprite DrawHospitalBed()
    {
        var t = NewTex(192, 96);

        // Shadow under bed
        FillEllipse(t, 96, 4, 80, 8, new Color(0, 0, 0, 0.2f));

        // Bed frame (metallic grey with gradient)
        FillRect(t, 12, 20, 168, 12, Hex("5A5A6A")); // top frame
        FillRect(t, 12, 20, 168, 3, Hex("7A7A8A")); // highlight
        FillRect(t, 12, 29, 168, 3, Hex("3A3A4A")); // shadow
        FillRect(t, 12, 60, 168, 12, Hex("5A5A6A")); // bottom frame
        FillRect(t, 12, 60, 168, 3, Hex("7A7A8A"));
        FillRect(t, 12, 69, 168, 3, Hex("3A3A4A"));

        // Legs
        FillRect(t, 16, 4, 8, 20, Hex("3A3A4A"));
        FillRect(t, 168, 4, 8, 20, Hex("3A3A4A"));
        FillRect(t, 16, 72, 8, 20, Hex("3A3A4A"));
        FillRect(t, 168, 72, 8, 20, Hex("3A3A4A"));
        // Wheel dots
        FillEllipse(t, 20, 4, 4, 3, Hex("1A1A2A"));
        FillEllipse(t, 172, 4, 4, 3, Hex("1A1A2A"));
        FillEllipse(t, 20, 92, 4, 3, Hex("1A1A2A"));
        FillEllipse(t, 172, 92, 4, 3, Hex("1A1A2A"));

        // Mattress (white with subtle gradient)
        FillRect(t, 14, 32, 164, 28, Hex("F8F8FA"));
        FillRect(t, 14, 32, 164, 4, Hex("FFFFFF")); // top highlight
        FillRect(t, 14, 56, 164, 4, Hex("E0E0E4")); // bottom shadow
        // Mattress seam
        DrawLine(t, 14, 46, 178, 46, Hex("D8D8DC"), 1);

        // Pillow
        FillRoundRect(t, 18, 48, 28, 12, 4, Hex("F0F0F4"));
        FillRoundRect(t, 18, 48, 28, 3, 4, Hex("FFFFFF")); // pillow highlight

        // Side rail (raised, metallic)
        FillRect(t, 40, 54, 120, 6, Hex("7A7A8A"));
        FillRect(t, 40, 54, 120, 2, Hex("9A9AAA")); // rail highlight
        FillRect(t, 40, 58, 120, 2, Hex("5A5A6A")); // rail shadow
        // Rail posts
        for (int x = 50; x < 160; x += 20)
            FillRect(t, x, 54, 2, 6, Hex("5A5A6A"));

        // Headboard (tall)
        FillRect(t, 4, 20, 8, 52, Hex("6A6A7A"));
        FillRect(t, 4, 20, 3, 52, Hex("8A8A9A")); // highlight
        // Footboard
        FillRect(t, 180, 20, 8, 52, Hex("6A6A7A"));
        FillRect(t, 180, 20, 3, 52, Hex("8A8A9A"));

        return ToSprite(t, 192, 96);
    }

    // ===================================================================
    // IV STAND — 64x128
    // ===================================================================
    public static Sprite DrawIVStand()
    {
        var t = NewTex(64, 128);
        int cx = 32;

        // Shadow
        FillEllipse(t, cx, 4, 20, 5, new Color(0, 0, 0, 0.25f));

        // Base (5-star wheel base)
        for (int i = 0; i < 5; i++)
        {
            float angle = i * 72f * Mathf.Deg2Rad;
            int ex = cx + (int)(Mathf.Cos(angle) * 18);
            int ey = 8 + (int)(Mathf.Sin(angle) * 6);
            FillEllipse(t, ex, ey, 6, 3, Hex("3A3A4A"));
        }
        FillEllipse(t, cx, 8, 8, 4, Hex("5A5A6A")); // center hub
        FillEllipse(t, cx, 8, 5, 2, Hex("7A7A8A")); // hub highlight

        // Pole (chrome with gradient)
        FillRect(t, cx - 2, 12, 4, 90, Hex("9A9AA8"));
        FillRect(t, cx - 2, 12, 1, 90, Hex("C0C0CC")); // highlight strip
        FillRect(t, cx + 1, 12, 1, 90, Hex("6A6A78")); // shadow strip

        // Adjustable clamp
        FillRect(t, cx - 4, 50, 8, 6, Hex("5A5A6A"));
        FillRect(t, cx - 4, 50, 8, 2, Hex("7A7A8A")); // clamp highlight

        // Hook (curved at top)
        FillRect(t, cx - 2, 102, 4, 8, Hex("9A9AA8"));
        FillRect(t, cx + 2, 104, 8, 3, Hex("9A9AA8"));
        FillRect(t, cx + 8, 104, 2, 10, Hex("9A9AA8"));

        // IV Bag (hanging from hook)
        FillRoundRect(t, cx + 6, 78, 16, 28, 3, Hex("C0E0F0")); // bag body
        FillRoundRect(t, cx + 6, 78, 16, 6, 3, Hex("E0F0FA")); // bag top highlight
        FillRoundRect(t, cx + 6, 98, 16, 8, 3, Hex("90C0D8")); // bag bottom (fluid)
        // Bag cap
        FillRect(t, cx + 8, 74, 12, 5, Hex("4A4A5A"));
        FillRect(t, cx + 8, 74, 12, 2, Hex("6A6A7A")); // cap highlight
        // Label
        FillRect(t, cx + 9, 86, 10, 6, Hex("F8F8F8"));
        DrawLine(t, cx + 9, 89, cx + 18, 89, Hex("C0C0C8"), 1);
        DrawLine(t, cx + 9, 91, cx + 18, 91, Hex("C0C0C8"), 1);

        // Drip tube (from bag going down)
        DrawLine(t, cx + 14, 78, cx + 14, 70, Hex("C0E0F0"), 2);
        DrawLine(t, cx + 14, 70, cx + 4, 60, Hex("C0E0F0"), 2);
        // Drip drop
        FillEllipse(t, cx + 4, 58, 2, 3, Hex("90C0D8"));

        return ToSprite(t, 64, 128);
    }

    // ===================================================================
    // DR. OLAYINKA PORTRAIT — 128x128
    // ===================================================================
    public static Sprite DrawMentorPortrait()
    {
        var t = NewTex();
        int cx = S / 2;

        // Background (warm gradient circle)
        FillEllipse(t, cx, cx, 60, 60, Hex("1A2A4A"));
        FillEllipse(t, cx - 10, cx + 10, 50, 50, Hex("2A3A5A")); // lighter inner

        // White coat (shoulders + torso)
        FillEllipse(t, cx, 30, 44, 30, Hex("F8F8FA")); // coat base
        FillRect(t, cx - 2, 20, 4, 40, Hex("E0E0E4")); // coat opening line
        // Lapels
        FillTriangle(t, cx - 4, 50, cx - 20, 30, cx - 4, 30, Hex("F0F0F2"));
        FillTriangle(t, cx + 4, 50, cx + 20, 30, cx + 4, 30, Hex("F0F0F2"));
        // Coat highlight
        FillEllipse(t, cx - 12, 35, 10, 20, Hex("FFFFFF"));
        FillEllipse(t, cx + 12, 35, 10, 20, Hex("F4F4F6"));

        // Stethoscope (hanging on neck)
        DrawArc(t, cx, 48, 24, 200, 340, Hex("1A1A2A"), 2); // arc
        FillEllipse(t, cx - 20, 30, 5, 5, Hex("2A2A3A")); // chest piece
        FillEllipse(t, cx - 20, 30, 3, 3, Hex("4A4A5A")); // chest piece highlight
        DrawLine(t, cx - 20, 30, cx - 16, 40, Hex("1A1A2A"), 2); // tube

        // Neck
        FillRect(t, cx - 8, 50, 16, 12, Hex("6A4A30"));
        FillRect(t, cx - 8, 50, 16, 3, Hex("7A5A40")); // neck highlight

        // Head (darker skin tone)
        FillEllipse(t, cx, 72, 22, 24, Hex("6A4A30")); // base
        FillEllipse(t, cx - 6, 76, 14, 16, Hex("7A5A3A")); // highlight
        FillEllipse(t, cx + 8, 68, 8, 10, Hex("5A3A20")); // shadow side

        // Hair (short, dark)
        FillEllipse(t, cx, 94, 24, 12, Hex("1A1008"));
        FillRect(t, cx - 22, 84, 44, 10, Hex("1A1008"));
        FillRect(t, cx - 20, 88, 40, 4, Hex("0A0804")); // hair shadow
        // Grey streaks
        for (int i = cx - 16; i < cx + 16; i += 6)
            SetPixel(t, i, 96, Hex("6A6A6A"));

        // Glasses
        FillRoundRect(t, cx - 14, 72, 10, 8, 2, new Color(0.8f, 0.9f, 1f, 0.6f));
        FillRoundRect(t, cx + 4, 72, 10, 8, 2, new Color(0.8f, 0.9f, 1f, 0.6f));
        // Glasses frame
        DrawRect(t, cx - 14, 72, 10, 8, Hex("2A2A3A"), 1);
        DrawRect(t, cx + 4, 72, 10, 8, Hex("2A2A3A"), 1);
        DrawLine(t, cx - 4, 76, cx + 4, 76, Hex("2A2A3A"), 1); // bridge
        // Eyes behind glasses
        SetPixel(t, cx - 10, 76, Hex("1A1A2A"));
        SetPixel(t, cx + 8, 76, Hex("1A1A2A"));

        // Nose
        DrawLine(t, cx, 70, cx - 2, 64, Hex("5A3A20"), 1);
        DrawLine(t, cx - 2, 64, cx + 2, 64, Hex("5A3A20"), 1);

        // Smile (warm, confident)
        DrawArc(t, cx, 62, 8, 0, 180, Hex("4A2A1A"), 1);
        FillRect(t, cx - 3, 62, 6, 1, Hex("5A3A2A")); // teeth hint

        // Earrings (small gold dots)
        SetPixel(t, cx - 22, 72, Hex("FFD700"));
        SetPixel(t, cx + 22, 72, Hex("FFD700"));

        return ToSprite(t);
    }

    // ===================================================================
    // MEDICAL ICONS (for action buttons) — 64x64 each
    // ===================================================================
    public static Sprite DrawSyringeIcon()
    {
        var t = NewTex(64, 64);
        // Barrel
        FillRoundRect(t, 14, 28, 32, 10, 2, Hex("E8F0F4"));
        FillRoundRect(t, 14, 28, 32, 3, 2, Hex("FFFFFF")); // highlight
        FillRoundRect(t, 14, 35, 32, 3, 2, Hex("B8C8CC")); // shadow
        // Plunger
        FillRect(t, 44, 26, 6, 14, Hex("4A8AB0"));
        FillRect(t, 44, 26, 6, 3, Hex("6AABD0")); // plunger highlight
        FillRect(t, 50, 30, 8, 6, Hex("3A6A88"));
        // Needle
        FillRect(t, 4, 32, 10, 3, Hex("C0C0C8"));
        FillRect(t, 4, 32, 10, 1, Hex("E0E0E4")); // needle highlight
        // Tip
        SetPixel(t, 3, 33, Hex("9A9AA2"));
        // Measurement marks
        for (int x = 18; x < 42; x += 6)
            SetPixel(t, x, 29, Hex("8A8A92"));
        return ToSprite(t, 64, 64);
    }

    public static Sprite DrawGlovesIcon()
    {
        var t = NewTex(64, 64);
        // Left glove
        DrawHandIcon(t, 6, 16, Hex("4AB0D8"));
        // Right glove (mirrored)
        DrawHandIcon(t, 34, 16, Hex("4AB0D8"));
        return ToSprite(t, 64, 64);
    }

    public static Sprite DrawTourniquetIcon()
    {
        var t = NewTex(64, 64);
        // Strap
        FillRoundRect(t, 6, 28, 52, 10, 3, Hex("D04040"));
        FillRoundRect(t, 6, 28, 52, 3, 3, Hex("F06060")); // highlight
        FillRoundRect(t, 6, 35, 52, 3, 3, Hex("A03030")); // shadow
        // Buckle
        FillRoundRect(t, 26, 24, 12, 18, 2, Hex("8A8A92"));
        FillRoundRect(t, 26, 24, 12, 3, 2, Hex("AAAAAA"));
        FillRoundRect(t, 28, 26, 8, 14, 2, Hex("6A6A72")); // buckle hole
        return ToSprite(t, 64, 64);
    }

    public static Sprite DrawSwabIcon()
    {
        var t = NewTex(64, 64);
        // Handle
        FillRect(t, 4, 30, 36, 5, Hex("F0F0F0"));
        FillRect(t, 4, 30, 36, 2, Hex("FFFFFF")); // highlight
        // Cotton tip
        FillEllipse(t, 44, 32, 8, 9, Hex("FAFAFA"));
        FillEllipse(t, 42, 34, 5, 5, Hex("FFFFFF")); // tip highlight
        SetPixel(t, 46, 28, Hex("E0E0E4")); // tip shadow dot
        return ToSprite(t, 64, 64);
    }

    public static Sprite DrawSharpsBinIcon()
    {
        var t = NewTex(64, 64);
        // Bin body
        FillRoundRect(t, 12, 12, 40, 44, 3, Hex("F0C020"));
        FillRoundRect(t, 12, 12, 40, 5, 3, Hex("FFE040")); // top highlight
        FillRoundRect(t, 12, 48, 40, 8, 3, Hex("D0A010")); // bottom shadow
        // Lid
        FillRoundRect(t, 8, 54, 48, 6, 2, Hex("4A4A52"));
        FillRoundRect(t, 8, 54, 48, 2, 2, Hex("6A6A72")); // lid highlight
        // Opening slot
        FillRect(t, 22, 55, 20, 2, Hex("1A1A22"));
        // Biohazard symbol (simplified)
        FillCircle(t, 32, 30, 8, Hex("1A1A22"));
        FillCircle(t, 32, 30, 5, Hex("F0C020"));
        FillCircle(t, 32, 30, 2, Hex("1A1A22"));
        return ToSprite(t, 64, 64);
    }

    public static Sprite DrawClipboardIcon()
    {
        var t = NewTex(64, 64);
        // Board
        FillRoundRect(t, 14, 6, 36, 52, 3, Hex("8A5A2A"));
        FillRoundRect(t, 14, 6, 36, 52, 3, Hex("A06A3A")); // overlay
        FillRoundRect(t, 16, 8, 32, 48, 2, Hex("F8F8F0")); // paper
        // Clip
        FillRoundRect(t, 24, 4, 16, 8, 2, Hex("5A5A5A"));
        FillRoundRect(t, 24, 4, 16, 3, 2, Hex("7A7A7A"));
        // Lines on paper
        for (int y = 18; y < 50; y += 6)
            DrawLine(t, 20, y, 44, y, Hex("C0C0C8"), 1);
        return ToSprite(t, 64, 64);
    }

    public static Sprite DrawHandwashIcon()
    {
        var t = NewTex(64, 64);
        // Hand
        FillEllipse(t, 28, 36, 14, 16, Hex("E8B894"));
        FillEllipse(t, 26, 38, 10, 12, Hex("F0C8A4")); // highlight
        // Fingers
        for (int i = 0; i < 4; i++)
            FillRect(t, 20 + i * 4, 48, 3, 8, Hex("E8B894"));
        // Water drops
        FillEllipse(t, 42, 44, 4, 6, Hex("4AB0D8"));
        FillEllipse(t, 48, 36, 3, 4, Hex("6AD0F0"));
        FillEllipse(t, 44, 28, 3, 4, Hex("4AB0D8"));
        return ToSprite(t, 64, 64);
    }

    // ===================================================================
    // INTERNAL HELPERS (enhanced drawing primitives)
    // ===================================================================

    static void DrawHandIcon(Texture2D t, int x, int y, Color c)
    {
        // Palm
        FillRoundRect(t, x, y, 16, 14, 3, c);
        // Thumb
        FillEllipse(t, x + 14, y + 6, 4, 4, c);
        // Fingers
        FillRoundRect(t, x + 2, y + 14, 3, 8, 1, c);
        FillRoundRect(t, x + 6, y + 14, 3, 9, 1, c);
        FillRoundRect(t, x + 10, y + 14, 3, 8, 1, c);
        // Cuff
        FillRoundRect(t, x, y - 3, 16, 4, 2, Hex("3A90B8"));
        // Highlight on palm
        FillRoundRect(t, x + 1, y + 8, 14, 3, 2, new Color(c.r + 0.1f, c.g + 0.1f, c.b + 0.1f, 1));
    }

    static Texture2D NewTex(int w = S, int h = S)
    {
        var t = new Texture2D(w, h) { filterMode = FilterMode.Point };
        var px = new Color[w * h];
        for (int i = 0; i < px.Length; i++) px[i] = new Color(0, 0, 0, 0);
        t.SetPixels(px);
        return t;
    }

    static void FillRect(Texture2D t, int x, int y, int w, int h, Color c)
    {
        for (int dx = 0; dx < w; dx++)
            for (int dy = 0; dy < h; dy++)
                SetPixel(t, x + dx, y + dy, c);
    }

    static void FillRoundRect(Texture2D t, int x, int y, int w, int h, int r, Color c)
    {
        // Fill center
        FillRect(t, x + r, y, w - 2 * r, h, c);
        FillRect(t, x, y + r, w, h - 2 * r, c);
        // Corners
        FillEllipse(t, x + r, y + r, r, r, c);
        FillEllipse(t, x + w - r - 1, y + r, r, r, c);
        FillEllipse(t, x + r, y + h - r - 1, r, r, c);
        FillEllipse(t, x + w - r - 1, y + h - r - 1, r, r, c);
    }

    static void FillEllipse(Texture2D t, int cx, int cy, int rx, int ry, Color c)
    {
        for (int x = -rx; x <= rx; x++)
            for (int y = -ry; y <= ry; y++)
                if ((x * x * ry * ry + y * y * rx * rx) <= rx * rx * ry * ry)
                    SetPixel(t, cx + x, cy + y, c);
    }

    static void FillCircle(Texture2D t, int cx, int cy, int r, Color c)
    {
        FillEllipse(t, cx, cy, r, r, c);
    }

    static void FillTriangle(Texture2D t, int x1, int y1, int x2, int y2, int x3, int y3, Color c)
    {
        int minX = Mathf.Min(x1, x2, x3), maxX = Mathf.Max(x1, x2, x3);
        int minY = Mathf.Min(y1, y2, y3), maxY = Mathf.Max(y1, y2, y3);
        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            {
                float d1 = Sign(x, y, x1, y1, x2, y2);
                float d2 = Sign(x, y, x2, y2, x3, y3);
                float d3 = Sign(x, y, x3, y3, x1, y1);
                bool neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
                bool pos = (d1 > 0) || (d2 > 0) || (d3 > 0);
                if (!(neg && pos)) SetPixel(t, x, y, c);
            }
    }

    static float Sign(int px, int py, int x1, int y1, int x2, int y2)
    {
        return (px - x2) * (y1 - y2) - (x1 - x2) * (py - y2);
    }

    static void DrawLine(Texture2D t, int x1, int y1, int x2, int y2, Color c, int thickness = 1)
    {
        int dx = Mathf.Abs(x2 - x1), dy = Mathf.Abs(y2 - y1);
        int sx = x1 < x2 ? 1 : -1, sy = y1 < y2 ? 1 : -1;
        int err = dx - dy;
        while (true)
        {
            for (int tx = -thickness / 2; tx <= thickness / 2; tx++)
                for (int ty = -thickness / 2; ty <= thickness / 2; ty++)
                    SetPixel(t, x1 + tx, y1 + ty, c);
            if (x1 == x2 && y1 == y2) break;
            int e2 = 2 * err;
            if (e2 > -dy) { err -= dy; x1 += sx; }
            if (e2 < dx) { err += dx; y1 += sy; }
        }
    }

    static void DrawRect(Texture2D t, int x, int y, int w, int h, Color c, int thickness = 1)
    {
        DrawLine(t, x, y, x + w, y, c, thickness);
        DrawLine(t, x, y + h, x + w, y + h, c, thickness);
        DrawLine(t, x, y, x, y + h, c, thickness);
        DrawLine(t, x + w, y, x + w, y + h, c, thickness);
    }

    static void DrawArc(Texture2D t, int cx, int cy, int r, float startAngle, float endAngle, Color c, int thickness = 1)
    {
        for (float a = startAngle; a <= endAngle; a += 1f)
        {
            float rad = a * Mathf.Deg2Rad;
            int x = cx + (int)(Mathf.Cos(rad) * r);
            int y = cy + (int)(Mathf.Sin(rad) * r);
            for (int tx = -thickness / 2; tx <= thickness / 2; tx++)
                for (int ty = -thickness / 2; ty <= thickness / 2; ty++)
                    SetPixel(t, x + tx, y + ty, c);
        }
    }

    static void SetPixel(Texture2D t, int x, int y, Color c)
    {
        if (x >= 0 && x < t.width && y >= 0 && y < t.height)
            t.SetPixel(x, y, c);
    }

    static Sprite ToSprite(Texture2D t, int w = S, int h = S)
    {
        t.Apply();
        return Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f), t.width);
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
