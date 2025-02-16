using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RaceManager : MonoBehaviour
{
    private bool player1Finished = false;
    private bool player2Finished = false;
    private float player1Time = 0f;
    private float player2Time = 0f;

    // Referenzen zu den Canvases
    public GameObject raceCanvas;  // Das Canvas während des Rennens
    public GameObject resultCanvas;  // Das Canvas für die Ergebnisse
    public GameObject pauseCanvas;   // Das Canvas für das Pausenmenü

    // Referenzen zu den UI-Elementen im ResultCanvas
    public Text player1TimeText;
    public Text player2TimeText;
    public Text winnerText;

    // Buttons für Pause-Menu und Ergebnisse
    public Button resumeButton;
    public Button mainMenuButton;
    
    public Button mainMenuButton2;
    public Button restartButton;

    public bool isPaused = false;  // Status, ob das Spiel pausiert ist oder nicht
    
    private bool singleplayer = SingleplayerVarHolder.singleplayer;

    void Start()
    {
        // Zu Beginn ist nur das Renn-Canvas sichtbar
        raceCanvas.SetActive(true);
        resultCanvas.SetActive(false);
        pauseCanvas.SetActive(false);

        // Button-Events zuweisen
        resumeButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
        mainMenuButton2.onClick.AddListener(GoToMainMenu);
        restartButton.onClick.AddListener(RestartRace);
    }

    void Update()
    {
        // Wenn ESC gedrückt wird, Pausenmenü anzeigen oder schließen
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();  // Fortsetzen
            }
            else
            {
                PauseGame();  // Pause
            }
        }
    }

    public void PlayerFinished(int playerNumber, float time)
    {
        if (playerNumber == 1)
        {
            player1Finished = true;
            player1Time = time;
        }
        else if (playerNumber == 2)
        {
            player2Finished = true;
            player2Time = time;
        }

        // Überprüfen, ob beide Spieler fertig sind
        if (player1Finished && player2Finished)
        {
            ShowResults();
        }
    }

    void ShowResults()
    {
        // RaceCanvas ausblenden und ResultCanvas anzeigen
        raceCanvas.SetActive(false);
        resultCanvas.SetActive(true);

        // Zeige die Zeiten an
        if (!singleplayer)
        {
            player1TimeText.text = "Player 1 Time: " + player1Time.ToString("F2") + "s";
            player2TimeText.text = "Player 2 Time: " + player2Time.ToString("F2") + "s";
        }
        else
        {
            player1TimeText.text = "Bot Time: " + player1Time.ToString("F2") + "s";
            player2TimeText.text = "Your Time: " + player2Time.ToString("F2") + "s";
        }

        // Bestimme den Gewinner
        if (player1Time < player2Time)
        {
            if (!singleplayer)
            {
                winnerText.text = "Player 1 Wins!";
            }
            else
            {
                winnerText.text = "You Lose!";
            }
        }
        else if (player2Time < player1Time)
        {
            if (!singleplayer)
            {
                winnerText.text = "Player 2 Wins!";
            }
            else
            {
                winnerText.text = "You Win!";
            }
        }
        else
        {
            winnerText.text = "It's a Tie!";
        }
    }

    // Funktion zum Neustarten des Rennens
    public void RestartRace()
    {
        // Lade die aktuelle Szene neu, um das Rennen zu wiederholen
        Time.timeScale = 1f;  // Stelle sicher, dass das Spiel nicht pausiert ist
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Funktion zum Hauptmenü
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;  // Spielzeit normalisieren bevor das Hauptmenü geladen wird
        SceneManager.LoadScene("Menü");  // Hauptmenü-Szene laden
    }

    // Funktion zum Pausieren des Spiels
    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;  // Spielzeit anhalten
        pauseCanvas.SetActive(true);  // Pause-Menü anzeigen
    }

    // Funktion zum Fortsetzen des Spiels
    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;  // Spielzeit fortsetzen
        pauseCanvas.SetActive(false);  // Pause-Menü ausblenden
    }
}
