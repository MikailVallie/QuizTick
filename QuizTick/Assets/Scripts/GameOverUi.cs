using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;

    void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }

    public void OnLeaderboardButton()
    {
        SceneManager.LoadScene("Leaderboard"); 
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene(1);
    }
}
