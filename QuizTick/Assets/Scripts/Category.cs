using UnityEngine;
using UnityEngine.SceneManagement;

public class Category : MonoBehaviour
{

 public void BackToMenu()
   {
      SceneManager.LoadSceneAsync("MainMenu");
   }
    void Start()
    {

    }

    void Update()
    {

    }

    public void ButtonSound()
    {
        SoundManager.Instance.PlaySound("ButtonSound");
    }

    public void BackSound()
    {
        SoundManager.Instance.PlaySound("BackSound");
    }
   
   
}
