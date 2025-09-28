using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject profilePanel;

    // Show profile panel, hide main menu
    public void ShowProfile()
{
    mainMenuPanel.SetActive(false);
    profilePanel.SetActive(true); // must be active first
}


    // Go back to main menu
    public void ShowMainMenu()
    {
        profilePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
