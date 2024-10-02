using UnityEngine;

public class LongJump3D : MonoBehaviour
{
    public float maxSpeed = 35f;                 // Maximale Geschwindigkeit 
    public int clicksToMaxSpeed = 100;           // Anzahl der Klicks
    public float rapidDeceleration = 2f;        // Schnelle Geschwindigkeitsabnahme
    public float deceleration = 0.5f;           // Normale Geschwindigkeitsabnahme
    public float jumpForce = 10f;               // Kraft des Sprungs
    public float perfectTimingWindow = 0.5f;    // Zeitfenster für perfekten Sprung
    public float timingRange = 2f;              // Zeitfenster, um den Sprung zu starten

    private Rigidbody rb;                       // Rigidbody des Spielers
    private bool hasJumped = false;             // Hat der Spieler bereits gesprungen?
    private Vector3 jumpStartPosition;          // Position des Sprungstartpunkts
    private float startTime;                    // Startzeit des Anlaufs
    private float currentSpeed = 0f;            // Aktuelle Geschwindigkeit des Spielers
    private bool lastKeyWasLeft = false;        // Um zu verfolgen, welche Pfeiltaste zuletzt gedrückt wurde
    private float lastKeyTime;                  // Zeit, zu der die letzte Pfeiltaste gedrückt wurde
    private bool canBuildSpeed = true;          // Ob der Spieler die Geschwindigkeit aufbauen kann
    private float acceleration;                 // Effektive Beschleunigung
    private bool foul = false;
    private bool perfectJumpOff = false;
    float lastSpeed;
    bool isSlowingDown = false;
    float jumpDistance;
    float maxHeight = 0;

    private Animator animator;
    private CameraFollow camera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startTime = Time.time;
        lastKeyTime = Time.time;

        camera = Camera.main.GetComponent<CameraFollow>();

        // Berechne die Beschleunigung pro Klick so, dass nach 10 Klicks die maximale Geschwindigkeit erreicht wird
        acceleration = maxSpeed / clicksToMaxSpeed;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float minSpeed = 0.1f;    // Minimale Geschwindigkeit
        float maxSpeed = 30f;   // Maximale Geschwindigkeit
        float minAnimationSpeed = 0.5f; // Minimale Animationsgeschwindigkeit
        float maxAnimationSpeed = 1.2f; // Maximale Animationsgeschwindigkeit

        if (currentSpeed > 0f)
        {
            // Berechne die Animationsgeschwindigkeit basierend auf der aktuellen Geschwindigkeit
            float animationSpeed = Mathf.Lerp(minAnimationSpeed, maxAnimationSpeed, Mathf.InverseLerp(minSpeed, maxSpeed, currentSpeed));
            
            animator.SetBool("Start", true);
            animator.SetBool("Stand", false);
            animator.speed = animationSpeed;
        }

        if (!hasJumped)
        {
            HandleInput();
            Move();
            CheckJump();
        }

        float height = CalculateCurrentHeight();
        
        if (height > maxHeight)
        {
            maxHeight = height;
        }

        if (hasJumped && height > 0f && height < 1f && maxHeight > 1f) 
        {
            animator.SetBool("Landing", true);
            animator.SetBool("Jump", false);
        }

        if (isSlowingDown)
        {   
            maxSpeed = 0;
            
            currentSpeed = 10f;
            animator.speed = 0.5f;
            transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);

            
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
                    IncreaseSpeed();
                    lastKeyWasLeft = true;
                    lastKeyTime = Time.time;
                }
                else
                {
                    // Schnelle Geschwindigkeitsreduktion, wenn die Tasten nicht korrekt abwechselnd gedrückt wurden
                    DecreaseSpeedRapidly();
                    lastKeyTime = Time.time;
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
                    lastKeyTime = Time.time;
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
            // Normale Beschleunigung
            currentSpeed = Mathf.Min(currentSpeed + acceleration, maxSpeed);

            // Reduzierte Beschleunigung
            if (currentSpeed >= maxSpeed)
            {
                acceleration = 0.25f;  // Sehr langsame Beschleunigung
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
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetBool("Start", false);
            animator.SetBool("Jump", true);
            float currentTime = Time.time;
            float elapsedTime = currentTime - startTime;

            // Startpositiond des Sprungs
            jumpStartPosition = transform.position;

            // Berechnung der Sprungkraft basierend auf der aktuellen Geschwindigkeit
            float jumpSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            // Prüfen, ob Sprung Perfekt ist
            if (perfectJumpOff)
            {
                Jump(jumpSpeed/1.1f);
                Debug.Log("Perfect");
            }
            else
            {
                Jump(jumpSpeed / 1.5f);
            }
            
            hasJumped = true;  // Spieler ist gesprungen
            currentSpeed = 0;
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

            GameObject jumpOffObject = GameObject.FindWithTag("JumpOff");
            // Berechnnung der Sprungweite
            jumpDistance = Vector3.Distance(jumpOffObject.transform.position, transform.position);
            jumpDistance = jumpDistance/5;
            Debug.Log("Sprungweite: " + jumpDistance);

            lastSpeed = jumpDistance/1.75f;
    

            isSlowingDown = true;
            startTime = Time.time;       // Setze die Startzeit für den nächsten Anlauf zurück
            currentSpeed = 0f;
            canBuildSpeed = false;        // kein erneuter Geschwindigkeitsaufbau

            camera.StopFollowingPlayer(); // Kamera fixieren

        }
    }

    private float CalculateCurrentHeight()
    {
        GameObject jumpOffObject = GameObject.FindWithTag("JumpOff");
        // Berechnet die Höhe relativ zum Startpunkt des Sprungs
        return transform.position.y - jumpOffObject.transform.position.y;
    }
    
    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Foul"))
        {
            foul = true;
            Debug.Log("foul");
        }

        if(other.CompareTag("JumpOff"))
        {
            perfectJumpOff = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("JumpOff"))
        {
            perfectJumpOff = false;
        }
    }
}
