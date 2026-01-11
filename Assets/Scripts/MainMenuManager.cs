using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tutorialOverlay;

    public void PlayGame()
    {
        SceneManager.LoadScene("Deck");
    }

    public void ShowTutorial()
    {
        tutorialOverlay.SetActive(true);
    }

    public void HideTutorial()
    {
        tutorialOverlay.SetActive(false);
    }
}
