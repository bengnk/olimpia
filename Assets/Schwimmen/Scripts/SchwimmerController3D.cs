using UnityEngine;
using UnityEngine.UI;

public class SchwimmerController3D : MonoBehaviour
{
    public float initialMoveAmount = 0.5f;
    public float waterDeceleration = 0.05f;
    public float sizeIncreaseRate = 0.1f;
    public float maxButtonSize = 3f;
    public float minDistanceBetweenButtons = 100f;
    public RectTransform button1;
    public RectTransform button2;
    public RectTransform specialButton;
    public RectTransform canvasRectTransform;
    public float specialButtonAppearanceChance = 0.1f;
    public float specialButtonDuration = 2f;
    public float specialButtonSpeedMultiplier = 0.05f;

    private int lastButtonPressed = 0;
    private float currentSpeed = 0f;
    private bool isSpecialButtonActive = false;
    private float specialButtonTimer = 0f;
    private bool goalReached = false;

    public float jumpForce = 5f;
    public float jumpForwardSpeed = 5f;
    private Rigidbody rb;
    private bool isJumping = false;
    private bool hasTouchedWater = false;

    private Animator animator;

    public GameManager gameManager; // Referenz zum GameManager für die Zeitmessung
    public int swimmerID; // Eindeutige ID für jeden Schwimmer (muss im Inspector gesetzt werden)

    // Kamera-Verfolgung
    public Transform cameraTransform; // Referenz zur Kamera
    public Vector3 cameraOffset = new Vector3(0, 5, -10); // Kameraoffset
    public float cameraFollowSpeed = 5f; // Verfolgungsgeschwindigkeit der Kamera

    private bool canJump = false; // Variable, um zu prüfen, ob der Spieler springen darf

    // Hier die End-Canvas-Referenz hinzufügen
    public GameObject endCanvas;

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

        // Alle Buttons zu Beginn deaktivieren
        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        specialButton.gameObject.SetActive(false);

        // End-Canvas zu Beginn deaktivieren
        if (endCanvas != null)
        {
            endCanvas.SetActive(false);
        }
    }

    void Update()
    {
        // Prüfen, ob der GameManager das "Go!"-Signal gegeben hat
        if (gameManager != null && gameManager.isGoTime && !canJump)
        {
            canJump = true; // Spieler darf jetzt springen
        }

        // Bereits bei "Go!" soll das Springen erlaubt sein
        if (Input.GetMouseButtonDown(0) && canJump && !isJumping && !hasTouchedWater)
        {
            animator.SetBool("jump", true);
            StartJump();
        }

        afterJump();
        AdjustSwimAnimationSpeed();
    }

    private void StartJump()
    {
        isJumping = true;
        rb.velocity = new Vector3(0, jumpForce, jumpForwardSpeed);
        currentSpeed = jumpForwardSpeed;

        // Timer für den Schwimmer im GameManager starten
        gameManager.StartTimer(swimmerID);

        // Buttons anzeigen, sobald der Spieler springt
        button1.gameObject.SetActive(true);
        button2.gameObject.SetActive(true);
        specialButton.gameObject.SetActive(true);
    }

    void afterJump()
    {
        if (currentSpeed > 0)
        {
            transform.position += transform.forward * currentSpeed * Time.deltaTime;
            currentSpeed -= hasTouchedWater ? waterDeceleration * Time.deltaTime : waterDeceleration * 5 * Time.deltaTime;
        }
        else
        {
            currentSpeed = 0;
        }

        IncreaseButtonSize(button1);
        IncreaseButtonSize(button2);
        IncreaseButtonSize(specialButton);

        HandleSpecialButton();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isJumping = false;
            hasTouchedWater = true;
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.velocity = new Vector3(0, 0, currentSpeed);
            animator.SetBool("jump", false);
        }
        else if (other.CompareTag("Ende"))
        {
            goalReached = true;
            currentSpeed = 0;
            animator.SetBool("stop", true);

            // Timer für den Schwimmer im GameManager stoppen
            gameManager.StopTimer(swimmerID);

            // Wenn der gesteuerte Schwimmer (ID 4) das Ziel erreicht
            if (swimmerID == 4)
            {
                // Blende das End-Canvas ein
                if (endCanvas != null)
                {
                    endCanvas.SetActive(true);
                }
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Wand"))
        {
            transform.Rotate(0, 180, 0);
            ResetSpeed();
            currentSpeed = initialMoveAmount;
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
        currentSpeed = initialMoveAmount;
    }

    private void MoveButtonToRandomPosition(RectTransform buttonToMove, RectTransform otherButton)
    {
        Vector2 canvasSize = canvasRectTransform.sizeDelta; // Größe des Canvas
        Vector2 newPosition;
        float distanceToOtherButton;

        // Bestimme die vertikalen Begrenzungen des unteren 3/4 des Canvas
        float minY = -(canvasSize.y / 2) * 0.75f; // Untere Grenze des Canvas (unteres 3/4)
        float maxY = 0;                           // Obere Grenze des unteren 3/4 (Mitte des Canvas)

        // Bestimme die horizontalen Begrenzungen des Canvas
        float minX = -(canvasSize.x / 2) + buttonToMove.rect.width / 2;  // Linke Grenze (mit Berücksichtigung der Button-Breite)
        float maxX = (canvasSize.x / 2) - buttonToMove.rect.width / 2;   // Rechte Grenze (mit Berücksichtigung der Button-Breite)

        do
        {
            // Generiere eine zufällige Position innerhalb der horizontalen und vertikalen Begrenzungen
            newPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));

            // Berechne den Abstand zum anderen Button
            distanceToOtherButton = Vector2.Distance(newPosition, otherButton.anchoredPosition);
        }
        while (distanceToOtherButton < minDistanceBetweenButtons);

        // Setze die neue Position für den Button relativ zum Canvas
        buttonToMove.anchoredPosition = newPosition;
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
    if (!isSpecialButtonActive)
    {
        // Wenn der Special-Button nicht aktiv ist, ihn neu positionieren und aktivieren
        MoveButtonToRandomPosition(specialButton, button1);
        AdjustButtonProperties(specialButton); // Größe zurücksetzen
        specialButton.gameObject.SetActive(true);
        isSpecialButtonActive = true;
        specialButtonTimer = specialButtonDuration; // Startet den Timer für 2 Sekunden
    }
    else
    {
        // Countdown, während der Special-Button aktiv ist
        specialButtonTimer -= Time.deltaTime;
        if (specialButtonTimer <= 0f)
        {
            // Nach Ablauf des Timers den Button deaktivieren und den Zyklus neu starten
            specialButton.gameObject.SetActive(false);
            isSpecialButtonActive = false;
        }
    }
}



    private void AdjustSwimAnimationSpeed()
    {
        if (hasTouchedWater)
        {
            float normalizedSpeed = currentSpeed / jumpForwardSpeed;
            animator.speed = Mathf.Clamp(normalizedSpeed, 0.1f, 1.5f);
        }
        else
        {
            animator.speed = 1.0f;
        }
    }

    void LateUpdate()
    {
        FollowPlayerWithCamera();
    }

    private void FollowPlayerWithCamera()
    {
        if (cameraTransform != null)
        {
            Vector3 targetPosition = transform.position + cameraOffset;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, cameraFollowSpeed * Time.deltaTime);
            cameraTransform.LookAt(transform);
        }
    }
}
