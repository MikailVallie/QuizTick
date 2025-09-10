using UnityEngine;
using UnityEngine.Audio;

public class AccLog : MonoBehaviour
{

    public AudioMixer audioMixer;
    

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
