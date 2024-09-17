using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SchwimmerController3D_Auto : MonoBehaviour
{
    public float initialMoveAmount = 0.5f; // Der Ausgangsimpuls, der nach jedem Sprung gesetzt wird
    public float waterDeceleration = 0.05f; // Abnahme der Geschwindigkeit pro Sekunde im Wasser
    public float minSpeed = 1f; // Minimale Sprunggeschwindigkeit
    public float maxSpeed = 10f; // Maximale Sprunggeschwindigkeit

    private float currentSpeed = 0f; // Aktuelle Geschwindigkeit des Schwimmers
    private bool isJumping = false;
    private bool hasTouchedWater = false; // Zustandsvariable für Wasserberührung
    public float jumpForce = 5f;
    private Rigidbody rb;
    
    private Animator animator; // Animator-Referenz

    private float timerStartTime = 0f;
    private bool timerRunning = false;
    public Text timerText;  // Text-Objekt zur Anzeige der Zeit

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.isKinematic = false;
        currentSpeed = 0; // Initialisiert currentSpeed

        animator = GetComponent<Animator>(); // Animator-Komponente abrufen

        // Startet den automatischen Sprung nach einer zufälligen Zeit
        StartCoroutine(AutoJump());
    }

    void Update()
    {
        afterJump();
        AdjustSwimAnimationSpeed();
        if (timerRunning)
        {
            UpdateTimerUI();
        }
    }

    private IEnumerator AutoJump()
    {
        while (true)
        {
            // Warte eine zufällige Zeit zwischen 1 und 3 Sekunden, bevor der nächste Sprung erfolgt
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            // Starte den Sprung, wenn der Schwimmer nicht bereits springt und das Wasser nicht berührt hat
            if (!isJumping && !hasTouchedWater)
            {
                animator.SetBool("jump", true);
                StartJump();
                StartTimer();
            }
        }
    }

    private void StartJump()
    {
        isJumping = true;
        currentSpeed = Random.Range(minSpeed, maxSpeed); // Zufällige Geschwindigkeit festlegen
        rb.velocity = new Vector3(0, jumpForce, currentSpeed);
    }

    private void StartTimer()
    {
        timerStartTime = Time.time;
        timerRunning = true;
    }

    private void UpdateTimerUI()
    {
        float timeSinceStart = Time.time - timerStartTime;
        timerText.text = "Zeit: " + timeSinceStart.ToString("F2") + " Sekunden";
    }

    private void StopTimer()
    {
        if (timerRunning)
        {
            float elapsedTime = Time.time - timerStartTime;
            timerRunning = false;
            timerText.text = "Endzeit: " + elapsedTime.ToString("F2") + " Sekunden";
            Debug.Log("Zeit gestoppt: " + elapsedTime + " Sekunden.");
        }
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
            currentSpeed = 0; // Geschwindigkeit nicht unter 0 fallen lassen
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
            rb.velocity = new Vector3(0, 0, currentSpeed); // Setzt die Bewegung im Wasser fort
            animator.SetBool("jump", false);
        }
        if (other.CompareTag("Wand"))
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
            StopTimer();
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
