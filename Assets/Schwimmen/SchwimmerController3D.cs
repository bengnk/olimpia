using UnityEngine;
using UnityEngine.UI;

public class SchwimmerController3D : MonoBehaviour
{
    public float initialMoveAmount = 0.5f; // Der Ausgangsimpuls, der nach jedem Klick gesetzt wird
    public float waterDeceleration = 0.05f; // Abnahme der Geschwindigkeit pro Sekunde im Wasser
    public float sizeIncreaseRate = 0.1f; // Geschwindigkeit, mit der die Größe der Buttons zunimmt
    public float maxButtonSize = 3f; // Maximale Größe der Buttons
    public float minDistanceBetweenButtons = 100f; // Mindestabstand zwischen den Buttons
    public RectTransform button1; // Referenz zu Button 1
    public RectTransform button2; // Referenz zu Button 2
    public RectTransform specialButton; // Referenz zu Button 3 (spezieller Button)
    public RectTransform canvasRectTransform; // Referenz zum Canvas
    public float specialButtonAppearanceChance = 0.1f; // Wahrscheinlichkeit, dass der spezielle Button erscheint
    public float specialButtonDuration = 2f; // Dauer, für die der spezielle Button sichtbar bleibt
    public float specialButtonSpeedMultiplier = 0.05f; // Faktor, um den die Geschwindigkeit verringert wird

    private int lastButtonPressed = 0; // 0 = kein Button, 1 = Button1, 2 = Button2
    private float currentSpeed = 0f; // Aktuelle Geschwindigkeit der Kapsel
    private bool isSpecialButtonActive = false; // Status des speziellen Buttons
    private float specialButtonTimer = 0f; // Timer für den speziellen Button
    private bool goalReached = false; // Variable, um zu prüfen, ob das Ziel erreicht wurde

    public float jumpForce = 5f;
    public float jumpForwardSpeed = 5f;
    private Rigidbody rb;
    private bool isJumping = false;
    
    private float timerStartTime = 0f;
    private bool timerRunning = false;
    private bool hasTouchedWater = false; // Zustandsvariable, um den Wasserberührungszustand zu verfolgen
    private float elapsedTime = 0f;  // Speichert die verstrichene Zeit nach dem Stopp des Timers
    public Text timerText;  // Fügen Sie die using-Anweisung `using UnityEngine.UI;` oben im Skript hinzu, falls nicht bereits vorhanden.

    private Animator animator; // Animator-Referenz

