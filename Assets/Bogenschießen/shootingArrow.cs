using UnityEngine;
using UnityEngine.UI; // Für UI-Komponenten wie Canvas und Text
using UnityEngine.SceneManagement; // Für das Laden von Szenen

public class ArrowShoot : MonoBehaviour
{
    public Transform shootPoint;    // Startpunkt des Pfeils
    public GameObject arrowPrefab;  // Pfeil-Prefab
    public float shootSpeed = 10f;  // Geschwindigkeit des Pfeils
    public float maxDistance = 10f; // Maximale Flugdistanz des Pfeils (10 Meter)
    public Camera mainCamera;       // Die Kamera, die verwendet wird, um die Mitte des Bildschirms zu bestimmen
    public float raycastLength = 0.5f; // Länge des Raycasts zur Kollisionsprüfung

    private GameObject currentArrow; // Aktueller Pfeil
    private bool isShooting = false; // Status, ob der Pfeil geschossen wird
    private bool hasCollided = false; // Zusätzlicher Status, um Doppelanzeigen zu verhindern
    private Vector3 targetPosition;  // Zielposition des Pfeils
    private float travelledDistance = 0f; // Zurückgelegte Distanz
    private float chargeTime = 0f;    // Zeit, die die Maustaste gedrückt wird
    private bool isCharging = false;   // Status, ob die Maustaste gedrückt wird
    private bool roundEnded = false;   // Status, ob die Runde beendet ist

    // Punkte für die jeweiligen Ringe
    private int whiteScore = 1;
    private int blackScore = 3;
    private int blueScore = 5;
    private int redScore = 7;
    private int yellowScore = 10;

    private int totalScore = 0;      // Gesamtpunktzahl für die Runde
    private int arrowCount = 0;      // Zählt die Anzahl der geschossenen Pfeile
    private int maxArrows = 3;       // Maximale Anzahl der Pfeile

    public ScoreDisplay scoreDisplay; // Referenz auf das ScoreDisplay-Skript

    void Start()
    {
        // Stelle sicher, dass der Cursor zu Beginn des Spiels gesperrt und ausgeblendet ist
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Wenn die Runde beendet ist, keine Eingaben mehr zulassen
        if (roundEnded) return;

        // Überprüfe, ob die linke Maustaste gedrückt ist
        if (Input.GetMouseButton(0))
        {
            chargeTime += Time.deltaTime; // Zeit erhöhen, solange die Taste gedrückt ist
            isCharging = true; // Status auf Laden setzen
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Wenn die Maustaste losgelassen wird
            if (chargeTime >= 3.5f && arrowCount < maxArrows) // Überprüfe, ob die Zeit >= 3,5 Sekunden ist
            {
                ShootArrow(); // Pfeil abschießen
            }
            // Setze die Ladezeit und den Status zurück
            chargeTime = 0f; // Reset der Ladezeit
            isCharging = false; // Status zurücksetzen
        }

        // Bewege den Pfeil, falls er aktiv ist
        if (isShooting && currentArrow != null)
        {
            MoveArrow();
            if (!hasCollided)
            {
                CheckCollision(); // Prüfe die Kollision des Pfeils
            }
        }

        // Überprüfe, ob alle Pfeile abgeschossen wurden
        if (arrowCount >= maxArrows && !isShooting)
        {
            EndRound();
        }
    }

    void ShootArrow()
    {
        // Erhöhe den Pfeilzähler
        arrowCount++;
        Debug.Log("Pfeil Nummer: " + arrowCount);

        // Stelle sicher, dass kein alter Pfeil noch existiert
        if (currentArrow != null)
        {
            Destroy(currentArrow);
        }

        // Erstelle einen neuen Pfeil am Schusspunkt
        currentArrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        isShooting = true;
        hasCollided = false; // Zurücksetzen des Kollisionstatus
        travelledDistance = 0f; // Setze die zurückgelegte Distanz zurück

        // Berechne die Zielposition durch einen Raycast aus der Mitte des Bildschirms
        targetPosition = GetTargetPositionFromScreenCenter();
    }

