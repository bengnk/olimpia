using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ScoreDisplay : MonoBehaviour
{
    public Canvas scoreCanvas;      // Das Canvas, das die Punktzahlen anzeigt
    public Text playerScoreText;    // Das Text-Element für die Punktzahl des Spielers
    public Text[] enemyScoreTexts;  // Ein Array von Text-Elementen für die Gegner-Punktzahlen
    public Text[] enemyScore;       // Ein Array, das die Gegner-Punktzahlen anzeigt

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
            // Setze den Spieler-Score immer separat
            playerScoreText.text = $"{playerScore}";

            // Erstelle eine Liste für Gegner-Punkte und Namen
            List<(int score, string name)> enemyScoreList = new List<(int, string)>();
            string[] enemyNames = new string[] { "Robin Hood", "Legolas", "Katniss Everdeen", "Green Arrow" };

            for (int i = 0; i < enemyScores.Length; i++)
            {
                // Füge die Namen der berühmten Bogenschützen hinzu
                enemyScoreList.Add((enemyScores[i], enemyNames[i]));
            }

            // Sortiere die Gegner-Punkte absteigend
            enemyScoreList.Sort((a, b) => b.score.CompareTo(a.score));

            // Weise die sortierten Gegner-Scores zu
            for (int i = 0; i < enemyScoreTexts.Length && i < enemyScoreList.Count; i++)
            {
                enemyScoreTexts[i].text = enemyScoreList[i].name;
                enemyScore[i].text = enemyScoreList[i].score.ToString();
            }

            // Zeige das Canvas an
            scoreCanvas.gameObject.SetActive(true);
            isScoreShown = true;
        }
    }

    // Methode, um das Canvas auszublenden, falls erforderlich
    public void HideScore()
    {
        scoreCanvas.gameObject.SetActive(false);
        isScoreShown = false;
    }
}
