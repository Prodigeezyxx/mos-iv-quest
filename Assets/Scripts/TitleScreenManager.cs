using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the title screen → gameplay → restart flow.
/// Put on a "TitleScreenManager" GameObject. Assign the title panel
/// (which has a "Start Shift" button that calls StartGame()).
/// </summary>
public class TitleScreenManager : MonoBehaviour
{
    [Header("Title screen")]
    public GameObject titlePanel;
    public TMP_Text titleText;
    public TMP_Text subtitleText;
    public Button startButton;

    [Header("Learning outcomes (shown on title)")]
    [TextArea(2, 4)]
    public string learningOutcomes =
        "After playing, you will be able to:\n" +
        "• Sequence the 12 steps of IV cannulation\n" +
        "• Identify suitable veins\n" +
        "• Recognise common procedural errors\n" +
        "• Recall flashback & flush roles\n" +
        "• Reflect via AI feedback";

    private void Start()
    {
        if (titlePanel != null) titlePanel.SetActive(true);
        if (titleText != null) titleText.text = "MO's IV Quest";
        if (subtitleText != null) subtitleText.text = "A 48-Hour Night Shift at St. Pixel General";
        if (startButton != null) startButton.onClick.AddListener(StartGame);
    }

    public void StartGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        if (titlePanel != null) titlePanel.SetActive(false);

        var tm = FindAnyObjectByType<TurnManager>();
        if (tm != null) tm.BeginGame();
    }

    public void RestartGame()
    {
        if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
