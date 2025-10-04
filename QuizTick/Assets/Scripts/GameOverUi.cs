using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI scoreText; 

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

        
public void ShowGameOver(int score, int totalQuestions)
{
    gameOverPanel.SetActive(true); 

    scoreText.text = $"{score} / {totalQuestions}";
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
    
    UnityEngine.SceneManagement.SceneManager.LoadScene(
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
    );
}

}

