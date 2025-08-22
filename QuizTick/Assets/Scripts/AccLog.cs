using UnityEngine;

public class AccLog : MonoBehaviour
{

    void Start()
    {
        MusicManager.Instance.PlayMusic("Start");
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
