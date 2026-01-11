using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI; // Required for Slider

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game References")]
    public Transform destinationTarget; // Drag your "DestinationPort" object here

    [Header("UI References")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverReasonText;

    [Header("Score UI")]
    public Slider distanceSlider;       // Drag your UI Slider here
    public TextMeshProUGUI distanceText; // Drag text for "500m left"

    [Header("Score Settings")]
    public float maxScoreDistance = 1000f; // Distances above this show 0% progress
    [Tooltip("1 = Linear. 2 = Quadratic (Bar fills slowly at first, fast at the end).")]
    public float progressCurveExponent = 2.0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void GameOver(string reason)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            // 1. Show Reason
            if (gameOverReasonText != null)
                gameOverReasonText.text = reason;

            // 2. Calculate Distance Score
            CalculateDistanceScore();

            Time.timeScale = 0f;
        }
        Debug.Log($"GAME OVER: {reason}");
    }

    void CalculateDistanceScore()
    {
        float currentDistance = 0f;

        // Ship is always at (0,0,0), so distance is just the magnitude of destination position
        if (destinationTarget != null)
        {
            currentDistance = destinationTarget.position.magnitude;
        }

        // --- UPDATE TEXT ---
        if (distanceText != null)
        {
            // Format to 0 decimal places (e.g., "Distance: 450m")
            distanceText.text = $"Distance to Port: {currentDistance:F0}m";
        }

        // --- UPDATE SLIDER (Non-Linear) ---
        if (distanceSlider != null)
        {
            // 1. Calculate linear fraction (0 = at max distance, 1 = at destination)
            float fraction = 1f - Mathf.Clamp01(currentDistance / maxScoreDistance);

            // 2. Apply "Clever" Curve
            // Raising to power of 2 makes the bar fill slower initially, emphasizing the final stretch.
            float curvedValue = Mathf.Pow(fraction, progressCurveExponent);

            distanceSlider.value = curvedValue;
        }
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