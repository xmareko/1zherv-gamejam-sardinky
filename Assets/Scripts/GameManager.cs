using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject gameOverPanel; // Assign your Game Over UI panel here

    void Awake()
    {
        // Simple Singleton pattern
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            // Optional: Stop time so the ship stops sinking
            Time.timeScale = 0f;
        }
        Debug.Log("GAME OVER");
    }

    // Connect this to the "Return to Main Menu" button on your Game Over screen
    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Reset time before leaving
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}