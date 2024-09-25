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

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        rb.isKinematic = true; // Kinematischer Rigidbody, damit keine unberechenbare Physik aktiv ist
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
            CheckCollision();
        }
    }

    void StartShooting()
    {
        direction = (targetPosition - transform.position).normalized; // Richtung zum Ziel berechnen
        isShooting = true;
    }

    void MoveArrow()
    {
        // Manuelle Bewegung des Pfeils in gerader Linie zum Ziel
        transform.position += direction * speed * Time.deltaTime;
    }

    void CheckCollision()
    {
        // Überprüfe Kollision mit einem Raycast in Richtung des Pfeils
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, raycastLength))
        {
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
}
