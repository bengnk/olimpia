using UnityEngine;
using System.Collections;

public class SchwimmerController3D_Auto : MonoBehaviour
{
    public float initialMoveAmount = 0.5f;
    public float waterDeceleration = 0.05f;
    public float minSpeed = 1f;
    public float maxSpeed = 10f;

    private float currentSpeed = 0f;
    private bool isJumping = false;
    private bool hasTouchedWater = false;
    public float jumpForce = 5f;
    private Rigidbody rb;

    private Animator animator;

    public GameManager gameManager;  // Referenz zum GameManager für die Zeitmessung
    public int swimmerID;  // Eindeutige ID für jeden Schwimmer (muss im Inspector gesetzt werden)

    private bool timerStarted = false; // Flag, um zu prüfen, ob der Timer bereits gestartet wurde

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.isKinematic = false;
        currentSpeed = 0;

        animator = GetComponent<Animator>();

        // Starte den automatischen Sprung nach einer zufälligen Zeit, wenn das Rennen gestartet ist
        StartCoroutine(AutoJump());
    }

    void Update()
    {
        afterJump();
        AdjustSwimAnimationSpeed();
    
    if (gameManager.isGoTime){
                if (!timerStarted)
        {
            gameManager.StartTimer(swimmerID);
            timerStarted = true; // Verhindert mehrfaches Starten des Timers
        }
            }

    }

    private IEnumerator AutoJump()
    {
        while (true)
        {
            // Warte eine zufällige Zeit zwischen 1 und 3 Sekunden, bevor der nächste Sprung erfolgt
            yield return new WaitForSeconds(Random.Range(1f, 3f));
    	    
            // Nur springen und Timer starten, wenn das Rennen begonnen hat
            if (!isJumping && !hasTouchedWater && gameManager.isGoTime)
            {
                animator.SetBool("jump", true);
                StartJump();
            }
        }
    }

    private void StartJump()
    {
        isJumping = true;
        currentSpeed = Random.Range(minSpeed, maxSpeed);  // Zufällige Geschwindigkeit festlegen
        rb.velocity = new Vector3(0, jumpForce, currentSpeed);

        // Timer für den Gegner-Schwimmer im GameManager starten, aber nur einmal
        
    }

    void afterJump()
    {
        if (currentSpeed > 0)
        {
            // Schwimmer bewegen
            transform.position += transform.forward * currentSpeed * Time.deltaTime;

            // Verlangsamen, aber nicht unter eine Mindestgeschwindigkeit fallen lassen
            float deceleration = hasTouchedWater ? waterDeceleration * Time.deltaTime : waterDeceleration * 5 * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed - deceleration, 1f);  // Mindestgeschwindigkeit von 1
        }
        else
        {
            currentSpeed = 0;  // Geschwindigkeit nicht unter 0 fallen lassen
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isJumping = false;
            hasTouchedWater = true;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = new Vector3(0, 0, currentSpeed);  // Setzt die Bewegung im Wasser fort
            animator.SetBool("jump", false);
        }
        else if (other.CompareTag("Wand"))
        {
            // Stoppe die Bewegung, bevor du die Rotation durchführst
            currentSpeed = 0;
            rb.velocity = Vector3.zero;  // Geschwindigkeit auf null setzen

            // Drehe den Schwimmer um 180 Grad
            transform.Rotate(0, 180, 0);

            // Setze die Geschwindigkeit auf den initialen Wert, um die Bewegung in die neue Richtung fortzusetzen
            currentSpeed = initialMoveAmount;
            rb.velocity = transform.forward * currentSpeed;  // Setze neue Geschwindigkeit in Vorwärtsrichtung
        }
        else if (other.CompareTag("Ende"))
        {
            currentSpeed = 0;
            animator.SetBool("stop", true);

            // Timer für den Gegner-Schwimmer im GameManager stoppen
            gameManager.StopTimer(swimmerID);
        }
    }

    private void AdjustSwimAnimationSpeed()
    {
        if (hasTouchedWater)
        {
            float normalizedSpeed = currentSpeed / maxSpeed;
            animator.speed = Mathf.Clamp(normalizedSpeed, 0.1f, 1.5f);
        }
        else
        {
            animator.speed = 1.0f;
        }
    }
}
