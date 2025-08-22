using UnityEngine;

public class Difficulty : MonoBehaviour
{

    public void Start()
    {

    }


    public void Update()
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
