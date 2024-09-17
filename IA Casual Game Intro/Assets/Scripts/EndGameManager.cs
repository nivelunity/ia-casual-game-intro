using UnityEngine;
using TMPro;

public class EndGameManager : MonoBehaviour {
    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text scoreText;

    void OnEnable() {
        // Subscribe to the GameOver event when this script is enabled
        GameManager.OnGameOver += HandleGameOver;
    }

    void OnDisable() {
        // Unsubscribe from the GameOver event when this script is disabled
        GameManager.OnGameOver -= HandleGameOver;
    }

    /// <summary>
    /// Using this method to handle the GameOver event
    /// </summary>
    void HandleGameOver() {
        // Do something when the game is over
        Debug.Log("Game Over!");

        ShowGameOverPanel();
        scoreText.text = GameManager.playerScore.ToString();
    }

    /// <summary>
    /// Show the Game Over panel
    /// </summary>
    void ShowGameOverPanel() {
        if (gameOverPanel != null) {
            gameOverPanel.SetActive(true);
            // You can customize the panel further, update score, show relevant information, etc.
        }
    }
}
