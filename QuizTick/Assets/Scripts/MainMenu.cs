using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

   public AudioMixer audioMixer;
   public Slider MusicSlider;
   public Slider SFXSlider;

   private void Start()
   {
      
      MusicManager.Instance.PlayMusic("MainMenu");
      LoadVolume();
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

   public void UpdateMusicVolume(float volume)
   {
      audioMixer.SetFloat("MusicVolume", volume);
   }

   public void UpdateSoundVolume(float volume)
   {
      audioMixer.SetFloat("SFXVolume", volume);
   }
    
    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
 
        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
 
    public void LoadVolume()
    {
        MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }

}
