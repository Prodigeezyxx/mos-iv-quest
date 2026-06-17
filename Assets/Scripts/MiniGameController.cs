using System;
using UnityEngine;
using UnityEngine.UI;

namespace MoIVQuest
{
    /// <summary>
    /// Runs the three mini-games. TurnManager calls Launch(step, onComplete);
    /// when the mini-game finishes we invoke onComplete(step, score 0-100).
    ///
    /// Setup in Inspector:
    ///  - Assign the three panels (each is a UI GameObject you toggle on/off).
    ///  - VEIN: put 3 buttons inside veinPanel, each with a VeinOption component.
    ///  - INSERTION: assign the moving 'handle' RectTransform and the track width;
    ///    player presses SPACE (or call StopInsertion()) to lock it in.
    ///  - SHARPS: put a SharpsDraggable on the needle and set the bin RectTransform.
    /// </summary>
    public class MiniGameController : MonoBehaviour
    {
        [Header("Panels (toggle on/off)")]
        public GameObject veinPanel;
        public GameObject insertionPanel;
        public GameObject sharpsPanel;

        [Header("Insertion timing bar")]
        public RectTransform insertionHandle;   // the moving cursor
        public float trackHalfWidth = 200f;      // px from centre to each edge
        public float greenZoneHalfWidth = 40f;   // px; inside = high score
        public float insertionSpeed = 350f;       // px/sec

        private ClinicalStep _activeStep = ClinicalStep.None;
        private Action<ClinicalStep, int> _onComplete;

        private bool _insertionRunning;
        private float _insertionPos;       // -trackHalfWidth..+trackHalfWidth
        private int _insertionDir = 1;

        private void Awake()
        {
            HideAll();
        }

        public void Launch(ClinicalStep step, Action<ClinicalStep, int> onComplete)
        {
            _activeStep = step;
            _onComplete = onComplete;
            HideAll();

            switch (step)
            {
                case ClinicalStep.VeinSelection:
                    if (veinPanel != null) veinPanel.SetActive(true);
                    break;
                case ClinicalStep.InsertCannula:
                    if (insertionPanel != null) insertionPanel.SetActive(true);
                    StartInsertion();
                    break;
                case ClinicalStep.DisposeSharps:
                    if (sharpsPanel != null) sharpsPanel.SetActive(true);
                    break;
            }
        }

        // ---------- VEIN SELECTION ----------
        // Hook this to each vein option button via the VeinOption helper.
        public void SelectVein(int quality0to100)
        {
            Finish(ClinicalStep.VeinSelection, Mathf.Clamp(quality0to100, 0, 100));
        }

        // ---------- INSERTION TIMING ----------
        private void StartInsertion()
        {
            _insertionRunning = true;
            _insertionPos = -trackHalfWidth;
            _insertionDir = 1;
        }

        private void Update()
        {
            if (!_insertionRunning) return;

            _insertionPos += _insertionDir * insertionSpeed * Time.deltaTime;
            if (_insertionPos > trackHalfWidth) { _insertionPos = trackHalfWidth; _insertionDir = -1; }
            else if (_insertionPos < -trackHalfWidth) { _insertionPos = -trackHalfWidth; _insertionDir = 1; }

            if (insertionHandle != null)
                insertionHandle.anchoredPosition = new Vector2(_insertionPos, insertionHandle.anchoredPosition.y);

            // Legacy input: works with default Unity project input settings.
            if (Input.GetKeyDown(KeyCode.Space))
                StopInsertion();
        }

        /// <summary>Call from SPACE or a UI "Insert!" button.</summary>
        public void StopInsertion()
        {
            if (!_insertionRunning) return;
            _insertionRunning = false;

            float dist = Mathf.Abs(_insertionPos);
            int score;
            if (dist <= greenZoneHalfWidth) score = 100;
            else
            {
                float t = Mathf.InverseLerp(trackHalfWidth, greenZoneHalfWidth, dist); // edge=0 .. green=1
                score = Mathf.RoundToInt(Mathf.Clamp01(t) * 80f); // up to 80 outside green
            }
            Finish(ClinicalStep.InsertCannula, score);
        }

        // ---------- SHARPS DISPOSAL ----------
        // Called by SharpsDraggable: success=true if dropped on the bin.
        public void CompleteSharps(bool success)
        {
            Finish(ClinicalStep.DisposeSharps, success ? 100 : 30);
        }

        // ---------- shared ----------
        private void Finish(ClinicalStep step, int score)
        {
            if (_activeStep != step) return;
            HideAll();
            var cb = _onComplete;
            _activeStep = ClinicalStep.None;
            _onComplete = null;
            cb?.Invoke(step, score);
        }

        private void HideAll()
        {
            if (veinPanel != null) veinPanel.SetActive(false);
            if (insertionPanel != null) insertionPanel.SetActive(false);
            if (sharpsPanel != null) sharpsPanel.SetActive(false);
            _insertionRunning = false;
        }
    }
}