    void Start(){
        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.isKinematic = false;
        currentSpeed = 0; // Initialisiert currentSpeed

        animator = GetComponent<Animator>(); // Animator-Komponente abrufen
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isJumping && !hasTouchedWater)
        {
            animator.SetBool("jump", true);
            StartJump();
            StartTimer();
        }
        afterJump();
        AdjustSwimAnimationSpeed();
        if (timerRunning)
        {
            UpdateTimerUI();
        }
    }

    private void StartJump()
    {
        isJumping = true;
        rb.velocity = new Vector3(0, jumpForce, jumpForwardSpeed);
        currentSpeed = jumpForwardSpeed; // Setzt die aktuelle Geschwindigkeit
        
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
            elapsedTime = Time.time - timerStartTime;
            timerRunning = false;
            timerText.text = "Endzeit: " + elapsedTime.ToString("F2") + " Sekunden";
            Debug.Log("Zeit gestoppt: " + elapsedTime + " Sekunden.");
        }
    }


    void afterJump(){
        // Wenn die Geschwindigkeit größer als 0 ist, bewege die Kapsel nach vorne
        if (currentSpeed > 0)
        {
            transform.position += transform.forward * currentSpeed * Time.deltaTime;
            // Verwende unterschiedliche Verzögerungen je nachdem, ob die Kapsel im Wasser ist oder nicht
            currentSpeed -= hasTouchedWater ? waterDeceleration * Time.deltaTime : waterDeceleration * 5 * Time.deltaTime;
        }
        else
        {
            currentSpeed = 0; // Geschwindigkeit nicht unter 0 fallen lassen
        }

        // Vergrößere die Buttons kontinuierlich
        IncreaseButtonSize(button1);
        IncreaseButtonSize(button2);
        IncreaseButtonSize(specialButton);

        // Kontrolliere das Erscheinen des speziellen Buttons
        HandleSpecialButton();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isJumping = false;
            hasTouchedWater = true;
            rb.isKinematic = true;
            rb.useGravity = false; // Deaktiviert die Gravitation, wenn das Wasser berührt wird
            rb.velocity = new Vector3(0, 0, currentSpeed); // Setzt die Bewegung im Wasser fort, basierend auf der aktuellen Geschwindigkeit
            animator.SetBool("jump", false);

        
        }
        else if (other.CompareTag("Ende"))
        {
            goalReached = true;  // Setze die Variable, dass das Ziel erreicht wurde
            currentSpeed = 0;
            animator.SetBool("stop", true);
            StopTimer();
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Wand"))
        {
            transform.Rotate(0, 180, 0);

            // Geschwindigkeit zurücksetzen und in die neue Richtung beschleunigen
            ResetSpeed();
            currentSpeed = initialMoveAmount; // Stellen Sie sicher, dass die Kapsel weiterhin bewegt wird
        }
    }

    public void Button1Pressed()
    {
        if (!goalReached && lastButtonPressed != 1)
        {
            ResetSpeed();
            AdjustButtonProperties(button1);
            lastButtonPressed = 1;
            MoveButtonToRandomPosition(button1, button2);
        }
    }

    public void Button2Pressed()
    {
        if (!goalReached && lastButtonPressed != 2)
        {
            ResetSpeed();
            AdjustButtonProperties(button2);
            lastButtonPressed = 2;
            MoveButtonToRandomPosition(button2, button1);
        }
    }

    public void SpecialButtonPressed()
    {
        if (!goalReached)
        {
            currentSpeed *= specialButtonSpeedMultiplier;
            specialButton.gameObject.SetActive(false);
            isSpecialButtonActive = false;
        }
    }

    private void ResetSpeed()
    {
        currentSpeed = initialMoveAmount; // Setze die Geschwindigkeit nach jedem Klick zurück
    }

    private void MoveButtonToRandomPosition(RectTransform buttonToMove, RectTransform otherButton)
    {
        Vector2 canvasSize = canvasRectTransform.sizeDelta;
        Vector2 newPosition;
        float distanceToOtherButton;
        do
        {
            newPosition = new Vector2(Random.Range(buttonToMove.rect.width, canvasSize.x - buttonToMove.rect.width),
                                      Random.Range(buttonToMove.rect.height, canvasSize.y - buttonToMove.rect.height));
            distanceToOtherButton = Vector2.Distance(newPosition, otherButton.anchoredPosition);
        }
        while (distanceToOtherButton < minDistanceBetweenButtons);

        buttonToMove.anchoredPosition = newPosition - (canvasSize / 2);
    }

    private void IncreaseButtonSize(RectTransform button)
    {
        if (button.gameObject.activeSelf && button.localScale.x < maxButtonSize)
        {
            button.localScale += new Vector3(sizeIncreaseRate * Time.deltaTime, sizeIncreaseRate * Time.deltaTime, 0);
        }
    }

    private void AdjustButtonProperties(RectTransform button)
    {
        button.localScale = new Vector3(1f, 1f, 1f);
    }

    private void HandleSpecialButton()
    {
        if (!isSpecialButtonActive && Random.value < specialButtonAppearanceChance * Time.deltaTime)
        {
            MoveButtonToRandomPosition(specialButton, button1);
            specialButton.gameObject.SetActive(true);
            isSpecialButtonActive = true;
            specialButtonTimer = specialButtonDuration;
        }
        else if (isSpecialButtonActive)
        {
            specialButtonTimer -= Time.deltaTime;
            if (specialButtonTimer <= 0f)
            {
                specialButton.gameObject.SetActive(false);
                isSpecialButtonActive = false;
            }
        }
    }

    private void AdjustSwimAnimationSpeed()
    {
        if (hasTouchedWater)
        {
            // Setzt die Animationsgeschwindigkeit proportional zur aktuellen Geschwindigkeit.
            float normalizedSpeed = currentSpeed / jumpForwardSpeed; // Normalisiert auf die Geschwindigkeit des Sprungs
            animator.speed = Mathf.Clamp(normalizedSpeed, 0.1f, 1.5f); // Begrenze die Geschwindigkeit auf einen sinnvollen Bereich
        }
        else
        {
            // Setze die Animationsgeschwindigkeit auf den Standardwert, wenn der Spieler nicht schwimmt.
            animator.speed = 1.0f;
        }
    }
}