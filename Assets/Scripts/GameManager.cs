using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Add this namespace for TextMeshPro

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverReasonText; // Drag your new "ReasonText" here in Inspector

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // UPDATED: Now requires a reason string
    public void GameOver(string reason)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // Set the reason text if assigned
            if (gameOverReasonText != null)
            {
                gameOverReasonText.text = reason;
            }

            Time.timeScale = 0f;
        }
        Debug.Log($"GAME OVER: {reason}");
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}