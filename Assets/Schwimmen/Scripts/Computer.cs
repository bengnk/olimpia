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

    public GameManager gameManager;  // Referenz zum GameManager
    public int swimmerID;  // Eindeutige ID für diesen Schwimmer

    private bool timerStarted = false; // Damit der Timer nur einmal gestartet wird

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
        currentSpeed = 0;
        animator = GetComponent<Animator>();
        StartCoroutine(AutoJump());
    }

    void Update()
    {
        afterJump();
        AdjustSwimAnimationSpeed();
        // Hier wird NICHT im Update der Timer gestartet – das erfolgt in StartJump()
    }

    private IEnumerator AutoJump()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            if (!isJumping && !hasTouchedWater && gameManager.activation)
            {
                animator.SetBool("jump", true);
                StartJump();
            }
        }
    }

    private void StartJump()
    {
        // Timer nur einmal direkt beim Sprung
       // if (gameManager != null && !timerStarted)
      //  {
       //     gameManager.StartTimer(swimmerID);
      //      timerStarted = true;
      //  }
        isJumping = true;
        currentSpeed = Random.Range(minSpeed, maxSpeed);
        rb.velocity = new Vector3(0, jumpForce, currentSpeed);
    }

    void afterJump()
    {
        if (currentSpeed > 0)
        {
            transform.position += transform.forward * currentSpeed * Time.deltaTime;
            float deceleration = hasTouchedWater ? waterDeceleration * Time.deltaTime : waterDeceleration * 5 * Time.deltaTime;
            currentSpeed = Mathf.Max(currentSpeed - deceleration, 1f);
        }
        else
        {
            currentSpeed = 0;
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
            rb.velocity = new Vector3(0, 0, currentSpeed);
            animator.SetBool("jump", false);
        }
        else if (other.CompareTag("Wand"))
        {
            currentSpeed = 0;
            rb.velocity = Vector3.zero;
            transform.Rotate(0, 180, 0);
            currentSpeed = initialMoveAmount;
            rb.velocity = transform.forward * currentSpeed;
        }
        else if (other.CompareTag("Ende"))
        {
            currentSpeed = 0;
            animator.SetBool("stop", true);
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
