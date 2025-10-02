using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //To open the Help Scene
    public void LoadHelpScene()
    {
        SceneManager.LoadScene("Help"); 
    }

    //To go back to Main Menu
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}

