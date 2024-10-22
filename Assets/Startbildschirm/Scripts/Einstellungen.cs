using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // Für die Soundsteuerung

public class SettingsMenu : MonoBehaviour
{
    public Canvas settingsCanvas;      // Das Canvas für die Einstellungen

     [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;

   
    public void SetMusicVolume()
        {
            float volume = musicSlider.value;
            myMixer.SetFloat("music", Mathf.Log10(volume)*20);
            PlayerPrefs.SetFloat("musicVolume",volume);
        }
    
    
    public void QuitGame()
    {
        Debug.Log("Spiel wird beendet...");
        Application.Quit(); // Beendet das Spiel (funktioniert nur im Build)
    }
}
