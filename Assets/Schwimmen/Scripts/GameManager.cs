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
    // UI-Text für den Highscore
    public Text highscoreText;

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

    // Highscore – hier wird die beste (niedrigste) Zeit des Spielers gespeichert
    private float highscore;

    void Start()
    {
        // Lade den Highscore aus PlayerPrefs (falls vorhanden)
        if (PlayerPrefs.HasKey("Highscore"))
        {
            highscore = PlayerPrefs.GetFloat("Highscore");
        }
        else
        {
            // Falls noch kein Highscore existiert, setzen wir einen sehr hohen Standardwert.
            highscore = float.MaxValue;
        }
        UpdateHighscoreText();

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
        timerBackground.gameObject.SetActive(true);

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

            // Falls dieser Schwimmer der Spieler ist, Highscore aktualisieren
            if (swimmerID == playerSwimmerID)
            {
                if (elapsedTime < highscore)
                {
                    highscore = elapsedTime;
                    SaveHighscore(highscore);
                    UpdateHighscoreText();
                    Debug.Log("Neuer Highscore: " + highscore + " s");
                }
            }

            if (swimmerTimes.Count == totalSwimmers)
            {
                ShowEndCanvas();
            }
        }
    }


private void DisplayResults()
{
    var sortedResults = swimmerTimes.OrderBy(x => x.Value).ToList();

    for (int i = 0; i < laneResultTexts.Length; i++)
    {
        if (i < sortedResults.Count)
        {
            int lane = sortedResults[i].Key;
            string displayName = "";

            // Namen je nach Bahnzuordnung: Bei Bahn 4 bleibt es "du"
            switch (lane)
            {
                case 1:
                    displayName = "Michael Phelps (Bahn 1)";
                    break;
                case 2:
                    displayName = "Nemo (Bahn 2)";
                    break;
                case 3:
                    displayName = "Ariel die Meerjungfrau (Bahn 3)";
                    break;
                case 4:
                    // Hier bleibt es ausschließlich "du"
                    displayName = "Du";
                    break;
                case 5:
                    displayName = "Spongebob (Bahn 5)";
                    break;
                default:
                    displayName = "Bahn " + lane.ToString();
                    break;
            }

            laneResultTexts[i].text = displayName;
            timeResultTexts[i].text = sortedResults[i].Value.ToString("F2") + " s";
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

    // Speichert den Highscore in den PlayerPrefs
    private void SaveHighscore(float newHighscore)
    {
        PlayerPrefs.SetFloat("Highscore", newHighscore);
        PlayerPrefs.Save();
    }

    // Aktualisiert den Highscore-Text in der UI
    private void UpdateHighscoreText()
    {
        if (highscoreText != null)
        {
            if (highscore == float.MaxValue)
            {
                highscoreText.text = "Highscore: ---";
            }
            else
            {
                highscoreText.text = "Highscore: " + highscore.ToString("F2") + " s";
            }
        }
    }
}