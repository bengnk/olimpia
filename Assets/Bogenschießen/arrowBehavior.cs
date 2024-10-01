using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
    public float speed = 20f;
    private bool isShooting = false;
    private Vector3 targetPosition;
    private Vector3 direction;

    public Rigidbody rb;
    public BoxCollider boxCollider;

    public Collider white;
    public Collider black;
    public Collider blue;
    public Collider red;
    public Collider yellow;

    private int whiteScore = 1;
    private int blackScore = 3;
    private int blueScore = 5;
    private int redScore = 7;
    private int yellowScore = 10;

    public float raycastLength = 0.5f;

    private Vector3 windDirection;
    public float windStrength;
    private float windStrengthKmh;

    private bool hasHit = false;

    // Neue Variablen für Durchgänge und Punkte
    private int currentRound = 1; // Aktuelle Runde (1 bis 3)
    private int totalScore = 0;   // Gesamtpunkte

    private const int maxRounds = 3; // Maximale Anzahl der Durchgänge

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        rb.isKinematic = true;

        windDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        windStrengthKmh = Random.Range(0, 11);
        windStrength = windStrengthKmh / 3.6f;

        string windDirectionName = GetWindDirectionName(windDirection);
        Debug.Log("Windrichtung: " + windDirectionName);
        Debug.Log("Windstärke: " + windStrengthKmh + " km/h");
    }

    void Update()
    {
        if (currentRound <= maxRounds) // Nur schießen, wenn es noch nicht mehr als 3 Runden sind
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;
            }

            if (Input.GetMouseButtonDown(0) && !isShooting)
            {
                StartShooting();
            }

            if (isShooting)
            {
                MoveArrow();
                CheckCollision();
            }
        }
    }

    void StartShooting()
    {
        direction = (targetPosition - transform.position).normalized;
        isShooting = true;
        hasHit = false;
    }

    void MoveArrow()
    {
        Vector3 windInfluence = windDirection * windStrength * Time.deltaTime;
        transform.position += (direction * speed * Time.deltaTime) + windInfluence;
    }

    void CheckCollision()
    {
        RaycastHit hit;
        if (!hasHit && Physics.Raycast(transform.position, direction, out hit, raycastLength))
        {
            hasHit = true;
            isShooting = false;
            rb.isKinematic = true;

            int roundScore = 0;

            if (hit.collider.CompareTag("whiteCircle"))
            {
                roundScore = whiteScore;
            }
            else if (hit.collider.CompareTag("blackCircle"))
            {
                roundScore = blackScore;
            }
            else if (hit.collider.CompareTag("blueCircle"))
            {
                roundScore = blueScore;
            }
            else if (hit.collider.CompareTag("redCircle"))
            {
                roundScore = redScore;
            }
            else if (hit.collider.CompareTag("yellowCircle"))
            {
                roundScore = yellowScore;
            }

            totalScore += roundScore; // Addiere die Punkte zur Gesamtpunktzahl
            Debug.Log("Punkte für Runde " + currentRound + ": " + roundScore);
            Debug.Log("Gesamtpunkte: " + totalScore);

            transform.position = hit.point;

            // Nächste Runde starten oder beenden, wenn die maximale Anzahl erreicht ist
            currentRound++;
            if (currentRound > maxRounds)
            {
                Debug.Log("Spiel beendet! Gesamtpunktzahl: " + totalScore);
            }
            else
            {
                ResetArrow();
            }
        }
    }

    void ResetArrow()
    {
        // Setze den Pfeil an die Startposition zurück und setze das Kinematic-Flag zurück
        transform.position = new Vector3(0, 1, 0); // Beispielhafte Startposition
        rb.isKinematic = true; // Pfeil in der Startposition kinematisch setzen
    }

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
