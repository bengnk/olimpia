using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreDisplay : MonoBehaviour
{
    public Canvas scoreCanvas;      // Das Canvas, das die Punktzahlen anzeigt
    public Text[] scoreTexts;       // Ein Array für alle Punktestand-Textobjekte (Spieler + Gegner)
    public Text[] nameTexts;        // Ein Array für alle Namen (Spieler + Gegner)
    public Text highscoreValue;     // Neues UI-Element für den Highscore

    private bool isScoreShown = false;

    void Start()
    {
        // Stelle sicher, dass das Canvas zu Beginn deaktiviert ist
        scoreCanvas.gameObject.SetActive(false);
    }

    // Methode zum Anzeigen der Punktzahlen des Spielers und der Gegner
    public void ShowScore(int playerScore, int[] enemyScores)
    {
        if (!isScoreShown)
        {
            // Erstelle eine Liste für Spieler- und Gegner-Punkte
            List<(int score, string name)> scoreList = new List<(int, string)>
            {
                (playerScore, "Du") // Spieler hinzufügen
            };

            // Namen der Gegner
            string[] enemyNames = new string[] { "Robin Hood", "Legolas", "Katniss Everdeen", "Green Arrow" };

            // Füge Gegner-Scores hinzu
            for (int i = 0; i < enemyScores.Length; i++)
            {
                scoreList.Add((enemyScores[i], enemyNames[i]));
            }

            // Sortiere alle Punkte absteigend
            scoreList.Sort((a, b) => b.score.CompareTo(a.score));

            // Aktualisiere die UI mit den sortierten Werten
            for (int i = 0; i < scoreList.Count && i < scoreTexts.Length; i++)
            {
                nameTexts[i].text = scoreList[i].name;
                scoreTexts[i].text = scoreList[i].score.ToString();
            }

            // Highscore aus PlayerPrefs holen und aktualisieren
            int highscore = PlayerPrefs.GetInt("HighScore", 0);
            highscoreValue.text = highscore.ToString();

            // Zeige das Canvas an
            scoreCanvas.gameObject.SetActive(true);
            isScoreShown = true;
        }
    }

    // Methode, um das Canvas auszublenden
    public void HideScore()
    {
        scoreCanvas.gameObject.SetActive(false);
        isScoreShown = false;
    }
}
