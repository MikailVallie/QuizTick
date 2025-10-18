using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject profilePanel;
    public void OnSignOutButton()
    {
        GameSession.SignOut();

        SceneManager.LoadScene(0); 
    }

    public void ShowProfile()
    {
        mainMenuPanel.SetActive(true);
        profilePanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        profilePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void ShowLeaderboard()
    {
        SceneManager.LoadScene("Leaderboard");
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
