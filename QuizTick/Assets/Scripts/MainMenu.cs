using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    private void Start()
    {
      MusicManager.Instance.PlayMusic("MainMenu");
    }

    public void ButtonSound()
    {
        SoundManager.Instance.PlaySound("ButtonSound");
    }
   
   public void BackSound()
   {
        SoundManager.Instance.PlaySound("BackSound");
   }

    public void PlayGame()
   {
      SceneManager.LoadSceneAsync("Category");
   }

   public void QuitGame()
   {
      Application.Quit();
   }
}
