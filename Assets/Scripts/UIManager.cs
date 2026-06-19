using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// All on-screen HUD: score, Dr. Olayinka's speech bubble, and the
/// end-of-shift patient outcome panel.
/// Drag the matching UI objects into these fields in the Inspector.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI scoreText;

    [Header("Dr. Olayinka speech bubble")]
    public GameObject speechBubble;
    public TextMeshProUGUI speechText;
    public Image mentorPortrait;     // optional: assign Dr. Olayinka sprite
    public string mentorName = "Dr. Olayinka";

    [Header("Outcome screen")]
    public GameObject outcomePanel;
    public TextMeshProUGUI outcomeText;
    public Image outcomeFace;
    public Sprite faceHappy;         // thumbs up
    public Sprite faceNeutral;
    public Sprite faceHurt;          // 🤕

    [Header("Outcome thresholds (% of max score)")]
    [Range(0, 1)] public float happyThreshold = 0.8f;
    [Range(0, 1)] public float neutralThreshold = 0.5f;

    private void Start()
    {
        if (outcomePanel != null) outcomePanel.SetActive(false);
    }

    public void SetScore(int score)
    {
        if (scoreText != null) scoreText.text = $"Score: {score}";
    }

    public void ShowMentorMessage(string message)
    {
        if (speechBubble != null)
        {
            speechBubble.SetActive(true);
            if (UITweener.Instance != null)
                UITweener.Instance.Punch(speechBubble, 0.05f, 0.2f);
        }
        if (speechText != null) speechText.text = $"{mentorName}: {message}";
    }

    public void ShowOutcome(int score)
    {
        int max = ClinicalStepInfo.TotalSteps * GameManager.CorrectStepPoints;
        float pct = max > 0 ? (float)score / max : 0f;

        string verdict;
        Sprite face;
        if (pct >= happyThreshold) { verdict = "The patient gives MO a thumbs up!"; face = faceHappy; }
        else if (pct >= neutralThreshold) { verdict = "The IV is in, but it was a bumpy ride."; face = faceNeutral; }
        else { verdict = "Ouch. The patient had a rough time tonight."; face = faceHurt; }

        if (outcomePanel != null)
        {
            if (UITweener.Instance != null)
                UITweener.Instance.ScaleIn(outcomePanel, 0.4f);
            else
                outcomePanel.SetActive(true);
        }
        if (outcomeText != null) outcomeText.text = $"Shift complete!\nScore: {score}/{max}\n{verdict}";
        if (outcomeFace != null && face != null) outcomeFace.sprite = face;
    }
}
