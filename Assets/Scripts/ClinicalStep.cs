using System.Collections.Generic;

/// <summary>
/// The 12 core steps of peripheral IV cannulation, in clinical order.
/// The integer order of this enum IS the correct order, which makes
/// validation dead simple (see TurnManager).
/// </summary>
public enum ClinicalStep
{
    None = -1,
    IntroduceAndConsent = 0,
    HandHygiene = 1,
    GatherEquipment = 2,
    ApplyTourniquet = 3,
    VeinSelection = 4,        // mini-game
    DonGloves = 5,
    CleanSite = 6,
    InsertCannula = 7,        // mini-game
    ConfirmFlashback = 8,
    AdvanceAndRelease = 9,    // advance cannula, retract needle, release tourniquet
    SecureAndFlush = 10,
    DisposeSharps = 11        // mini-game
}

/// <summary>
/// Helper data: human-readable names + which steps are mini-games.
/// Keep all "what does this step mean" knowledge in one place.
/// </summary>
public static class ClinicalStepInfo
{
    public const int TotalSteps = 12;

    public static readonly Dictionary<ClinicalStep, string> DisplayName = new Dictionary<ClinicalStep, string>
    {
        { ClinicalStep.IntroduceAndConsent, "Introduce & Consent" },
        { ClinicalStep.HandHygiene,         "Hand Hygiene" },
        { ClinicalStep.GatherEquipment,     "Gather Equipment" },
        { ClinicalStep.ApplyTourniquet,     "Apply Tourniquet" },
        { ClinicalStep.VeinSelection,       "Vein Selection" },
        { ClinicalStep.DonGloves,           "Don Gloves" },
        { ClinicalStep.CleanSite,           "Clean Site" },
        { ClinicalStep.InsertCannula,       "Insert Cannula" },
        { ClinicalStep.ConfirmFlashback,    "Confirm Flashback" },
        { ClinicalStep.AdvanceAndRelease,   "Advance & Release Tourniquet" },
        { ClinicalStep.SecureAndFlush,      "Secure Dressing & Flush" },
        { ClinicalStep.DisposeSharps,       "Dispose Sharps" }
    };

    /// <summary>Steps that trigger a mini-game instead of resolving instantly.</summary>
    public static bool IsMiniGame(ClinicalStep step)
    {
        return step == ClinicalStep.VeinSelection
            || step == ClinicalStep.InsertCannula
            || step == ClinicalStep.DisposeSharps;
    }

    public static string Name(ClinicalStep step)
    {
        return DisplayName.TryGetValue(step, out var n) ? n : step.ToString();
    }

    /// <summary>The correct order as a list, index 0..11.</summary>
    public static List<ClinicalStep> CorrectOrder()
    {
        return new List<ClinicalStep>
        {
            ClinicalStep.IntroduceAndConsent,
            ClinicalStep.HandHygiene,
            ClinicalStep.GatherEquipment,
            ClinicalStep.ApplyTourniquet,
            ClinicalStep.VeinSelection,
            ClinicalStep.DonGloves,
            ClinicalStep.CleanSite,
            ClinicalStep.InsertCannula,
            ClinicalStep.ConfirmFlashback,
            ClinicalStep.AdvanceAndRelease,
            ClinicalStep.SecureAndFlush,
            ClinicalStep.DisposeSharps
        };
    }
}
