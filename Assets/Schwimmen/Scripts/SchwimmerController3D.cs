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

    private float timerStartTime = 0f;
    private bool timerRunning = false;
    private bool hasTouchedWater = false;
    private float elapsedTime = 0f;
    public Text timerText;

    private Animator animator;

    // Kamera-Verfolgung
    public Transform cameraTransform; // Referenz zur Kamera
    public Vector3 cameraOffset = new Vector3(0, 5, -10); // Kameraoffset
    public float cameraFollowSpeed = 5f; // Verfolgungsgeschwindigkeit der Kamera

    void Start(){
        rb = GetComponent<Rigidbody>();
        if (rb == null) {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = true;
        rb.isKinematic = false;
        currentSpeed = 0;

        animator = GetComponent<Animator>();
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
        currentSpeed = jumpForwardSpeed;
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
            StopTimer();
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
