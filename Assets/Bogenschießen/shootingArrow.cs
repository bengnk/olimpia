using UnityEngine;

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

    private Rigidbody arrowRb;      // Rigidbody des aktuellen Pfeils

    // Punkte für die jeweiligen Ringe
    private int whiteScore = 1;
    private int blackScore = 3;
    private int blueScore = 5;
    private int redScore = 7;
    private int yellowScore = 10;

    // Methode für den ersten Schuss vorbereiten

    void Update()
    {
        
        // Überprüfe, ob der Spieler die Schusstaste drückt und kein Pfeil bereits fliegt
        if (Input.GetButtonDown("Fire1"))
        {
            ShootArrow();
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
    }

    void ShootArrow()
    {
        
        
        // Stelle sicher, dass kein alter Pfeil noch existiert
        if (currentArrow != null)
        {
            Destroy(currentArrow);
        }

        // Erstelle einen neuen Pfeil am Schusspunkt
        currentArrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);
        arrowRb = currentArrow.GetComponent<Rigidbody>();
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
            HandleArrowHit();    // Behandle das Ende des Schusses (z.B. Zerstören des Pfeils)
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
                Debug.Log("Punkte: " + whiteScore);
            }
            else if (hit.collider.CompareTag("blackCircle"))
            {
                Debug.Log("Punkte: " + blackScore);
            }
            else if (hit.collider.CompareTag("blueCircle"))
            {
                Debug.Log("Punkte: " + blueScore);
            }
            else if (hit.collider.CompareTag("redCircle"))
            {
                Debug.Log("Punkte: " + redScore);
            }
            else if (hit.collider.CompareTag("yellowCircle"))
            {
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

        // Der Spieler darf wieder schießen, nachdem der Pfeil eine Kollision hatte oder die maximale Distanz erreicht hat
        

        // Zerstöre den Pfeil nach der Kollision oder wenn er die maximale Distanz erreicht hat
        Destroy(currentArrow, 5f);

        Debug.Log("Pfeil hat das Ziel erreicht oder die maximale Distanz erreicht!");
    }
}
