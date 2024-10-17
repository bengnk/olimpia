using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    public Canvas scoreCanvas;   // Das Canvas, das die Punkte anzeigt
    public Text scoreText;       // Das Text-Element, das die Punktzahl anzeigt
    private bool isScoreShown = false;

    void Start()
    {
        // Stelle sicher, dass das Canvas zu Beginn deaktiviert ist
        scoreCanvas.gameObject.SetActive(false);
    }

    // Methode zum Anzeigen der Punktzahl
    public void ShowScore(int totalScore)
    {
        if (!isScoreShown)
        {
            // Setze den Text auf die aktuelle Punktzahl
            scoreText.text = "Gesamtpunkte: " + totalScore.ToString();

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
