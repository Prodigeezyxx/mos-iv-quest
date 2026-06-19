using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the guided game flow: highlights the correct next button,
/// greys out wrong ones, shows progress bar, and displays step instructions.
/// This is what makes the game NOT confusing.
/// </summary>
public class GameFlowGuide : MonoBehaviour
{
    [Header("Progress bar")]
    public Image progressFill;
    public TMP_Text progressText;

    [Header("Instruction banner")]
    public GameObject instructionBanner;
    public TMP_Text instructionText;

    [Header("Step list (optional)")]
    public Transform stepListContainer;
    public GameObject stepListItemPrefab;

    [Header("Colors")]
    public Color activeColor = new Color(0.2f, 0.8f, 0.4f);    // green = do this
    public Color completedColor = new Color(0.4f, 0.6f, 0.8f);  // blue = done
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 0.5f); // grey = locked
    public Color wrongColor = new Color(0.9f, 0.3f, 0.3f);      // red = wrong

    private Dictionary<ClinicalStep, Button> _stepButtons = new Dictionary<ClinicalStep, Button>();
    private Dictionary<ClinicalStep, Image> _stepButtonImages = new Dictionary<ClinicalStep, Image>();
    private Dictionary<ClinicalStep, TMP_Text> _stepButtonTexts = new Dictionary<ClinicalStep, TMP_Text>();
    private List<GameObject> _stepListItems = new List<GameObject>();
    private ClinicalStep? _currentExpected;

    private static readonly string[] StepInstructions = {
        "Walk to the patient. Introduce yourself and explain the procedure. Get their consent first!",
        "Wash your hands thoroughly. This is the #1 skipped step in real life!",
        "Gather all equipment: tourniquet, cannula, swab, dressing, flush, gloves, sharps bin.",
        "Apply the tourniquet 5-7cm above the insertion site. Check for a pulse below.",
        "Select the best vein. Look for visible, palpable veins. Avoid areas near joints.",
        "Don your gloves now that you've selected the vein. Sterile technique begins!",
        "Clean the site with an alcohol swab. Wait 30 seconds for it to dry completely.",
        "Insert the cannula! Watch the timing bar — stop it in the green zone for a clean insertion.",
        "Check for flashback in the cannula chamber. This confirms you're in the vein!",
        "Advance the cannula slightly, retract the needle, and release the tourniquet.",
        "Secure the cannula with a dressing and flush with saline to confirm patency.",
        "Dispose of the sharps safely. Drag the needle into the yellow sharps bin!"
    };

    public void RegisterButton(ClinicalStep step, Button btn)
    {
        _stepButtons[step] = btn;
        _stepButtonImages[step] = btn.GetComponent<Image>();
        var tmp = btn.GetComponentInChildren<TMP_Text>();
        if (tmp != null) _stepButtonTexts[step] = tmp;
    }

    public void SetCurrentExpected(ClinicalStep step, int stepIndex)
    {
        _currentExpected = step;

        // Update all button states
        foreach (var kvp in _stepButtons)
        {
            ClinicalStep s = kvp.Key;
            Button btn = kvp.Value;
            Image img = _stepButtonImages[s];
            TMP_Text txt = _stepButtonTexts.ContainsKey(s) ? _stepButtonTexts[s] : null;

            if (s == step)
            {
                // Active / highlighted
                if (img != null) img.color = activeColor;
                if (txt != null) txt.color = Color.white;
                btn.interactable = true;
            }
            else if (System.Array.IndexOf(ClinicalStepInfo.CorrectOrder().ToArray(), s) < stepIndex)
            {
                // Completed
                if (img != null) img.color = completedColor;
                if (txt != null) txt.color = new Color(0.7f, 0.7f, 0.7f);
                btn.interactable = true; // can still click but won't advance
            }
            else
            {
                // Locked / future
                if (img != null) img.color = lockedColor;
                if (txt != null) txt.color = new Color(0.5f, 0.5f, 0.5f);
                btn.interactable = true; // can click but will get wrong feedback
            }
        }

        // Update progress bar
        float progress = (float)stepIndex / ClinicalStepInfo.TotalSteps;
        if (progressFill != null) progressFill.fillAmount = progress;
        if (progressText != null)
            progressText.text = $"Step {stepIndex + 1} of {ClinicalStepInfo.TotalSteps}";

        // Update instruction banner
        if (instructionBanner != null && instructionText != null)
        {
            instructionText.text = StepInstructions[(int)step];
            if (UITweener.Instance != null)
            {
                UITweener.Instance.ScaleIn(instructionBanner, 0.25f);
            }
            else
            {
                instructionBanner.SetActive(true);
            }
        }

        // Update step list
        UpdateStepList(stepIndex);
    }

    public void FlashWrong(ClinicalStep clickedStep)
    {
        if (!_stepButtonImages.TryGetValue(clickedStep, out var img)) return;
        if (UITweener.Instance != null)
            UITweener.Instance.FlashColor(img, wrongColor, 0.4f);
        if (UITweener.Instance != null && _stepButtons.TryGetValue(clickedStep, out var btn))
            UITweener.Instance.Punch(btn.gameObject, 0.1f, 0.2f);
    }

    public void FlashCorrect(ClinicalStep clickedStep)
    {
        if (!_stepButtonImages.TryGetValue(clickedStep, out var img)) return;
        if (UITweener.Instance != null && _stepButtons.TryGetValue(clickedStep, out var btn))
            UITweener.Instance.Punch(btn.gameObject, 0.2f, 0.25f);
    }

    public void BuildStepList()
    {
        if (stepListContainer == null) return;

        // Clear existing
        foreach (var item in _stepListItems)
        {
            if (item != null) DestroyImmediate(item);
        }
        _stepListItems.Clear();

        var order = ClinicalStepInfo.CorrectOrder();
        for (int i = 0; i < order.Count; i++)
        {
            GameObject item;
            if (stepListItemPrefab != null)
                item = Instantiate(stepListItemPrefab, stepListContainer);
            else
            {
                item = new GameObject($"Step_{i}", typeof(RectTransform));
                item.transform.SetParent(stepListContainer, false);
                var img = item.AddComponent<Image>();
                img.color = new Color(0, 0, 0, 0.3f);
                var tmp = item.AddComponent<TextMeshProUGUI>();
                tmp.text = $"{i + 1}. {ClinicalStepInfo.Name(order[i])}";
                tmp.fontSize = 16;
                tmp.color = new Color(0.7f, 0.7f, 0.7f);
                tmp.alignment = TextAlignmentOptions.Left;
                var rect = item.GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(250, 28);
            }
            _stepListItems.Add(item);
        }
    }

    void UpdateStepList(int currentIndex)
    {
        for (int i = 0; i < _stepListItems.Count; i++)
        {
            if (_stepListItems[i] == null) continue;
            var tmp = _stepListItems[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tmp == null) continue;

            if (i < currentIndex)
            {
                tmp.color = new Color(0.4f, 0.7f, 1f); // completed = blue
                tmp.text = $"✓ {i + 1}. {ClinicalStepInfo.Name(ClinicalStepInfo.CorrectOrder()[i])}";
            }
            else if (i == currentIndex)
            {
                tmp.color = new Color(0.2f, 0.9f, 0.4f); // current = green
                tmp.fontStyle = FontStyles.Bold;
                tmp.text = $"▶ {i + 1}. {ClinicalStepInfo.Name(ClinicalStepInfo.CorrectOrder()[i])}";
            }
            else
            {
                tmp.color = new Color(0.5f, 0.5f, 0.5f); // future = grey
                tmp.fontStyle = FontStyles.Normal;
                tmp.text = $"  {i + 1}. {ClinicalStepInfo.Name(ClinicalStepInfo.CorrectOrder()[i])}";
            }
        }
    }
}
