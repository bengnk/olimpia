using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Canvas scoreCanvas;     // Das Canvas, das die Punkte anzeigt
    public Text playerScoreText;   // Das Text-Element, das die Punktzahl des Spielers anzeigt

    [SerializeField]               // Erzwingt die Anzeige im Inspector
    private Text[] enemyScoreTexts; // Ein Array von Text-Elementen für die Punktzahlen der Gegner (4 Gegner)
    
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
            // Setze den Text auf die aktuelle Punktzahl des Spielers
            playerScoreText.text = playerScore.ToString();

            // Setze die Punktzahlen der Gegner in die entsprechenden Text-Elemente
            for (int i = 0; i < enemyScores.Length; i++)
            {
                enemyScoreTexts[i].text = enemyScores[i].ToString();
            }

            // Zeige das Canvas an
            scoreCanvas.gameObject.SetActive(true);

            // Markiere, dass die Punktzahl angezeigt wurde
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
