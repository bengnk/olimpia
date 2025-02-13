using UnityEngine;
using UnityEngine.SceneManagement; // Falls du eine Szene laden willst

public class PauseManager : MonoBehaviour
{
    public GameObject pauseCanvas;  // Referenz zum Pause-Canvas
    public bool isPaused = false;  // Status des Spiels

    void Start()
    {
        // Stelle sicher, dass das Pause-Canvas zu Beginn deaktiviert ist
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);
        }
    }

    void Update()
    {
        // Überprüfe, ob die ESC-Taste gedrückt wurde
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();  // Spiel fortsetzen, wenn bereits pausiert
            }
            else
            {
                PauseGame();  // Spiel pausieren
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;  // Spiel pausieren (Zeit einfrieren)
        // **Cursor sichtbar und freigegeben machen**
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(true);  // Pause-Canvas anzeigen
        }
    }

    public void ResumeGame()
    {
        if (SceneManager.GetActiveScene().name == "Bogenschießen")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        isPaused = false;
        Time.timeScale = 1f;  // Spiel fortsetzen (Zeit normal weiterlaufen lassen)
        if (pauseCanvas != null)
        {
            pauseCanvas.SetActive(false);  // Pause-Canvas ausblenden
        }
    }

    public void QuitToMainMenu()
    {
        // Hier könntest du den Hauptmenü-Screen laden, z. B.:
        SceneManager.LoadScene("Menü");  // Lade die Hauptmenü-Szene
        Time.timeScale = 1f;
        Debug.Log("Zurück zum Hauptmenü");
    }
}
