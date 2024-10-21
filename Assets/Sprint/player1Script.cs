using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class player1Script: MonoBehaviour
{
    public float speedP1 = 0f;
    public float acceleration = 1.5f;
    private bool isMoving = false;
    private bool singleplayer = false;

    // Timer variables
    public float countdownTime = 5f; // Fixed to 5 seconds
    private bool countdownFinished = false;
    private bool timerStarted = false;
    private float runTimer = 0f; // Timer for run duration

    // UI references for W, S, A, D elements
    public Image wElement;
    public Image sElement;
    public Image aElement;
    public Image dElement;

    // Player 2 reference to check if he has finished
    public GameObject player2;
    private Player2Movement Player2Movement;

    private Image currentElement; // Keep track of the currently displayed element
    private string currentKey; // Store the currently active key
    private string previousKey = ""; // Store the previously active key

    // UI Text for run timer and countdown
    public Text runTimerText;  // Reference to the UI Text element for run timer
    public Text countdownText;  // Reference to the UI Text element for countdown

    // Deceleration settings
    public float decelerationTime = 1.5f; // Time to decelerate to zero after finish
    private bool isFinished = false; // Track if the race is finished

    // Flag to prevent input
    private bool canInput = true;

    // Reference to Race Manager
    private RaceManager raceManager;

    private Animator animator;
    private int counter = 0;

    public GameObject countdownBackground;

    public Camera player1Camera;
    public Camera player2Camera;

    void Start()
    {
        animator = GetComponent<Animator>(); 
        Player2Movement = player2.GetComponent<Player2Movement>();
        raceManager = FindObjectOfType<RaceManager>(); // Find RaceManager in the scene
        Color tempColor = runTimerText.color;
        tempColor.a = 0f; // Setzt den Alpha-Wert auf 0 (vollständig transparent)
        runTimerText.color = tempColor;
        HideAllElements();

        if (countdownText != null)
        {
            countdownText.text = countdownTime.ToString();
        }
    }

    void Update()
    {
        if(singleplayer) {
            player1Camera.enabled = false;
            player2Camera.rect = new Rect(0f, 0f, 1f, 1f);
        } else {
            player1Camera.enabled = true;
            player2Camera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
        }

        if (!countdownFinished)
        {
            HandleCountdown();
        }
        else
        {
            if (!timerStarted)
            {
                timerStarted = true;
                runTimer = 0f; // Reset run timer
            }

            if (!isFinished || !Player2Movement.isFinishedP2)
            {
                runTimer += Time.deltaTime;

                if (runTimerText != null)
                {
                    runTimerText.text = runTimer.ToString("F2") + "s"; // Show total seconds
                }

                if (currentElement == null)
                {
                    ShowRandomElement(); // Show the first random element
                }
            }

            if(!singleplayer) {
                MoveSquare();
                CheckInput();
            } else {
                MoveSquare();
                HideAllElements();
                float randomValue = Random.Range(0f, 1f);

                if(canInput) {
                    // 80% chance to gain momentum
                    if (randomValue <= 0.8f)
                    {
                        GainMomentum();
                    }
                    // 20% chance to lose momentum
                    else
                    {
                        LoseMomentum();
                    }
                }
            } 
        }
    }

    void HandleCountdown()
    {
        countdownTime -= Time.deltaTime;
        if (countdownTime > 0)
        {
            if (countdownText != null)
            {
                countdownText.text = Mathf.Ceil(countdownTime).ToString();
            }
        }
        else
        {
            if (countdownText != null)
            {
                countdownText.text = "GO!";
            }
            countdownFinished = true;

            StartCoroutine(ClearCountdownText());
        }
    }

    private IEnumerator ClearCountdownText()
    {
        yield return new WaitForSeconds(1f); 
        if (countdownText != null)
        {
            countdownText.text = ""; 
            Color tempColor = runTimerText.color;
            tempColor.a = 1f; // Setzt den Alpha-Wert auf 0 (vollständig transparent)
            runTimerText.color = tempColor;
    
        }
    }

    void MoveSquare()
    {
        if (isMoving)
        {
            counter += 1;
            if (counter == 1) {
                animator.SetBool("crouched", true);
                animator.speed = 0.25f;
            } else {
                UpdateAnimationSpeed();
            }

            transform.Translate(Vector3.forward * speedP1 * Time.deltaTime);
            speedP1 *= 0.9999f; 
        }
    }

    private void ShowRandomElement()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, 4); 
        } while (GetKeyFromIndex(randomIndex) == previousKey);

        previousKey = GetKeyFromIndex(randomIndex); 
        HideAllElements();

        switch (randomIndex)
        {
            case 0:
                currentElement = wElement;
                currentKey = "W";
                break;
            case 1:
                currentElement = sElement;
                currentKey = "S";
                break;
            case 2:
                currentElement = aElement;
                currentKey = "A";
                break;
            case 3:
                currentElement = dElement;
                currentKey = "D";
                break;
        }

        if (currentElement != null)
        {
            currentElement.color = new Color(1, 1, 1, 1);
        }
    }

    private string GetKeyFromIndex(int index)
    {
        switch (index)
        {
            case 0: return "W";
            case 1: return "S";
            case 2: return "A";
            case 3: return "D";
            default: return "";
        }
    }

    private void HideAllElements()
    {
        wElement.color = new Color(1, 1, 1, 0);
        sElement.color = new Color(1, 1, 1, 0);
        aElement.color = new Color(1, 1, 1, 0);
        dElement.color = new Color(1, 1, 1, 0);
    }

    private void CheckInput()
    {
        if (canInput)
        {
            if (Input.GetKeyDown(KeyCode.W) && currentKey == "W")
            {
                GainMomentum();
            }
            else if (Input.GetKeyDown(KeyCode.S) && currentKey == "S")
            {
                GainMomentum();
            }
            else if (Input.GetKeyDown(KeyCode.A) && currentKey == "A")
            {
                GainMomentum();
            }
            else if (Input.GetKeyDown(KeyCode.D) && currentKey == "D")
            {
                GainMomentum();
            }
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) ||
                    Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                if (currentKey != GetPressedKey())
                {
                    LoseMomentum();
                }
            }
        }
    }

    private string GetPressedKey()
    {
        if (Input.GetKeyDown(KeyCode.W)) return "W";
        if (Input.GetKeyDown(KeyCode.S)) return "S";
        if (Input.GetKeyDown(KeyCode.A)) return "A";
        if (Input.GetKeyDown(KeyCode.D)) return "D";
        return "";
    }

    private void GainMomentum()
    {
        if(!singleplayer) {
            HideCurrentElement();
            ShowRandomElement();
        }
        IncreaseSpeedP1();
    }

    private void LoseMomentum()
    {
        speedP1 *= 0.5f;
    }

    private void HideCurrentElement()
    {
        if (currentElement != null)
        {
            currentElement.color = new Color(1, 1, 1, 0);
            currentElement = null;
        }
    }

    public void IncreaseSpeedP1()
    {
        speedP1 += acceleration;
        isMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish") && !isFinished)
        {
            isFinished = true;
            canInput = false;
            raceManager.PlayerFinished(1, runTimer);
            StartDeceleration();
        }
    }

    public void StartDeceleration()
    {
        StartCoroutine(DecelerateToZero());
    }

    private IEnumerator DecelerateToZero()
    {
        float initialSpeedP1 = speedP1;
        float decelerationRate = initialSpeedP1 / decelerationTime;

        while (speedP1 > 0)
        {
            speedP1 -= decelerationRate * Time.deltaTime;
            if (speedP1 < 0) speedP1 = 0;
            yield return null;
        }
        isMoving = false;
        animator.SetBool("cheering", true);
        animator.speed = 1.0f;
    }

    private void UpdateAnimationSpeed()
    {
        animator.speed = Mathf.Clamp(speedP1 / 20f, 0.25f, 1.5f);
    }
}
