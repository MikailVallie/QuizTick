using UnityEngine;

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
}
