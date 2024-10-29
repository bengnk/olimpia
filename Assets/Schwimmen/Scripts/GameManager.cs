using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Text resultsText;
    public Text countdownText;

    public Image timerBackground;

    private Dictionary<int, float> swimmerTimes = new Dictionary<int, float>();
    private Dictionary<int, float> swimmerStartTimes = new Dictionary<int, float>();
    public bool isGoTime = false;
    public bool gameStarted = false;
    private bool raceOngoing = false;

    private float raceStartTime;

    public int totalSwimmers = 4;

    public GameObject startCanvas;
    public GameObject endCanvas;
    public GameObject countdownCanvas;

    public Button restartButton;
    public Button mainMenuButton;

    void Start()
    {
        timerBackground.color = new Color(0, 0, 0, 0);

        if (startCanvas != null)
        {
            startCanvas.SetActive(true);
        }

        if (endCanvas != null)
        {
            endCanvas.SetActive(false);
        }

        if (countdownCanvas != null)
        {
            countdownCanvas.SetActive(true);
        }

        Time.timeScale = 0f;
        countdownText.text = "";

        restartButton.onClick.AddListener(RestartRace);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    void Update()
    {
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
        {
            startCanvas.SetActive(false);
        }

        if (countdownCanvas != null)
        {
            countdownCanvas.SetActive(true);
        }

        StartCoroutine(StartRace());
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

        yield return new WaitForSeconds(1);

        raceStartTime = Time.time;  // Startzeit des Rennens festlegen
        raceOngoing = true;
    }

    private void UpdateRaceTime()
    {
        float currentTime = Time.time - raceStartTime;
        countdownText.text = currentTime.ToString("F2") + "s";
    }

    public void StartTimer(int swimmerID)
    {
        if (isGoTime && !swimmerStartTimes.ContainsKey(swimmerID))
        {
            swimmerStartTimes[swimmerID] = Time.time - raceStartTime;  // Startzeit des Schwimmers relativ zur Rennstartzeit
            Debug.Log("Schwimmer " + swimmerID + " hat gestartet.");
        }
    }

    public void StopTimer(int swimmerID)
    {
        if (swimmerStartTimes.ContainsKey(swimmerID) && !swimmerTimes.ContainsKey(swimmerID))
        {
            float endTime = Time.time - raceStartTime - swimmerStartTimes[swimmerID];  // Endzeit relativ zur Startzeit des Schwimmers
            swimmerTimes[swimmerID] = endTime;
            DisplayResults();
            Debug.Log("Schwimmer " + swimmerID + " hat das Ziel in " + endTime + " Sekunden erreicht.");

            if (swimmerTimes.Count == totalSwimmers)
            {
                ShowEndCanvas();
            }
        }
    }

    private void DisplayResults()
    {
        if (resultsText == null)
        {
            Debug.LogError("resultsText ist nicht zugewiesen! Bitte stelle sicher, dass das UI-Textfeld im Inspector gesetzt ist.");
            return;
        }

        resultsText.text = "";
        foreach (var entry in swimmerTimes)
        {
            resultsText.text += entry.Value.ToString("F2");
        }
    }

    private void ShowEndCanvas()
    {
        if (startCanvas != null)
        {
            startCanvas.SetActive(false);
        }

        if (endCanvas != null)
        {
            endCanvas.SetActive(true);
        }

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
