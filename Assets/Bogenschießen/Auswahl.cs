using UnityEngine;
using UnityEngine.SceneManagement;

public class ModeSelector : MonoBehaviour {
    // Wird vom "Schwer"-Button aufgerufen
    public void SetHeavyMode() {
        PlayerPrefs.SetFloat("breathAmplitudeIncreaseRate", 0.6f);
        PlayerPrefs.SetFloat("breathSpeed", 5f);
        PlayerPrefs.SetFloat("mouseSensitivity", 350f);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Bogenschießen"); // Name der Szene mit der Kamera
    }

    public void SetMiddleMode() {
        PlayerPrefs.SetFloat("breathAmplitudeIncreaseRate", 0.4f);
        PlayerPrefs.SetFloat("breathSpeed", 2.5f);
        PlayerPrefs.SetFloat("mouseSensitivity", 200f);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Bogenschießen"); // Name der Szene mit der Kamera
    }
    // Wird vom "Leicht"-Button aufgerufen
    public void SetLightMode() {
        PlayerPrefs.SetFloat("breathAmplitudeIncreaseRate", 0.2f);
        PlayerPrefs.SetFloat("breathSpeed", 1.0f);
        PlayerPrefs.SetFloat("mouseSensitivity", 100f);
        PlayerPrefs.Save();
        SceneManager.LoadScene("Bogenschießen"); // Name der Szene mit der Kamera
    }
}
