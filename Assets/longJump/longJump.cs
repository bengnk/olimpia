using UnityEngine;

public class LongJump3D : MonoBehaviour
{
    public float maxSpeed = 30f;              // Maximale Geschwindigkeit
    public int clicksToMaxSpeed = 10;         // Anzahl der Klicks, um maximale Geschwindigkeit zu erreichen
    public float rapidDeceleration = 30f;     // Schnelle Geschwindigkeitsabnahme
    public float deceleration = 5f;           // Normale Geschwindigkeitsabnahme
    public float jumpForce = 10f;             // Kraft des Sprungs
    public float perfectTimingWindow = 0.5f;  // Zeitfenster für perfekten Sprung
    public float timingRange = 2f;            // Zeitfenster, um den Sprung zu starten

    private Rigidbody rb;                     // Rigidbody des Spielers
    private bool hasJumped = false;           // Hat der Spieler bereits gesprungen?
    private Vector3 jumpStartPosition;        // Position des Sprungstartpunkts
    private float startTime;                  // Startzeit des Anlaufs
    private float currentSpeed = 0f;          // Aktuelle Geschwindigkeit des Spielers
    private bool lastKeyWasLeft = false;      // Um zu verfolgen, welche Pfeiltaste zuletzt gedrückt wurde
    private float lastKeyTime;                // Zeit, zu der die letzte Pfeiltaste gedrückt wurde
    private bool canBuildSpeed = true;        // Ob der Spieler die Geschwindigkeit aufbauen kann
    private float acceleration;               // Effektive Beschleunigung

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startTime = Time.time;  // Setze die Startzeit beim Spielbeginn
        lastKeyTime = Time.time;

        // Berechne die Beschleunigung pro Klick so, dass nach 10 Klicks die maximale Geschwindigkeit erreicht wird
        acceleration = maxSpeed / clicksToMaxSpeed;
    }

    void Update()
    {
        if (!hasJumped)
        {
            HandleInput();
            Move();
            CheckJump();
        }
    }

    private void HandleInput()
    {
        if (canBuildSpeed)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (!lastKeyWasLeft && Time.time - lastKeyTime < 0.5f)
                {
                    // Geschwindigkeit erhöhen, wenn die Tasten korrekt abwechselnd gedrückt wurden
                    currentSpeed = Mathf.Min(currentSpeed + acceleration, maxSpeed);
                    lastKeyWasLeft = true;
                    lastKeyTime = Time.time;
                }
                else
                {
                    // Schnelle Geschwindigkeitsreduktion, wenn die Tasten nicht korrekt abwechselnd gedrückt wurden
                    currentSpeed = Mathf.Max(currentSpeed - rapidDeceleration * Time.deltaTime, 0);
                    lastKeyTime = Time.time; // Zeit auch aktualisieren, wenn die Taste nicht korrekt gedrückt wurde
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (lastKeyWasLeft && Time.time - lastKeyTime < 0.5f)
                {
                    // Geschwindigkeit erhöhen, wenn die Tasten korrekt abwechselnd gedrückt wurden
                    currentSpeed = Mathf.Min(currentSpeed + acceleration, maxSpeed);
                    lastKeyWasLeft = false;
                    lastKeyTime = Time.time;
                }
                else
                {
                    // Schnelle Geschwindigkeitsreduktion, wenn die Tasten nicht korrekt abwechselnd gedrückt wurden
                    currentSpeed = Mathf.Max(currentSpeed - rapidDeceleration * Time.deltaTime, 0);
                    lastKeyTime = Time.time; // Zeit auch aktualisieren, wenn die Taste nicht korrekt gedrückt wurde
                }
            }
        }

        // Schnelle Geschwindigkeitsreduktion, wenn keine Tasten mehr gedrückt werden
        if (Time.time - lastKeyTime > 0.5f && canBuildSpeed)
        {
            currentSpeed = Mathf.Max(currentSpeed - rapidDeceleration * Time.deltaTime, 0);
        }
    }

    private void Move()
    {
        // Bewege das Objekt entsprechend der aktuellen Geschwindigkeit nach vorne
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float currentTime = Time.time;
            float elapsedTime = currentTime - startTime;

            // Setze die Startposition für den Sprung
            jumpStartPosition = transform.position;

            // Berechne die Sprungkraft basierend auf der aktuellen Geschwindigkeit
            float jumpSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            // Prüfen, ob die Leertaste innerhalb des Sprungzeitfensters gedrückt wurde
            if (elapsedTime >= timingRange && elapsedTime <= timingRange + perfectTimingWindow)
            {
                Jump(jumpSpeed);  // Perfekter Sprung
            }
            else
            {
                Jump(jumpSpeed / 2);  // Nicht perfekter Sprung, halbe Kraft
            }

            hasJumped = true;  // Spieler hat gesprungen
            currentSpeed = 0;  // Stoppe die horizontale Bewegung
            canBuildSpeed = false; // Verhindere das weitere Aufbauen der Geschwindigkeit
        }
    }

    private void Jump(float speed)
    {
        rb.velocity = new Vector3(0, jumpForce, speed);  // Sprung ausführen mit einer vertikalen und einer vorwärts gerichteten Kraft
    }

    void OnCollisionEnter(Collision collision)
    {
        // Sicherstellen, dass die Berechnung und das Zurücksetzen nur erfolgen, wenn der Spieler nach dem Sprung landet
        if (hasJumped && collision.gameObject.CompareTag("Ground"))
        {
            // Berechne die Sprungweite
            float jumpDistance = Vector3.Distance(jumpStartPosition, transform.position);
            Debug.Log("Sprungweite: " + jumpDistance);

            // Nach dem Landen, setze den Sprung zurück
            hasJumped = false;           // Spieler kann wieder springen
            startTime = Time.time;       // Setze die Startzeit für den nächsten Anlauf zurück
            currentSpeed = 0f;           // Geschwindigkeit zurücksetzen
            canBuildSpeed = true;        // Erlaube den Aufbau der Geschwindigkeit wieder
        }
    }
}
