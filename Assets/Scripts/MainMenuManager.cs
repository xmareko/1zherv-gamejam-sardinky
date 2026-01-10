using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tutorialOverlay;

    // Call this from the "Play" button
    public void PlayGame()
    {
        // Replace "GameScene" with the exact name of your game scene file
        SceneManager.LoadScene("Deck");
    }

    // Call this from the "Show Tutorial" button
    public void ShowTutorial()
    {
        tutorialOverlay.SetActive(true);
    }

    // Call this from the giant invisible button inside the Tutorial Overlay
    public void HideTutorial()
    {
        tutorialOverlay.SetActive(false);
    }
}