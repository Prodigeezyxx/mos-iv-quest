using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Put one of these on each of the 3 vein-choice buttons inside veinPanel.
/// Set a 'quality' value (0-100) per vein, and which is the best/right pick.
/// On click it reports the quality to the MiniGameController.
/// </summary>
[RequireComponent(typeof(Button))]
public class VeinOption : MonoBehaviour
{
    [Range(0, 100)] public int quality = 50;
    public MiniGameController miniGames;

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (miniGames != null) miniGames.SelectVein(quality);
        });
    }
}
