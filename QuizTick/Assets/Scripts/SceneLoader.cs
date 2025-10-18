using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    
    public void LoadHelpScene()
    {
        SceneManager.LoadScene("Help"); 
    }

    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}

