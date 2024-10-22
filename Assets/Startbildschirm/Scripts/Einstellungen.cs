using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio; // Für die Soundsteuerung

public class SettingsMenu : MonoBehaviour
{
    public Canvas settingsCanvas;      // Das Canvas für die Einstellungen
    public Slider volumeSlider;        // Der Slider für die Lautstärke
    public AudioMixer audioMixer;      // Der Audio-Mixer, der die Lautstärke steuert

    void Start()
    {
        // Stelle sicher, dass das Canvas für die Einstellungen am Start deaktiviert ist
        settingsCanvas.gameObject.SetActive(true);

        // Lade die gespeicherte Lautstärke (falls vorhanden) und setze sie im AudioMixer
        float savedVolume = PlayerPrefs.GetFloat("volume", 0.75f); // Standard-Lautstärke auf 75%
        SetVolume(savedVolume);
        volumeSlider.value = savedVolume;

        // Verknüpfe den Slider mit der SetVolume-Methode
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    // Methode zum Einstellen der Lautstärke
    public void SetVolume(float volume)
    {
        // Setze die Lautstärke im AudioMixer (erwartet Dezibel-Werte)
        audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20); // Lautstärke in Dezibel umwandeln

        // Speichere die Lautstärke für zukünftige Sitzungen
        PlayerPrefs.SetFloat("volume", volume);
    }

    // Methode zum Anzeigen der Einstellungen
    public void ShowSettings()
    {
        settingsCanvas.gameObject.SetActive(true);
        Time.timeScale = 0f; // Spiel pausieren, wenn das Einstellungsmenü geöffnet ist
    }

    // Methode zum Ausblenden der Einstellungen
    public void HideSettings()
    {
        settingsCanvas.gameObject.SetActive(false);
        Time.timeScale = 1f; // Spiel fortsetzen, wenn das Einstellungsmenü geschlossen wird
    }

    // Methode zum Beenden des Spiels
    public void QuitGame()
    {
        Debug.Log("Spiel wird beendet...");
        Application.Quit(); // Beendet das Spiel (funktioniert nur im Build)
    }
}
