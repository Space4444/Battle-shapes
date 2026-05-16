using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour {

    public AudioSource musicSource, soundSource;
    public GameObject settings, credits;
    public Toggle musicToggle, soundsToggle;
    public Slider musicSlider, soundsSlider;
    public LevelLoader levelLoader;
    public Button cont;
    private void Start()
    {
        Time.timeScale = 1f;
        if (PlayerPrefs.HasKey("music"))
            musicToggle.isOn = musicSource.enabled = PlayerPrefs.GetString("music") == "1";
        if (PlayerPrefs.HasKey("sounds"))
            soundsToggle.isOn = soundSource.enabled = PlayerPrefs.GetString("sounds") == "1";
        if (PlayerPrefs.HasKey("musicVol"))
            musicSlider.value = musicSource.volume = PlayerPrefs.GetFloat("musicVol");
        if (PlayerPrefs.HasKey("soundsVol"))
            soundsSlider.value = soundSource.volume = PlayerPrefs.GetFloat("soundsVol");
        cont.interactable = PlayerPrefs.HasKey("itemCounts0");
    }

    public void NewGame()
    {
        if(soundSource.enabled)
            soundSource.Play();
        if (cont.interactable)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString("music", musicSource.enabled ? "1" : "0");
            PlayerPrefs.SetString("sounds", soundSource.enabled ? "1" : "0");
            PlayerPrefs.SetFloat("musicVol", musicSource.volume);
            PlayerPrefs.SetFloat("soundsVol", soundSource.volume);
        }
        levelLoader.LoadLevel(1);
    }

    public void Continue()
    {
        if (soundSource.enabled)
            soundSource.Play();
        levelLoader.LoadLevel(1);
    }

    public void Options()
    {
        if (soundSource.enabled)
            soundSource.Play();
        settings.SetActive(true);
    }

    public void Credits()
    {
        if (soundSource.enabled)
            soundSource.Play();
        credits.SetActive(true);
    }

    public void OnOffMusic(bool value)
    {
        if (soundSource.enabled)
            soundSource.Play();
        musicSource.enabled = value;
        PlayerPrefs.SetString("music", value ? "1" : "0");
    }

    public void OnOffSounds(bool value)
    {
        soundSource.enabled = value;
        if (soundSource.enabled)
            soundSource.Play();
        PlayerPrefs.SetString("sounds", value ? "1" : "0");
    }

    public void SetMusicVol(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("musicVol", value);
    }

    public void SetSoundsVol(float value)
    {
        soundSource.volume = value;
        PlayerPrefs.SetFloat("soundsVol", value);
    }

    public void CloseSettings()
    {
        if (soundSource.enabled)
            soundSource.Play();
        settings.SetActive(false);
    }

    public void CloseCredits()
    {
        if (soundSource.enabled)
            soundSource.Play();
        credits.SetActive(false);
    }
}