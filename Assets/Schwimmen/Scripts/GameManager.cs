using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Für den Szenenwechsel

public class GameManager : MonoBehaviour
{
    public Text resultsText;  // UI-Text zum Anzeigen der Ergebnisse
    public Text countdownText;  // UI-Text zum Anzeigen des Countdowns oder der laufenden Zeit

    public Image timerBackground;

    private Dictionary<int, float> swimmerTimes = new Dictionary<int, float>();  // Speichert die Endzeiten der Schwimmer
    private Dictionary<int, float> swimmerStartTimes = new Dictionary<int, float>(); // Speichert die Startzeiten der Schwimmer
    public bool isGoTime = false; // Signalisiert, wann das Rennen startet
    public bool gameStarted = false; // Signalisiert, ob das Spiel gestartet wurde
    private bool raceOngoing = false; // Signalisiert, ob das Rennen läuft

    private float raceStartTime;  // Zeit, zu der das Rennen startet

    // Gesamtzahl der Schwimmer
    public int totalSwimmers = 4;  // Anzahl der Schwimmer (Anzahl kann im Inspector eingestellt werden)

    // Referenzen für das Start-, End- und Countdown-Canvas
    public GameObject startCanvas;  // Das Canvas, das zu Beginn angezeigt wird
    public GameObject endCanvas;    // Das Canvas, das nach dem Ende des Rennens angezeigt wird
    public GameObject countdownCanvas; // Das Canvas, das den Countdown enthält

    // Buttons für das End-Canvas
    public Button restartButton;  // Button für das Neustarten des Spiels
    public Button mainMenuButton;  // Button für das Zurückkehren ins Hauptmenü

    void Start()
    {
        timerBackground.color = new Color(0, 0, 0, 0);
        
        // Starte mit dem Start-Canvas und deaktiviere das End- und Countdown-Canvas
        if (startCanvas != null)
        {
            startCanvas.SetActive(true);  // Start-Canvas anzeigen
        }

        if (endCanvas != null)
        {
            endCanvas.SetActive(false);  // End-Canvas ausblenden
        }

        if (countdownCanvas != null)
        {
            countdownCanvas.SetActive(true);  // Countdown-Canvas anzeigen
        }

        // Spiel wird beim Start eingefroren (freeze)
        Time.timeScale = 0f;

        // Hinweis anzeigen: Drücke Leertaste zum Starten
        countdownText.text = "";

        // Buttons-Events zuweisen
        restartButton.onClick.AddListener(RestartRace);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void Update()
    {
        // Überprüfen, ob die Leertaste gedrückt wird und das Spiel noch nicht gestartet wurde
        if (Input.GetKeyDown(KeyCode.Space) && !gameStarted)
        {
            StartGame();
            timerBackground.color = new Color(0f, 0f, 0f, 0.5f);
        }

        // Wenn das Rennen läuft, aktualisiere die Rennzeit im Textfeld
        if (raceOngoing)
        {
            UpdateRaceTime();
        }
    }

    // Startet das Spiel und den Countdown
    private void StartGame()
    {
        gameStarted = true;

        // Spiel fortsetzen (freeze aufheben)
        Time.timeScale = 1f;

        // Start-Canvas ausblenden
        if (startCanvas != null)
        {
            startCanvas.SetActive(false);
        }

        // Countdown-Canvas bleibt eingeblendet, aber der Text wird auf den Countdown gesetzt
        if (countdownCanvas != null)
        {
            countdownCanvas.SetActive(true);
        }

        // Countdown zum Starten des Rennens beginnen
        StartCoroutine(StartRace());
    }

    // Coroutine für den Countdown und den Start des Rennens
    private IEnumerator StartRace()
    {
        int countdown = 3;  // Countdown-Zeit
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();  // Zeige die Countdown-Zahl an
            yield return new WaitForSeconds(1);  // Warte 1 Sekunde
            countdown--;
        }

        countdownText.text = "Go!";  // Zeige "Go!" an, wenn der Countdown endet
        isGoTime = true;  // Das Rennen beginnt

        // Warte 1 Sekunde, um "Go!" anzuzeigen
        yield return new WaitForSeconds(1);

        // Setze die Startzeit des Rennens
        raceStartTime = Time.time;

        // Das Rennen läuft jetzt
        raceOngoing = true;
    }

    // Aktualisiert die laufende Zeit im Countdown-Textfeld
    private void UpdateRaceTime()
    {
        float currentTime = Time.time - raceStartTime;  // Berechne die aktuelle Rennzeit
        countdownText.text = currentTime.ToString("F2") + "s";  // Zeige die Zeit im Textfeld an
    }

    // Methode, die von jedem Schwimmer aufgerufen wird, wenn er abspringt
    public void StartTimer(int swimmerID)
    {
        if (isGoTime && !swimmerStartTimes.ContainsKey(swimmerID))
        {
            swimmerStartTimes[swimmerID] = Time.time;  // Speichere die Startzeit des Schwimmers
            Debug.Log("Schwimmer " + swimmerID + " hat gestartet.");
        }
    }

    // Methode, die von jedem Schwimmer aufgerufen wird, wenn er das Ziel erreicht
    public void StopTimer(int swimmerID)
    {
        if (swimmerStartTimes.ContainsKey(swimmerID) && !swimmerTimes.ContainsKey(swimmerID))
        {
            float endTime = Time.time - swimmerStartTimes[swimmerID];  // Berechne die Endzeit
            swimmerTimes[swimmerID] = endTime;  // Speichere die Zeit
            DisplayResults();
            Debug.Log("Schwimmer " + swimmerID + " hat das Ziel in " + endTime + " Sekunden erreicht.");

            // Überprüfe, ob alle Schwimmer das Ziel erreicht haben
            if (swimmerTimes.Count == totalSwimmers)
            {
                // Wenn alle Schwimmer im Ziel sind, blende das End-Canvas ein
                ShowEndCanvas();
            }
        }
    }

    // Zeige die Ergebnisse an
    private void DisplayResults()
    {
        if (resultsText == null)
        {
            Debug.LogError("resultsText ist nicht zugewiesen! Bitte stelle sicher, dass das UI-Textfeld im Inspector gesetzt ist.");
            return;
        }

        resultsText.text = "";  // Setze den Text zurück
        foreach (var entry in swimmerTimes)
        {
            resultsText.text += entry.Value.ToString("F2") + " s";
        }
    }

    // Funktion zum Zeigen des End-Canvas
    private void ShowEndCanvas()
    {
        if (startCanvas != null)
        {
            startCanvas.SetActive(false);  // Start-Canvas ausblenden
        }

        if (endCanvas != null)
        {
            endCanvas.SetActive(true);  // End-Canvas einblenden
        }

        raceOngoing = false;  // Das Rennen ist jetzt offiziell beendet
    }

    // Funktion zum Neustarten des Spiels
    public void RestartRace()
    {
        Time.timeScale = 1f;  // Zeit wieder auf normale Geschwindigkeit setzen
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Aktuelle Szene neu laden
    }

    // Funktion zum Hauptmenü zurückkehren
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;  // Zeit wieder auf normale Geschwindigkeit setzen
        SceneManager.LoadScene("Menü");  // Hauptmenü-Szene laden (stelle sicher, dass sie im Build enthalten ist)
    }
}