    void MoveArrow()
    {
        // Berechne die Bewegung basierend auf der Geschwindigkeit
        float step = shootSpeed * Time.deltaTime;

        // Bewege den Pfeil in Richtung der Zielposition
        currentArrow.transform.position = Vector3.MoveTowards(currentArrow.transform.position, targetPosition, step);
        
        // Berechne die bisher zurückgelegte Distanz
        travelledDistance += step;

        // Überprüfe, ob der Pfeil die maximale Distanz erreicht hat oder das Ziel getroffen wurde
        if (travelledDistance >= maxDistance && !hasCollided)
        {
            HandleArrowHit();    // Behandle das Ende des Schusses (z.B. Stoppen des Pfeils)
        }
    }

    void CheckCollision()
    {
        // Überprüfe Kollision mit einem Raycast in Richtung des Pfeils
        RaycastHit hit;
        Vector3 direction = currentArrow.transform.forward;

        if (Physics.Raycast(currentArrow.transform.position, direction, out hit, raycastLength))
        {
            // Sobald eine Kollision erkannt wird, stoppen wir den Pfeil
            HandleArrowHit();

            // Punkte je nach getroffenem Ring anzeigen
            if (hit.collider.CompareTag("whiteCircle"))
            {
                totalScore += whiteScore;
                Debug.Log("Punkte: " + whiteScore);
            }
            else if (hit.collider.CompareTag("blackCircle"))
            {
                totalScore += blackScore;
                Debug.Log("Punkte: " + blackScore);
            }
            else if (hit.collider.CompareTag("blueCircle"))
            {
                totalScore += blueScore;
                Debug.Log("Punkte: " + blueScore);
            }
            else if (hit.collider.CompareTag("redCircle"))
            {
                totalScore += redScore;
                Debug.Log("Punkte: " + redScore);
            }
            else if (hit.collider.CompareTag("yellowCircle"))
            {
                totalScore += yellowScore;
                Debug.Log("Punkte: " + yellowScore);
            }

            // Bewege den Pfeil an die exakte Position des Aufschlags, um ein Durchdringen zu verhindern
            currentArrow.transform.position = hit.point;
        }
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
        // Stoppe den Pfeil, da er entweder das Ziel getroffen oder die maximale Distanz erreicht hat
        isShooting = false;
        hasCollided = true;

        Debug.Log("Pfeil hat das Ziel erreicht oder die maximale Distanz erreicht!");        
    }

    void EndRound()
    {
        Debug.Log("Runde beendet! Gesamtpunkte: " + totalScore);

        // Zeige die Punktzahl auf dem Canvas an
        scoreDisplay.ShowScore(totalScore);

        // Setze den Status, dass die Runde beendet ist, um weitere Eingaben zu verhindern
        roundEnded = true;

        // **Spiele anhalten**
        Time.timeScale = 0; // Das Spiel pausieren

        // **Cursor sichtbar und freigegeben machen**
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Generiere zufällige Punktzahl für den Gegner zwischen 23 und 30
        int enemyScore = Random.Range(23, 31); // Gegnerpunkte zwischen 23 und 30
        Debug.Log("Gegner-Punkte: " + enemyScore);

        // Überprüfe, wer gewonnen hat
        if (totalScore > enemyScore)
        {
            Debug.Log("Du hast gewonnen!");
        }
        else if (totalScore < enemyScore)
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
    }

    // Methode zum Fortsetzen des Spiels (wenn du später den Button drückst)
    public void ResumeGame()
    {
        Time.timeScale = 1; // Spiel fortsetzen
        roundEnded = false; // Runde zurücksetzen, damit das Spiel weitergeht
        // Stelle sicher, dass der Cursor zu Beginn des Spiels gesperrt und ausgeblendet ist
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    
    }

    // Methode zum Neustart des Spiels
    public void RestartGame()
    {
        Time.timeScale = 1; // Spielzeit zurücksetzen
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Lädt die aktuelle Szene neu
        // Stelle sicher, dass der Cursor zu Beginn des Spiels gesperrt und ausgeblendet ist
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
       
    }

    // Methode zum Zurückgehen ins Hauptmenü
    public void LoadMainMenu()
    {
        Time.timeScale = 1; // Spielzeit zurücksetzen
        SceneManager.LoadScene("Menü"); // Lädt die Hauptmenü-Szene (stelle sicher, dass die Szene korrekt benannt ist)

        
    }
}
