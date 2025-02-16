using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;  // Für die Sortierung

public class GameManager : MonoBehaviour
{
    public Text countdownText;
    public Image timerBackground;

    // Zwei Arrays: links für die Bahn-/Schwimmerangaben, rechts für die Zeiten
    public Text[] laneResultTexts; 
    public Text[] timeResultTexts;

    // Dictionaries für Start- und Endzeiten (jeweils relativ zur Rennstartzeit)
    private Dictionary<int, float> swimmerTimes = new Dictionary<int, float>();
    private Dictionary<int, float> swimmerStartTimes = new Dictionary<int, float>();

    public bool isGoTime = false;
    public bool gameStarted = false;      
    public bool countdownStarted = false; 
    private bool raceOngoing = false;

    private float raceStartTime;
    public int totalSwimmers = 5;  // Anzahl der Schwimmer

    public GameObject startCanvas;
    public GameObject endCanvas;
    public GameObject countdownCanvas;

    public Button restartButton;
    public Button mainMenuButton;

    // Signalisiert den Rennstart für alle Schwimmer (Spieler & Gegner)
    public bool activation = false;
    public SchwimmerController3D swimmerController;

    // Hier trägst du deine Spieler-ID ein – z. B. 4 (das ist dann deine Bahn)
    public int playerSwimmerID = 4;

    void Start()
    {
        timerBackground.color = new Color(0, 0, 0, 0);

        if (startCanvas != null)
            startCanvas.SetActive(true);
        if (endCanvas != null)
            endCanvas.SetActive(false);
        if (countdownCanvas != null)
            countdownCanvas.SetActive(true);

        Time.timeScale = 0f;
        countdownText.text = "";

        restartButton.onClick.AddListener(RestartRace);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void Update()
    {
        // Starte das Spiel mit der Leertaste
        if (Input.GetKeyDown(KeyCode.Space) && !gameStarted)
        {
            StartGame();
            timerBackground.color = new Color(0f, 0f, 0f, 0.5f);
        }

        if (raceOngoing)
        {
            UpdateRaceTime();
        }
    }

    private void StartGame()
    {
        gameStarted = true;
        Time.timeScale = 1f;
        if (startCanvas != null)
            startCanvas.SetActive(false);
        // Im Spieler-Skript erscheint dann der Charge-Balken – der Countdown startet nach dem Linksklick.
    }

    // Wird vom Spieler-Skript beim ersten Klick aufgerufen
    public void StartCountdown()
    {
        if (!countdownStarted)
        {
            countdownStarted = true;
            StartCoroutine(StartRace());
        }
    }

    private IEnumerator StartRace()
    {
        int countdown = 3;
        while (countdown > 0)
        {
            countdownText.text = countdown.ToString();
            yield return new WaitForSeconds(1);
            countdown--;
        }
        countdownText.text = "Go!";
        isGoTime = true;
        raceStartTime = Time.time;  // Rennstartzeit festlegen
        raceOngoing = true;
    }

    private void UpdateRaceTime()
    {
        activation = true; 
        float currentTime = Time.time - raceStartTime;
        countdownText.text = currentTime.ToString("F2") + " s";
    }

    // Jeder Schwimmer ruft diesen Timerstart genau einmal beim Sprung auf
    public void StartTimer(int swimmerID)
    {
        if (isGoTime && !swimmerStartTimes.ContainsKey(swimmerID))
        { 
            swimmerStartTimes[swimmerID] = Time.time - raceStartTime;
            Debug.Log("Schwimmer " + swimmerID + " startet bei " + swimmerStartTimes[swimmerID] + " s");
        }
    }

    // Beim Ziel wird die Endzeit relativ zum Start gemessen (T2 - T1)
    public void StopTimer(int swimmerID)
{
    if (!swimmerTimes.ContainsKey(swimmerID))
    {
        // Messe die Rennzeit ab dem offiziellen Start (raceStartTime)
        float elapsedTime = Time.time - raceStartTime;
        swimmerTimes[swimmerID] = elapsedTime;
        DisplayResults();
        Debug.Log("Schwimmer " + swimmerID + " finish time: " + elapsedTime + " s");

        if (swimmerTimes.Count == totalSwimmers)
        {
            ShowEndCanvas();
        }
    }
}


    // Ergebnisse sortieren und in zwei Spalten anzeigen:
    // Linke Spalte: "Bahn X" (wo X die Schwimmer-ID ist)
    // Rechte Spalte: Zeit in s, evtl. mit Medaillen und Kennzeichnung (Du)
    private void DisplayResults()
    {
        var sortedResults = swimmerTimes.OrderBy(x => x.Value).ToList();

        for (int i = 0; i < laneResultTexts.Length; i++)
        {
            if (i < sortedResults.Count)
            {
                // Medaillen für Top 3
                string medal = "";
                if (i == 0)
                    medal = " (Gold)";
                else if (i == 1)
                    medal = " (Silber)";
                else if (i == 2)
                    medal = " (Bronze)";

                // Linke Spalte: Zeigt direkt "Bahn [Schwimmer-ID]"
                laneResultTexts[i].text = "Bahn " + sortedResults[i].Key.ToString();

                // Falls dieser Schwimmer dein eigener ist, wird das markiert
                string playerMark = "";
                if (sortedResults[i].Key == playerSwimmerID)
                    playerMark = " (Du)";

                // Rechte Spalte: Zeit + Medaillen + ggf. (Du)
                timeResultTexts[i].text = sortedResults[i].Value.ToString("F2") + " s" + medal + playerMark;
            }
            else
            {
                laneResultTexts[i].text = "";
                timeResultTexts[i].text = "";
            }
        }
    }

    private void ShowEndCanvas()
    {
        if (startCanvas != null)
            startCanvas.SetActive(false);
        if (endCanvas != null)
            endCanvas.SetActive(true);
        raceOngoing = false;
    }

    public void RestartRace()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menü");
    }
}
