using System.Collections.Generic;
using UnityEngine;

namespace MoIVQuest
{
    /// <summary>
    /// Single source of truth for the current shift: score, what MO did,
    /// what they skipped, errors, and mini-game results.
    /// Singleton so any script can reach it via GameManager.Instance.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Live session data (read-only at runtime)")]
        public int Score;
        public List<ClinicalStep> PlayerSequence = new List<ClinicalStep>();
        public List<ClinicalStep> CompletedSteps = new List<ClinicalStep>();
        public List<string> Errors = new List<string>();

        [Header("Mini-game scores (0-100)")]
        public int VeinScore = -1;
        public int InsertionScore = -1;
        public int SharpsScore = -1;

        public const int CorrectStepPoints = 10;
        public const int WrongStepPenalty = 5;

        private void Awake()
        {
            // Standard singleton guard.
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void ResetSession()
        {
            Score = 0;
            PlayerSequence.Clear();
            CompletedSteps.Clear();
            Errors.Clear();
            VeinScore = -1;
            InsertionScore = -1;
            SharpsScore = -1;
        }

        public void AddScore(int amount)
        {
            Score = Mathf.Max(0, Score + amount);
        }

        public void RecordError(string message)
        {
            Errors.Add(message);
            Debug.Log("[Dr. Olayinka note] " + message);
        }

        /// <summary>Steps MO skipped = correct steps that never got completed.</summary>
        public List<ClinicalStep> GetSkippedSteps()
        {
            var skipped = new List<ClinicalStep>();
            foreach (var step in ClinicalStepInfo.CorrectOrder())
            {
                if (!CompletedSteps.Contains(step))
                    skipped.Add(step);
            }
            return skipped;
        }
    }
}
