using UnityEngine;
using UnityEngine.UI;  // Für den Zugriff auf UI-Elemente wie Text

public class LongJump3D : MonoBehaviour
{
    public float maxSpeed = 35f;               
    public int clicksToMaxSpeed = 100;         
    public float rapidDeceleration = 2f;       
    public float deceleration = 0.5f;          
    public float jumpForce = 10f;              
    public float perfectTimingWindow = 0.5f;   
    public float timingRange = 2f;             

    private Rigidbody rb;                      
    private bool hasJumped = false;            
    private Vector3 jumpStartPosition;         
    private float startTime;                   
    private float currentSpeed = 0f;           
    private bool lastKeyWasLeft = false;       
    private float lastKeyTime;                 
    private bool canBuildSpeed = true;         
    private float acceleration;                
    private bool foul = false;
    private bool perfectJumpOff = false;
    private bool spaceClicked = false;
    bool isSlowingDown = false;
    float jumpDistance;
    float maxHeight = 0;

    private Animator animator;
    private CameraFollow camera;

    // UI Elemente
    public Canvas resultsCanvas;         // Das Canvas für die Sprungergebnisse
    public Text jumpDistanceText;        // Der Text, der die Sprungweite anzeigt

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startTime = Time.time;
        lastKeyTime = Time.time;

        camera = Camera.main.GetComponent<CameraFollow>();

        acceleration = maxSpeed / clicksToMaxSpeed;
        animator = GetComponent<Animator>();

        // Stelle sicher, dass das Canvas zu Beginn nicht sichtbar ist
        if (resultsCanvas != null)
        {
            resultsCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        float minSpeed = 0.1f;   
        float maxSpeed = 30f;    
        float minAnimationSpeed = 0.5f; 
        float maxAnimationSpeed = 1.2f; 

        if (currentSpeed > 0f)
        {
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
                    IncreaseSpeed();
                    lastKeyWasLeft = true;
                    lastKeyTime = Time.time;
                }
                else
                {
                    DecreaseSpeedRapidly();
                    lastKeyTime = Time.time;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (lastKeyWasLeft && Time.time - lastKeyTime < 0.5f)
                {
                    IncreaseSpeed();
                    lastKeyWasLeft = false;
                    lastKeyTime = Time.time;
                }
                else
                {
                    DecreaseSpeedRapidly();
                    lastKeyTime = Time.time;
                }
            }
        }

        if (Time.time - lastKeyTime > 0.5f && canBuildSpeed)
        {
            currentSpeed = Mathf.Max(currentSpeed - rapidDeceleration * Time.deltaTime, 0);
        }
    }

    private void IncreaseSpeed()
    {
        if (currentSpeed < maxSpeed)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration, maxSpeed);
            if (currentSpeed >= maxSpeed)
            {
                acceleration = 0.25f;
            }
        }
    }

    private void DecreaseSpeedRapidly()
    {
        currentSpeed = Mathf.Max(currentSpeed - rapidDeceleration * Time.deltaTime, 0);
    }

    private void Move()
    {
        transform.Translate(Vector3.forward * currentSpeed * Time.deltaTime);
    }

    private void CheckJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && spaceClicked == false)
        {
            animator.SetBool("Start", false);
            animator.SetBool("Jump", true);
            float currentTime = Time.time;
            float elapsedTime = currentTime - startTime;

            jumpStartPosition = transform.position;
            float jumpSpeed = Mathf.Clamp(currentSpeed, 0, maxSpeed);

            if (perfectJumpOff)
            {
                Jump(jumpSpeed / 100f);
                Debug.Log("Perfect");
            }
            else
            {
                Jump(jumpSpeed / 150f);
            }
            spaceClicked = true;
        }
    }

    private void Jump(float speed)
    {
        rb.velocity = new Vector3(0, jumpForce, speed);
    }

   void OnCollisionEnter(Collision collision)
    {
        Results(collision);
    }

    private void Results(Collision collision = null) {
        if (collision==null) { // wenn nicht gesprungen wurde
            jumpDistance = 0;
            isSlowingDown = true;
            startTime = Time.time;
            currentSpeed = 0f;
            canBuildSpeed = false;
            camera.StopFollowingPlayer();

            // Übergibt die Sprungweite und die Gegnerergebnisse an das JumpResultDisplay-Skript
            JumpResultDisplay jumpResultDisplay = FindObjectOfType<JumpResultDisplay>();
            if (jumpResultDisplay != null)
            {
                LongJump longJump = FindObjectOfType<LongJump>();  // Gegnerergebnisse holen
                if (longJump != null)
                {
                    jumpResultDisplay.ShowJumpResults(jumpDistance, longJump.GetResults());  // Ergebnisse anzeigen
                }
            }
        }

        if (spaceClicked && collision.gameObject.CompareTag("Ground")) // wenn gesprungen wurde
        {
            GameObject jumpOffObject = GameObject.FindWithTag("JumpOff");
            animator.SetBool("Landing", true);
            animator.SetBool("Jump", false);

            // Berechnung der Sprungweite
            jumpDistance = Vector3.Distance(jumpOffObject.transform.position, transform.position) / 5;
            if (foul)
            {
                jumpDistance = 0;
            }
            Debug.Log("Sprungweite: " + jumpDistance);

            isSlowingDown = true;
            startTime = Time.time;
            currentSpeed = 0f;
            canBuildSpeed = false;
            camera.StopFollowingPlayer();

            // Übergibt die Sprungweite und die Gegnerergebnisse an das JumpResultDisplay-Skript
            JumpResultDisplay jumpResultDisplay = FindObjectOfType<JumpResultDisplay>();
            if (jumpResultDisplay != null)
            {
                LongJump longJump = FindObjectOfType<LongJump>();  // Gegnerergebnisse holen
                if (longJump != null)
                {
                    jumpResultDisplay.ShowJumpResults(jumpDistance, longJump.GetResults());  // Ergebnisse anzeigen
                }
            }
            
        } else if (spaceClicked && collision.gameObject.CompareTag("Before")) {
            spaceClicked = false;
            currentSpeed = 5f;
            animator.SetBool("Jump", false);
        }
        
    }



    private void DisplayResultsCanvas()
    {
        if (resultsCanvas != null)
        {
            // Sprungweite im Text anzeigen
            jumpDistanceText.text = "Sprungweite: " + jumpDistance.ToString("F2") + " m";
            resultsCanvas.gameObject.SetActive(true);
        }
    }

    private float CalculateCurrentHeight()
    {
        GameObject jumpOffObject = GameObject.FindWithTag("JumpOff");
        return transform.position.y - jumpOffObject.transform.position.y;
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Foul"))
        {
            foul = true;
            Debug.Log("foul");
        }

        if (other.CompareTag("JumpOff"))
        {
            perfectJumpOff = true;
        }
        
        if (other.CompareTag("Finish")){
            Results();
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
