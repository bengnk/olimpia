using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class player1Script: MonoBehaviour
{
    public float speedP1 = 0f;
    public float acceleration = 1.5f;
    private bool isMoving = false;

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

    //  Player 2 reference to check if hes finished
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

    void Start()
    {
        Player2Movement = player2.GetComponent<Player2Movement>();
        HideAllElements();
        // Do not show random element at the start
        // ShowRandomElement(); // Removed to delay showing elements until countdown finishes

        // Initialize countdown text to show starting countdown value
        if (countdownText != null)
        {
            countdownText.text = countdownTime.ToString();
        }
    }

    void Update()
    {
        if (!countdownFinished)
        {
            HandleCountdown();
        }
        else
        {
            // Start the run timer if it hasn't started yet
            if (!timerStarted)
            {
                timerStarted = true;
                runTimer = 0f; // Reset run timer
            }

            // Increment the run timer only if the race is not finished
            if (!isFinished || !Player2Movement.isFinishedP2)
            {
                runTimer += Time.deltaTime;

                // Update run timer UI text
                if (runTimerText != null)
                {
                    runTimerText.text = runTimer.ToString("F2") + "s"; // Show total seconds
                }

                // Only show random elements after countdown is finished
                if (currentElement == null)
                {
                    ShowRandomElement(); // Show the first random element
                }
            }

            MoveSquare();
            CheckInput();
        }
    }

    void HandleCountdown()
    {
        countdownTime -= Time.deltaTime;
        if (countdownTime > 0)
        {
            // Update UI Text with remaining countdown time
            if (countdownText != null)
            {
                countdownText.text = Mathf.Ceil(countdownTime).ToString();
            }
        }
        else
        {
            // Countdown finished, show "GO!" for a brief moment
            if (countdownText != null)
            {
                countdownText.text = "GO!";
            }
            countdownFinished = true;

            // Wait for 1 second before clearing the countdown text
            StartCoroutine(ClearCountdownText());
        }
    }

    private IEnumerator ClearCountdownText()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second
        if (countdownText != null)
        {
            countdownText.text = ""; // Clear the countdown text
        }
    }

    void MoveSquare()
    {
        if (isMoving)
        {
            transform.Translate(Vector3.forward * speedP1 * Time.deltaTime);
            speedP1 *= 0.9999f; // Gradually decelerate during the race
        }
    }

    private void ShowRandomElement()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, 4); // Generate a new random index
        } while (GetKeyFromIndex(randomIndex) == previousKey); // Repeat if it matches the previous key

        previousKey = GetKeyFromIndex(randomIndex); // Update the previous key
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
            currentElement.color = new Color(1, 1, 1, 1); // Set opacity to 1 (visible)
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
        wElement.color = new Color(1, 1, 1, 0); // Set opacity to 0 (invisible)
        sElement.color = new Color(1, 1, 1, 0);
        aElement.color = new Color(1, 1, 1, 0);
        dElement.color = new Color(1, 1, 1, 0);
    }

    private void CheckInput()
    {
        if (canInput) // Allow input only if canInput is true
        {
            // Check if the correct key is pressed
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
            // Check for any wrong key press among the relevant keys only
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S) ||
                    Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
            {
                if (currentKey != GetPressedKey()) // Wrong key pressed among WASD
                {
                    LoseMomentum(); // Call LoseMomentum() only if the wrong WASD key is pressed
                }
            }
        }
    }

    // Helper function to get the currently pressed WASD key as a string
    private string GetPressedKey()
    {
        if (Input.GetKeyDown(KeyCode.W)) return "W";
        if (Input.GetKeyDown(KeyCode.S)) return "S";
        if (Input.GetKeyDown(KeyCode.A)) return "A";
        if (Input.GetKeyDown(KeyCode.D)) return "D";
        return ""; // Return empty string if none of the relevant keys are pressed
    }


    private void GainMomentum()
    {
        IncreaseSpeedP1(); // Apply momentum to the cube
        HideCurrentElement();
        ShowRandomElement(); // Show the next random element
    }

    private void LoseMomentum()
    {
        speedP1 *= 0.5f; // Reduce speed by 50%
    }

    private void HideCurrentElement()
    {
        if (currentElement != null)
        {
            currentElement.color = new Color(1, 1, 1, 0); // Set opacity to 0 (invisible)
            currentElement = null; // Clear current element
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
            isFinished = true; // Mark the race as finished
            canInput = false; // Disable input for Player 1
            Debug.Log("Player 1 finished! Time: " + runTimer.ToString("F2") + "s"); // Log Player 2's finish time
            StartDeceleration(); // Start the deceleration process
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
            if (speedP1 < 0) speedP1 = 0; // Ensure speed doesn't go negative
            yield return null;
        }
        isMoving = false; // Stop moving after deceleration
    }
}