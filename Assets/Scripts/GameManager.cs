using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game References")]
    public Transform destinationTarget;

    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverReasonText;
    public GameObject hintsRoot;

    [Header("Winner UI")]
    public GameObject winnerPanel;

    [Header("Score UI")]
    public Slider distanceSlider;
    public TextMeshProUGUI distanceText;

    [Header("Settings")]
    public float maxScoreDistance = 1000f;
    [Tooltip("1 = Linear. 2 = Quadratic.")]
    public float progressCurveExponent = 2.0f;
    [Tooltip("Distance at which the game is won.")]
    public float winDistanceThreshold = 15f;

    bool gameEnded = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (!gameEnded)
            CalculateDistanceAndCheckWin();
    }

    void CalculateDistanceAndCheckWin()
    {
        float currentDistance = 0f;

        if (destinationTarget != null)
            currentDistance = destinationTarget.position.magnitude;

        if (distanceText != null)
            distanceText.text = $"Distance to Port: {currentDistance:F0}m";

        if (distanceSlider != null)
        {
            float fraction = 1f - Mathf.Clamp01(currentDistance / maxScoreDistance);
            float curvedValue = Mathf.Pow(fraction, progressCurveExponent);
            distanceSlider.value = curvedValue;
        }

        if (currentDistance <= winDistanceThreshold)
            GameWon();
    }

    public void GameWon()
    {
        if (gameEnded) return;
        gameEnded = true;

        // hide ui
        if (hintsRoot != null)
            hintsRoot.SetActive(false);

        // sound
        SimpleAudio.Instance.PlayWin();

        if (winnerPanel != null)
        {
            winnerPanel.SetActive(true);
            Time.timeScale = 0f;
        }

        Debug.Log("GAME WON: Port Reached!");
    }

    public void GameOver(string reason)
    {
        if (gameEnded) return;
        gameEnded = true;

        // hide ui
        if (hintsRoot != null)
            hintsRoot.SetActive(false);

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (gameOverReasonText != null)
                gameOverReasonText.text = reason;

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
