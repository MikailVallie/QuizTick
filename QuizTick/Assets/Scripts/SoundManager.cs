using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
 
    [SerializeField]
    private SoundLibrary sfxLibrary;
    [SerializeField]
    private AudioSource SFXSource;
 
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
 
    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
        {
            AudioSource.PlayClipAtPoint(clip, pos);
        }
    }
 
    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(sfxLibrary.GetClipFromName(soundName), pos);
    }
 
    public void PlaySound(string soundName)
    {
        SFXSource.PlayOneShot(sfxLibrary.GetClipFromName(soundName));
    }
}