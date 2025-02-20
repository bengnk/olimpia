using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    public Canvas settingsCanvas;

    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    private const float minVolumeDb = -80f;
    [SerializeField] private float maxVolumeDb = 6f; // gewünschtes Maximum

    // Wähle einen Exponenten, um den Kurvenverlauf zu ändern.
    // Ein Wert < 1 hebt niedrigere Werte an, ein Wert > 1 senkt sie.
    [SerializeField] private float curveExponent = 0.2f; 

    void Start()
    {
        // Standardwert 1 (volle Lautstärke)
        float savedVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        musicSlider.value = savedVolume;
        SetMusicVolume();
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        float dB;
        if(volume <= 0.0001f)
        {
            dB = minVolumeDb;
        }
        else
        {
            // Anwenden einer nicht-linearen Kurve: 
            // Dadurch wird bei kleinen Änderungen des Sliderwertes die Lautstärke weniger drastisch gesenkt.
            float t = Mathf.Pow(volume, curveExponent);
            dB = Mathf.Lerp(minVolumeDb, maxVolumeDb, t);
        }
        myMixer.SetFloat("music", dB);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }

    public void QuitGame()
    {
        Debug.Log("Spiel wird beendet...");
        Application.Quit();
    }
}
