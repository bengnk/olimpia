using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    public GameObject startCanvas; // Das Start-Canvas, das angezeigt werden soll

    private bool gameStarted = false; // Überprüft, ob das Spiel bereits gestartet wurde

    void Start()
    {
        // Start-Canvas anzeigen und das Spiel pausieren
        startCanvas.SetActive(true);
        Time.timeScale = 0; // Das Spiel ist pausiert, bis die Leertaste gedrückt wird

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        if (!gameStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1; // Spielgeschwindigkeit wieder auf normal setzen
        startCanvas.SetActive(false); // Start-Canvas ausblenden

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
