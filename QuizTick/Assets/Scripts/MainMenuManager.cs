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
        mainMenuPanel.SetActive(false);
        profilePanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        profilePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
