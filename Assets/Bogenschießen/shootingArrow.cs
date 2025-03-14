using TMPro; // Notwendig für TextMeshPro
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class ArrowShoot : MonoBehaviour
{
    // Vorherige Variablen
    public Transform shootPoint;
    public GameObject arrowPrefab;
    public float shootSpeed = 5f;
    public float maxDistance = 1f;
    public Camera mainCamera;
    public Camera scoreCamera; // Neue Kamera für das Ergebnis
    public float raycastLength = 0.25f;

    private GameObject currentArrow;
    private bool isShooting = false;
    private bool hasCollided = false;
    private Vector3 targetPosition;
    private float travelledDistance = 0f;
    private float chargeTime = 0f;
    private bool isCharging = false;
    private bool roundEnded = false;

    private int whiteScore = 1;
    private int blackScore = 3;
    private int blueScore = 5;
    private int redScore = 7;
    private int yellowScore = 10;
    private int highScore = 0;

    private int totalScore = 0;
    private int arrowCount = 0;
    private int maxArrows = 3;

    public ScoreDisplay scoreDisplay;

    // Neue Variable für TextMeshPro-Anzeige
    public TextMeshProUGUI displayNumber;

 void Start()
{
    Cursor.lockState = CursorLockMode.Locked;
    Cursor.visible = false;

    // Sicherstellen, dass die scoreCamera zu Beginn des Spiels deaktiviert ist
    scoreCamera.gameObject.SetActive(false);

    highScore = PlayerPrefs.GetInt("HighScore", 0);
}

    void Update()
    {
        if (roundEnded) return;

        if (Input.GetMouseButton(0))
        {
            chargeTime += Time.deltaTime;
            isCharging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (chargeTime >= 3.5f && arrowCount < maxArrows)
            {
                ShootArrow();
            }
            chargeTime = 0f;
            isCharging = false;
        }

        if (isShooting && currentArrow != null)
        {
            MoveArrow();
            if (!hasCollided)
            {
                CheckCollision();
            }
        }

        if (arrowCount >= maxArrows && !isShooting)
        {
            EndRound();
        }
    }

    void ShootArrow()
    {
        arrowCount++;
        if (currentArrow != null) Destroy(currentArrow);

        currentArrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        isShooting = true;
        hasCollided = false;
        travelledDistance = 0f;
        targetPosition = GetTargetPositionFromScreenCenter();
    }

    void MoveArrow()
    {
        float step = shootSpeed * Time.deltaTime;
        currentArrow.transform.position = Vector3.MoveTowards(currentArrow.transform.position, targetPosition, step);
        travelledDistance += step;

        if (travelledDistance >= maxDistance && !hasCollided)
        {
            HandleArrowHit();
        }
    }

    void CheckCollision()
    {
        RaycastHit hit;
        Vector3 direction = currentArrow.transform.forward;

        if (Physics.Raycast(currentArrow.transform.position, direction, out hit, raycastLength))
        {
            HandleArrowHit();

            int score = 0; // Variable für den Bereichs-Score
            if (hit.collider.CompareTag("whiteCircle"))
            {
                score = whiteScore;
                Debug.Log("Getroffene Zone: Weiß, Punkte: " + score);
            }
            else if (hit.collider.CompareTag("blackCircle"))
            {
                score = blackScore;
                Debug.Log("Getroffene Zone: Schwarz, Punkte: " + score);
            }
            else if (hit.collider.CompareTag("blueCircle"))
            {
                score = blueScore;
                Debug.Log("Getroffene Zone: Blau, Punkte: " + score);
            }
            else if (hit.collider.CompareTag("redCircle"))
            {
                score = redScore;
                Debug.Log("Getroffene Zone: Rot, Punkte: " + score);
            }
            else if (hit.collider.CompareTag("yellowCircle"))
            {
                score = yellowScore;
                Debug.Log("Getroffene Zone: Gelb, Punkte: " + score);
            }

            // Nur Punkte hinzufügen, wenn ein gültiger Score ermittelt wurde
            if (score > 0)
            {
                totalScore += score;
                currentArrow.transform.position = hit.point;
                DisplayScore(score); // Punkteanzeige aufrufen
                Debug.Log("Aktueller Gesamtpunktestand: " + totalScore);
            }
        }
    }

    void DisplayScore(int score)
{
    displayNumber.text = $"Punkte: {score}";
    displayNumber.gameObject.SetActive(true);
    
    // Kamera wird nach der Anzeige des aktuellen Punktestands aktiviert
    StartCoroutine(ShowScoreCamera());

    // Punkte nach kurzer Zeit ausblenden
    Invoke(nameof(HideScore), 2f);
}


    void HideScore()
    {
        displayNumber.gameObject.SetActive(false);
    }

    Vector3 GetTargetPositionFromScreenCenter()
    {
        // Erzeuge einen Ray aus der Mitte des Bildschirms
        Ray ray = mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        // Wenn der Ray auf ein Objekt trifft, wird die Trefferposition verwendet
        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        else
        {
            // Wenn der Ray nichts trifft, wird eine Position in der Ferne genommen
            return ray.GetPoint(maxDistance);
        }
    }

    void HandleArrowHit()
    {
        isShooting = false;
        hasCollided = true;
    }

    void EndRound()
    {
        // Plane die Anzeige des Scoreboards in 2 Sekunden
        Invoke("ShowScoreboard", 2f);
    }

    void ShowScoreboard()
    {
        Debug.Log("Endpunktestand: " + totalScore);

        if (totalScore > highScore)
    {
        highScore = totalScore;
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
        Debug.Log("Neuer Highscore: " + highScore);
    }


        // Generiere zufällige Punktzahlen für 4 Gegner zwischen 23 und 30
        int[] validScores = { 3, 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25, 27, 30 }; // Alle möglichen Summen
        int[] enemyScores = new int[4];

        for (int i = 0; i < 4; i++)
        {
            enemyScores[i] = validScores[Random.Range(0, validScores.Length)];
            Debug.Log("Gegner " + (i + 1) + " Punktestand: " + enemyScores[i]);
        }


        // Zeige die Punktzahl auf dem Canvas an, sowohl Spieler- als auch Gegnerpunkte
        scoreDisplay.ShowScore(totalScore, enemyScores);

        // Überprüfe, wer gewonnen hat
        int highestEnemyScore = Mathf.Max(enemyScores);
        if (totalScore > highestEnemyScore)
        {
            Debug.Log("Du hast gewonnen!");
        }
        else if (totalScore < highestEnemyScore)
        {
            Debug.Log("Du hast verloren!");
        }
        else
        {
            Debug.Log("Unentschieden!");
        }

        // Punkte und Pfeilanzahl für die nächste Runde zurücksetzen (falls benötigt)
        totalScore = 0;
        arrowCount = 0;

        // Spiele anhalten
        Time.timeScale = 0; // Das Spiel pausieren

        // Cursor sichtbar und freigegeben machen
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Coroutine, um die scoreCamera für eine bestimmte Zeit zu aktivieren
       IEnumerator ShowScoreCamera()
    {
        scoreCamera.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        scoreCamera.gameObject.SetActive(false);
    }


    public void ResumeGame()
    {
        Time.timeScale = 1;
        roundEnded = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("AuswahlBogenschießen");
        
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menü");
    }
}
