using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
    public float speed = 20f; // Geschwindigkeit des Pfeils
    private bool isShooting = false;
    private Vector3 targetPosition;
    private Vector3 direction;

    public Rigidbody rb; // Rigidbody des Pfeils
    public BoxCollider boxCollider; // BoxCollider des Pfeils

    // Referenzen für die Ziel-Ringe
    public Collider white;
    public Collider black;
    public Collider blue;
    public Collider red;
    public Collider yellow;

    // Punkte für die jeweiligen Ringe
    private int whiteScore = 1;
    private int blackScore = 3;
    private int blueScore = 5;
    private int redScore = 7;
    private int yellowScore = 10;

    // Länge des Raycast zur Kollisionserkennung
    public float raycastLength = 0.5f;

    // Windparameter
    private Vector3 windDirection;
    public float windStrength; // Zufällige Windstärke in m/s
    private float windStrengthKmh; // Windstärke in km/h

    // Statusvariable zur Vermeidung mehrfacher Kollisionsergebnisse
    private bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        rb.isKinematic = true; // Kinematischer Rigidbody, damit keine unberechenbare Physik aktiv ist

        // Zufällige Windrichtung generieren
        windDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;

        // Zufällige Windstärke generieren (zwischen 0 und 15 km/h umgerechnet in m/s)
        windStrengthKmh = Random.Range(0, 16); // Zufällige Windstärke in km/h, nur ganze Zahlen
        windStrength = windStrengthKmh / 3.6f;   // Umwandlung von km/h in m/s

        // Himmelsrichtung bestimmen und anzeigen
        string windDirectionName = GetWindDirectionName(windDirection);
        Debug.Log("Windrichtung: " + windDirectionName);
        Debug.Log("Windstärke: " + windStrengthKmh + " km/h");
    }

    void Update()
    {
        // Berechne das Ziel basierend auf dem Mauszeiger
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            targetPosition = hit.point;
        }

        // Überprüfen, ob die linke Maustaste gedrückt wurde und noch nicht geschossen wird
        if (Input.GetMouseButtonDown(0) && !isShooting)
        {
            StartShooting();
        }

        // Wenn geschossen wird, bewege den Pfeil
        if (isShooting)
        {
            MoveArrow();
            CheckCollision(); // Kollision überprüfen, wenn der Pfeil in Bewegung ist
        }
    }

    void StartShooting()
    {
        direction = (targetPosition - transform.position).normalized; // Richtung zum Ziel berechnen
        isShooting = true;
        hasHit = false; // Reset für die Kollisionserkennung
    }

    void MoveArrow()
    {
        // Beeinflusse den Pfeil durch die Windrichtung
        Vector3 windInfluence = windDirection * windStrength * Time.deltaTime;

        // Manuelle Bewegung des Pfeils in gerader Linie zum Ziel + Windbeeinflussung
        transform.position += (direction * speed * Time.deltaTime) + windInfluence;
    }

    void CheckCollision()
    {
        // Überprüfe Kollision mit einem Raycast in Richtung des Pfeils
        RaycastHit hit;
        if (!hasHit && Physics.Raycast(transform.position, direction, out hit, raycastLength))
        {
            hasHit = true; // Setze den Status, um mehrere Treffer zu vermeiden

            // Sobald eine Kollision erkannt wird, stoppen wir den Pfeil
            isShooting = false;
            rb.isKinematic = true; // Den Pfeil anhalten

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
            transform.position = hit.point;
        }
    }

    // Funktion zur Umwandlung der Windrichtung in eine Himmelsrichtung
    string GetWindDirectionName(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        if (angle < 0) angle += 360;

        if (angle >= 337.5f || angle < 22.5f) return "Osten";
        else if (angle >= 22.5f && angle < 67.5f) return "Nordosten";
        else if (angle >= 67.5f && angle < 112.5f) return "Norden";
        else if (angle >= 112.5f && angle < 157.5f) return "Nordwesten";
        else if (angle >= 157.5f && angle < 202.5f) return "Westen";
        else if (angle >= 202.5f && angle < 247.5f) return "Südwesten";
        else if (angle >= 247.5f && angle < 292.5f) return "Süden";
        else if (angle >= 292.5f && angle < 337.5f) return "Südosten";

        return "Unbekannt";
    }
}
