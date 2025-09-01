using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText; // Text to show the score

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    // Add score parameters
    public void ShowGameOver(int score, int totalQuestions)
    {
        gameOverPanel.SetActive(true); // Makes the panel visible
        scoreText.text = " " + score + " / " + totalQuestions;
    }


    public void OnLeaderboardButton()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene(1);
    }
    public void OnRetryButton()
{
    // Option 1: Reload the current scene
    UnityEngine.SceneManagement.SceneManager.LoadScene(
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
    );
}

}

