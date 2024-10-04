using UnityEngine;
using UnityEngine.SceneManagement;  // Für den Szenenwechsel (zurück zum Hauptmenü)
using UnityEngine.UI;  // Für UI-Elemente wie Buttons

public class PauseMenu : MonoBehaviour
{
    public Canvas pauseCanvas;   // Referenz auf das Pause-Canvas
    public Button resumeButton;  // Der Button für das Fortsetzen des Spiels
    public Button mainMenuButton;  // Der Button für das Hauptmenü

    private bool isPaused = false;  // Verfolgt, ob das Spiel pausiert ist

    void Start()
    {
        // Verknüpfe die Button-Events mit den Methoden
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);

        // Stelle sicher, dass das Pause-Canvas zu Beginn nicht sichtbar ist
        pauseCanvas.gameObject.SetActive(false);
    }

    void Update()
    {
        // Wenn die ESC-Taste gedrückt wird, Pause/Resume umschalten
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    // Methode, um das Spiel zu pausieren
    public void PauseGame()
    {
        pauseCanvas.gameObject.SetActive(true);  // Pause-Canvas anzeigen
        Time.timeScale = 0f;  // Das Spiel anhalten
        isPaused = true;  // Spielstatus auf "pausiert" setzen
    }

    // Methode, um das Spiel fortzusetzen
    public void ResumeGame()
    {
        pauseCanvas.gameObject.SetActive(false);  // Pause-Canvas ausblenden
        Time.timeScale = 1f;  // Das Spiel fortsetzen (normaler Zeitablauf)
        isPaused = false;  // Spielstatus auf "nicht pausiert" setzen
    }

    // Methode, um ins Hauptmenü zurückzukehren (Szene wechseln)
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;  // Spielzeit normalisieren, bevor die Szene wechselt
        SceneManager.LoadScene("Menü");  // Ersetze "MainMenu" mit dem Namen deiner Hauptmenü-Szene
    }
}
