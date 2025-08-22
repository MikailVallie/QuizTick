using UnityEngine;
<<<<<<< HEAD

public class Difficulty : MonoBehaviour
{

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
=======
using UnityEngine.SceneManagement;
public class JAVA : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        SceneManager.LoadSceneAsync("Diffulty Panel");
    }

    // Update is called once per frame
    public void Update()
    {
        Application.Quit();
    }
>>>>>>> 015791744bb9ffa8669bf28b12981db147a2b119
}
