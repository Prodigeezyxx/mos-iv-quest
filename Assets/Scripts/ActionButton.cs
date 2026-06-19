using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace MoIVQuest
{
    /// <summary>
    /// Attach to each of the 12 UI action buttons.
    /// 1. Pick which ClinicalStep this button represents in the Inspector.
    /// 2. Drag your TurnManager into the field.
    /// The label auto-fills from the step name on start.
    /// </summary>
    [RequireComponent(typeof(Button))]
    public class ActionButton : MonoBehaviour
    {
        [Header("Config")]
        public ClinicalStep step;
        public TurnManager turnManager;

        [Tooltip("Optional: a UI Text on the button to auto-label.")]
        public TextMeshProUGUI label;

        private void Start()
        {
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(OnClicked);

            if (label != null)
                label.text = ClinicalStepInfo.Name(step);
        }

        private void OnClicked()
        {
            if (turnManager != null)
                turnManager.PerformStep(step);
            else
                Debug.LogWarning($"ActionButton '{name}' has no TurnManager assigned.");
        }
    }
}
