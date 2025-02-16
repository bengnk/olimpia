using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SchwimmerController3D : MonoBehaviour
{
    // Alte Variablen (unverändert)
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

    public GameManager gameManager; // Referenz zum GameManager
    public int swimmerID; // Eindeutige ID (z. B. 4 für den Spieler)

    // Kamera-Verfolgung
    public Transform cameraTransform;
    public Vector3 cameraOffset = new Vector3(0, 5, -10);
    public float cameraFollowSpeed = 5f;

    private bool timerStarted = false; // Damit der Timer nur einmal gestartet wird

    public GameObject endCanvas;

    // Variablen für den Charge‑Sprung
    public RectTransform jumpChargeBar; 
    public Image backChargeBar; 
    public Vector2 chargeBarLeftLimit;
    public Vector2 chargeBarRightLimit;
    public float chargeBarSpeed = 1f;
    public float minJumpMultiplier = 3f;
    public float maxJumpMultiplier = 6f;
    private float currentCharge = 0f;
    private float chargeDirection = 1f;
    private bool isCharging = false;  // Aktiv während des Ladevorgangs

    // Speichert die ausgewählte Sprungstärke
    private bool jumpSelected = false;
    private float storedJumpMultiplier = 0f;

    public bool test = false;
    public bool activation = false;

    // Verhindert Mehrfachdrehungen an der Wand
    private bool hasTurnedAtWall = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        currentSpeed = 0;
        animator = GetComponent<Animator>();

        button1.gameObject.SetActive(false);
        button2.gameObject.SetActive(false);
        specialButton.gameObject.SetActive(false);
        if (endCanvas != null)
            endCanvas.SetActive(false);
        if (jumpChargeBar != null)
            jumpChargeBar.gameObject.SetActive(false);
            backChargeBar.gameObject.SetActive(false);
    }

    void Update()
    {
        // Charge‑Mechanismus (Startet den Countdown und wählt die Sprungstärke)
        if (gameManager != null && gameManager.gameStarted && !gameManager.countdownStarted && !isJumping && !hasTouchedWater && !jumpSelected)
        {
            if (!isCharging)
            {
                isCharging = true;
                currentCharge = 0f;
                chargeDirection = 1f;
                if (jumpChargeBar != null)

                    jumpChargeBar.gameObject.SetActive(true);
                    backChargeBar.gameObject.SetActive(true);
            }
            currentCharge += chargeDirection * chargeBarSpeed * Time.deltaTime;
            if (currentCharge >= 1f)
            {
                currentCharge = 1f;
                chargeDirection = -1f;
            }
            else if (currentCharge <= 0f)
            {
                currentCharge = 0f;
                chargeDirection = 1f;
            }
            if (jumpChargeBar != null)
                jumpChargeBar.anchoredPosition = Vector2.Lerp(chargeBarLeftLimit, chargeBarRightLimit, currentCharge);
            if (Input.GetMouseButtonDown(0))
            {
                storedJumpMultiplier = Mathf.Lerp(minJumpMultiplier, maxJumpMultiplier, currentCharge);
                jumpSelected = true;
                isCharging = false;
                test = true;
                if (jumpChargeBar != null)
                    jumpChargeBar.gameObject.SetActive(false);
                    backChargeBar.gameObject.SetActive(false);
                if (gameManager != null && !gameManager.countdownStarted)
                    gameManager.StartCountdown();
            }
        }

        if (gameManager != null && gameManager.isGoTime && jumpSelected && !isJumping)
        {
            activation = true;
            PerformJump();
        }

        afterJump();
        AdjustSwimAnimationSpeed();
    }

    private void PerformJump()
    {
        // Starte den Timer genau beim Sprung – nur einmal
       // if (gameManager != null && !timerStarted)
       // {
           // gameManager.StartTimer(swimmerID);
         //   timerStarted = true;
       // }
        isJumping = true;
        rb.velocity = new Vector3(0, jumpForce * storedJumpMultiplier, jumpForwardSpeed * storedJumpMultiplier);
        currentSpeed = jumpForwardSpeed * storedJumpMultiplier;
        animator.SetBool("jump", true);
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
            gameManager.StopTimer(swimmerID);
            if (swimmerID == 4 && endCanvas != null)
                endCanvas.SetActive(true);
        }
        else if (other.CompareTag("Wand"))
        {
            hasTurnedAtWall = false;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Wand"))
        {
            if (!hasTurnedAtWall)
            {
                transform.Rotate(0, 180, 0);
                hasTurnedAtWall = true;
            }
            ResetSpeed();
            currentSpeed = initialMoveAmount;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wand"))
            hasTurnedAtWall = false;
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
        Vector2 canvasSize = canvasRectTransform.sizeDelta;
        Vector2 newPosition;
        float distanceToOtherButton;
        float minY = -(canvasSize.y / 2) * 0.75f;
        float maxY = 0;
        float minX = -(canvasSize.x / 2) + buttonToMove.rect.width / 2;
        float maxX = (canvasSize.x / 2) - buttonToMove.rect.width / 2;
        do
        {
            newPosition = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            distanceToOtherButton = Vector2.Distance(newPosition, otherButton.anchoredPosition);
        }
        while (distanceToOtherButton < minDistanceBetweenButtons);
        buttonToMove.anchoredPosition = newPosition;
    }

    private void IncreaseButtonSize(RectTransform button)
    {
        if (button.gameObject.activeSelf && button.localScale.x < maxButtonSize)
        {
            float newSize = Mathf.Min(button.localScale.x + sizeIncreaseRate * Time.deltaTime, maxButtonSize);
            button.localScale = new Vector3(newSize, newSize, 0);
        }
    }

    private void AdjustButtonProperties(RectTransform button)
    {
        button.localScale = new Vector3(1f, 1f, 1f);
    }

   private void HandleSpecialButton()
{
    // Der Special Button erscheint nur, wenn das Rennen gestartet wurde.
    if (gameManager == null || !gameManager.isGoTime)
    {
        specialButton.gameObject.SetActive(false);
        return;
    }

    if (!isSpecialButtonActive)
    {
        // Special Button aktivieren, positionieren und initialisieren
        specialButton.gameObject.SetActive(true);
        MoveButtonToRandomPosition(specialButton, button1);
        AdjustButtonProperties(specialButton);
        isSpecialButtonActive = true;
        specialButtonTimer = specialButtonDuration;
    }
    else
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
