using UnityEngine;

public class Sprint : MonoBehaviour
{
    public float maxSpeed = 4.17f;              // Maximale Geschwindigkeit (entspricht ca. 15 km/h)
    public int clicksToMaxSpeed = 10;           // Anzahl der Klicks, um 15 km/h zu erreichen
    public float rapidDeceleration = 2f;        // Schnelle Geschwindigkeitsabnahme
    public float deceleration = 0.5f;           // Normale Geschwindigkeitsabnahme

    private float currentSpeed = 0f;            // Aktuelle Geschwindigkeit des Spielers
    private bool lastKeyWasLeft = false;        // Um zu verfolgen, welche Pfeiltaste zuletzt gedrückt wurde
    private float lastKeyTime;                  // Zeit, zu der die letzte Pfeiltaste gedrückt wurde
    private bool canBuildSpeed = true;          // Ob der Spieler die Geschwindigkeit aufbauen kann
    private float acceleration;                 // Effektive Beschleunigung
    private Vector3 moveDirection = Vector3.zero; // Bewegungsrichtung

    void Start()
    {
        lastKeyTime = Time.time;

        // Berechne die Beschleunigung pro Klick so, dass nach 10 Klicks die maximale Geschwindigkeit (15 km/h) erreicht wird
        acceleration = maxSpeed / clicksToMaxSpeed;
    }

    void Update()
    {
        HandleInput();
        Move();
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
                    IncreaseSpeed();
                    lastKeyWasLeft = true;
                    lastKeyTime = Time.time;
                }
                else
                {
                    // Schnelle Geschwindigkeitsreduktion, wenn die Tasten nicht korrekt abwechselnd gedrückt wurden
                    DecreaseSpeedRapidly();
                    lastKeyTime = Time.time; // Zeit auch aktualisieren, wenn die Taste nicht korrekt gedrückt wurde
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (lastKeyWasLeft && Time.time - lastKeyTime < 0.5f)
                {
                    // Geschwindigkeit erhöhen, wenn die Tasten korrekt abwechselnd gedrückt wurden
                    IncreaseSpeed();
                    lastKeyWasLeft = false;
                    lastKeyTime = Time.time;
                }
                else
                {
                    // Schnelle Geschwindigkeitsreduktion, wenn die Tasten nicht korrekt abwechselnd gedrückt wurden
                    DecreaseSpeedRapidly();
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

    private void IncreaseSpeed()
    {
        if (currentSpeed < maxSpeed)
        {
            // Normale Beschleunigung bis 15 km/h
            currentSpeed = Mathf.Min(currentSpeed + acceleration, maxSpeed);

            // Reduzierte Beschleunigung, wenn 15 km/h erreicht sind
            if (currentSpeed >= maxSpeed)
            {
                acceleration = 0.1f;  // Sehr langsame Beschleunigung ab 15 km/h
            }
        }
    }

    private void DecreaseSpeedRapidly()
    {
        currentSpeed = Mathf.Max(currentSpeed - rapidDeceleration * Time.deltaTime, 0);
    }

    private void Move()
    {
        // Bewege das Objekt entsprechend der aktuellen Geschwindigkeit nach vorne
        moveDirection = new Vector3(0, 0, currentSpeed);
        transform.Translate(moveDirection * Time.deltaTime);
    }
}
