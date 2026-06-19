using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Drives the turn flow and validates each action against the correct
/// clinical order. State machine: PlayerTurn -> Resolving -> Feedback -> next.
///
/// Wire this up in the Inspector: drag your UIManager, MiniGameController
/// and AIFeedback objects into the matching fields.
/// </summary>
public class TurnManager : MonoBehaviour
{
    public enum Phase { PlayerTurn, Resolving, Feedback, ShiftComplete }

    [Header("Scene references (drag in Inspector)")]
    public UIManager ui;
    public MiniGameController miniGames;
    public AIFeedback aiFeedback;

    [Header("Runtime state (read-only)")]
    public Phase CurrentPhase = Phase.PlayerTurn;
    public int CurrentStepIndex = 0;   // how far along the correct order we are

    [Header("Fired when the shift (level) ends")]
    public UnityEvent onShiftComplete;

    private List<ClinicalStep> _correctOrder;

    private void Start()
    {
        _correctOrder = ClinicalStepInfo.CorrectOrder();
        if (GameManager.Instance != null) GameManager.Instance.ResetSession();
        CurrentPhase = Phase.PlayerTurn;
        CurrentStepIndex = 0;
        if (ui != null)
        {
            ui.SetScore(0);
            ui.ShowMentorMessage("Welcome to the night shift, MO. Let's start with the patient. What's step one?");
        }
    }

    /// <summary>
    /// Called by ActionButton when MO clicks an action tile.
    /// </summary>
    public void PerformStep(ClinicalStep step)
    {
        if (CurrentPhase != Phase.PlayerTurn) return; // ignore clicks mid-resolve
        if (GameManager.Instance == null) return;

        var gm = GameManager.Instance;
        gm.PlayerSequence.Add(step);

        ClinicalStep expected = (CurrentStepIndex < _correctOrder.Count)
            ? _correctOrder[CurrentStepIndex]
            : ClinicalStep.None;

        bool correct = step == expected;

        if (correct)
        {
            CurrentPhase = Phase.Resolving;

            if (ClinicalStepInfo.IsMiniGame(step) && miniGames != null)
            {
                // Hand off to the mini-game; it calls OnMiniGameComplete when done.
                miniGames.Launch(step, OnMiniGameComplete);
                return;
            }

            ResolveCorrectStep(step);
        }
        else
        {
            // Wrong order: dock a few points, log it, but DON'T block the flow.
            gm.AddScore(-GameManager.WrongStepPenalty);
            string msg = $"Hold on — '{ClinicalStepInfo.Name(step)}' isn't next. " +
                         $"You should '{ClinicalStepInfo.Name(expected)}' first.";
            gm.RecordError($"out_of_order: did '{step}' when '{expected}' was expected");
            if (ui != null)
            {
                ui.SetScore(gm.Score);
                ui.ShowMentorMessage(msg);
            }
        }
    }

    private void ResolveCorrectStep(ClinicalStep step)
    {
        var gm = GameManager.Instance;
        gm.AddScore(GameManager.CorrectStepPoints);
        gm.CompletedSteps.Add(step);
        CurrentStepIndex++;

        if (ui != null)
        {
            ui.SetScore(gm.Score);
            ui.ShowMentorMessage($"Nice — '{ClinicalStepInfo.Name(step)}' done. " + NextHint());
        }

        AdvancePhase();
    }

    private void OnMiniGameComplete(ClinicalStep step, int score)
    {
        var gm = GameManager.Instance;

        switch (step)
        {
            case ClinicalStep.VeinSelection: gm.VeinScore = score; break;
            case ClinicalStep.InsertCannula: gm.InsertionScore = score; break;
            case ClinicalStep.DisposeSharps: gm.SharpsScore = score; break;
        }

        // Mini-game contributes its score scaled to step points.
        gm.AddScore(Mathf.RoundToInt(GameManager.CorrectStepPoints * (score / 100f)));
        gm.CompletedSteps.Add(step);
        CurrentStepIndex++;

        if (ui != null)
        {
            ui.SetScore(gm.Score);
            ui.ShowMentorMessage($"'{ClinicalStepInfo.Name(step)}' scored {score}/100. " + NextHint());
        }

        AdvancePhase();
    }

    private string NextHint()
    {
        if (CurrentStepIndex >= _correctOrder.Count) return "That's the last step!";
        return $"Next up: {ClinicalStepInfo.Name(_correctOrder[CurrentStepIndex])}.";
    }

    private void AdvancePhase()
    {
        if (CurrentStepIndex >= ClinicalStepInfo.TotalSteps)
        {
            EndShift();
        }
        else
        {
            CurrentPhase = Phase.PlayerTurn;
        }
    }

    private void EndShift()
    {
        CurrentPhase = Phase.ShiftComplete;
        var gm = GameManager.Instance;

        if (ui != null) ui.ShowOutcome(gm.Score);

        // Ask Dr. Olayinka for end-of-shift feedback.
        if (aiFeedback != null)
        {
            aiFeedback.RequestFeedback(feedback =>
            {
                if (ui != null) ui.ShowMentorMessage(feedback);
            });
        }

        onShiftComplete?.Invoke();
    }
}
