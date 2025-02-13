using UnityEngine;
using UnityEngine.UI;  // Für UI-Elemente wie Text und Buttons
using System.Collections.Generic;
using UnityEngine.SceneManagement;  // Für Szenenwechsel (Neustart und Hauptmenü)

public class JumpResultDisplay : MonoBehaviour
{
    public Canvas resultsCanvas;         // Das Canvas für die Sprungergebnisse
    public Text playerJumpText;          // Der Text für die Sprungweite des Spielers

    // Array von 10 Texten für die Gegnerergebnisse
    public Text[] opponentResultTexts = new Text[5];  // Referenzen für die 10 Textfelder
    public Text[] playerTexts = new  Text[5];

    // Buttons für Neustart und Hauptmenü
    public Button restartButton;  // Der Button für das Neustarten
    public Button mainMenuButton;  // Der Button für das Hauptmenü

    public AudioSource finishSound;
    private bool finishSoundPlayed = false;

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
    public void ShowJumpResults(string playerDistance, List<string> opponentResults)
{
    if (resultsCanvas != null)
    {
        List<float> allResults = new List<float>();
        List<string> foulResults = new List<string>(); // Speichert Fouls separat

        // Spieler-Sprungweite zur Liste hinzufügen
        if (playerDistance == "0,00" || playerDistance == "Foul") 
        {
            //playerJumpText.text = playerDistance;
            foulResults.Add("Foul");
        }
        else if (float.TryParse(playerDistance, out float playerScore))
        {
            allResults.Add(playerScore);
            //playerJumpText.text = playerDistance + " m";
        }

        // Gegner-Sprungweiten verarbeiten
        foreach (string opponent in opponentResults)
        {

            if (opponent == "Foul") 
            {
                foulResults.Add("Foul");
            }
            else if (float.TryParse(opponent, out float result))
            {
                allResults.Add(result);
            }
        }

        // Ergebnisse nach Größe sortieren (höchster Wert zuerst)
        allResults.Sort((a, b) => b.CompareTo(a));

        // Ergebnisse in die UI eintragen
        int index = 0;
        int count = 0;
        int count2 = 0;
        foreach (float result in allResults)
        {
            if (index < opponentResultTexts.Length)
            {
                if (float.TryParse(playerDistance, out float playerDistanceFloat) && result == playerDistanceFloat && count == 0) {
                    opponentResultTexts[index].text = result.ToString("F2") + " m";
                    playerTexts[index].text = "Du";
                    count++; // damit nicht mehrfach Du angezeigt werden kann
                } else {
                    opponentResultTexts[index].text = result.ToString("F2") + " m";
                    playerTexts[index].text = "Gegner " + index;
                }
                
                index++;
            }
        }

        // Foul-Ergebnisse hinten in die Liste eintragen
        foreach (string foul in foulResults)
        {
            if (index < opponentResultTexts.Length)
            {   
                if (playerDistance == "Foul" && count2 == 0) {
                    playerTexts[index].text = "Du";
                    opponentResultTexts[index].text = "Foul";
                    count2++;
                } else {
                    playerTexts[index].text = "Gegner " + index;
                    opponentResultTexts[index].text = "Foul";
                }
                
                index++;
            }
        }

        // Nach 5 Sekunden das Canvas anzeigen
        Invoke("DisplayCanvas", 5f);
    }
}


    // Methode, um das Canvas anzuzeigen
    private void DisplayCanvas()
    {
        resultsCanvas.gameObject.SetActive(true);
        if(!finishSoundPlayed) {
            finishSoundPlayed = true;
            finishSound.Play();
        }
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
