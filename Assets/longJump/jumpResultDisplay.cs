using UnityEngine;
using UnityEngine.UI;  // Für UI-Elemente wie Text und Buttons
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // Für Szenenwechsel (Neustart und Hauptmenü)

public class JumpResultDisplay : MonoBehaviour
{
    public Canvas resultsCanvas;         // Das Canvas für die Sprungergebnisse
    public Text playerJumpText;          // Der Text für die Sprungweite des Spielers

    // Array von 10 Texten für die Gegnerergebnisse
    public Text[] opponentResultTexts = new Text[10];  // Referenzen für die 10 Textfelder

    // Buttons für Neustart und Hauptmenü
    public Button restartButton;  // Der Button für das Neustarten
    public Button mainMenuButton;  // Der Button für das Hauptmenü

    void Start()
    {
        // Stelle sicher, dass das Canvas zu Beginn nicht sichtbar ist
        if (resultsCanvas != null)
        {
            resultsCanvas.gameObject.SetActive(false);
        }

        // Verknüpfe die Button-Events mit den Methoden
        restartButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(GoToMainMenu);
    }

    // Zeige die Sprungweite des Spielers und die Sprungweiten der Gegner an
    public void ShowJumpResults(float playerDistance, List<string> opponentResults)
    {
        if (resultsCanvas != null)
        {
            // Zeige die Sprungweite des Spielers an
            playerJumpText.text = playerDistance.ToString("F2") + " m";

            // Zeige die Ergebnisse der Gegner in den entsprechenden Textfeldern an
            for (int i = 0; i < opponentResults.Count && i < opponentResultTexts.Length; i++)
            {
                opponentResultTexts[i].text = opponentResults[i];
            }

            // Warte 5 Sekunden und blende dann das Canvas ein
            Invoke("DisplayCanvas", 5f);
        }
    }

    // Methode, um das Canvas anzuzeigen
    private void DisplayCanvas()
    {
        resultsCanvas.gameObject.SetActive(true);
    }

    // Methode für den Neustart des Spiels
    private void RestartGame()
    {
        Time.timeScale = 1f;  // Zeit wieder normalisieren, falls pausiert
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  // Neuladen der aktuellen Szene
    }

    // Methode für den Wechsel ins Hauptmenü
    private void GoToMainMenu()
    {
        Time.timeScale = 1f;  // Zeit wieder normalisieren
        SceneManager.LoadScene("Menü");  // Wechsle zur Hauptmenü-Szene (ersetze "MainMenu" mit dem richtigen Szenennamen)
    }
}
