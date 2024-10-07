using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player2Movement : MonoBehaviour
{
    public float speedP2 = 0f;
    public float acceleration = 1.5f;
    private bool isMoving = false;

    // Timer variables
    public float countdownTime = 5f;  // Fixed to 5 seconds
    private bool countdownFinished = false;
    private bool timerStarted = false;
    private float runTimer = 0f;  // Timer for run duration

    // UI references for Up, Down, Left, Right elements
    public Image upElement;
    public Image downElement;
    public Image leftElement;
    public Image rightElement;

    private Image currentElement; // Keep track of the currently displayed element
    private string currentKey; // Store the currently active key

    // Deceleration settings
    public float decelerationTime = 1.5f; // Time to decelerate to zero after finish
    public bool isFinishedP2 = false; // Track if the race is finished

    // Flag to prevent input
    private bool canInput = true;

    private string previousKey = "";

    // UI Text for countdown (optional, if you want to display it for Player 2)
    public Text countdownText;

    void Start()
    {
        HideAllElements();
        // Initialize countdown text to show starting countdown value (if using a countdown UI for Player 2)
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
            if (!isFinishedP2)
            {
                runTimer += Time.deltaTime;

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
            // Update countdown UI Text for Player 2 (optional)
            if (countdownText != null)
            {
                countdownText.text = Mathf.Ceil(countdownTime).ToString();
            }
        }
        else
        {
            countdownFinished = true;
            if (countdownText != null)
            {
                countdownText.text = ""; // Clear the countdown text after it's done
            }
            ShowRandomElement(); // Show random elements after countdown finishes
        }
    }

    void MoveSquare()
    {
        if (isMoving)
        {
            transform.Translate(Vector3.forward * speedP2 * Time.deltaTime);
            speedP2 *= 0.9999f; // Gradually decelerate during the race
        }
    }

    private void ShowRandomElement()
    {
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, 4);
        } while (GetKeyByIndex(randomIndex) == previousKey);

        previousKey = GetKeyByIndex(randomIndex);
        HideAllElements();

        switch (randomIndex)
        {
            case 0:
                currentElement = upElement;
                currentKey = "UpArrow";
                break;
            case 1:
                currentElement = downElement;
                currentKey = "DownArrow";
                break;
            case 2:
                currentElement = leftElement;
                currentKey = "LeftArrow";
                break;
            case 3:
                currentElement = rightElement;
                currentKey = "RightArrow";
                break;
        }

        if (currentElement != null)
        {
            currentElement.color = new Color(1, 1, 1, 1); // Set opacity to 1 (visible)
        }
    }

    private string GetKeyByIndex(int index)
    {
        switch (index)
        {
            case 0: return "UpArrow";
            case 1: return "DownArrow";
            case 2: return "LeftArrow";
            case 3: return "RightArrow";
            default: return "";
        }
    }

    private void HideAllElements()
    {
        upElement.color = new Color(1, 1, 1, 0); // Set opacity to 0 (invisible)
        downElement.color = new Color(1, 1, 1, 0);
        leftElement.color = new Color(1, 1, 1, 0);
        rightElement.color = new Color(1, 1, 1, 0);
    }

    private void CheckInput()
    {
        if (canInput) // Allow input only if canInput is true
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) && currentKey == "UpArrow")
            {
                GainMomentum();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && currentKey == "DownArrow")
            {
                GainMomentum();
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) && currentKey == "LeftArrow")
            {
                GainMomentum();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) && currentKey == "RightArrow")
            {
                GainMomentum();
            }
            else if (Input.anyKeyDown)
            {
                // Check if the pressed key is one of the arrow keys
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) || 
                    Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (currentKey != GetPressedKey()) // If it's a valid key but wrong one
                    {
                        LoseMomentum();
                    }
                }
            }
        }
    }


    private string GetPressedKey()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) return "UpArrow";
        if (Input.GetKeyDown(KeyCode.DownArrow)) return "DownArrow";
        if (Input.GetKeyDown(KeyCode.LeftArrow)) return "LeftArrow";
        if (Input.GetKeyDown(KeyCode.RightArrow)) return "RightArrow";
        return ""; // Return an empty string if none of the expected keys are pressed
    }

    private void GainMomentum()
    {
        IncreaseSpeedP2(); // Apply momentum to the cube
        HideCurrentElement();
        ShowRandomElement(); // Show the next random element
    }

    private void HideCurrentElement()
    {
        if (currentElement != null)
        {
            currentElement.color = new Color(1, 1, 1, 0); // Set opacity to 0 (invisible)
            currentElement = null; // Clear current element
        }
    }

    public void IncreaseSpeedP2()
    {
        speedP2 += acceleration;
        isMoving = true;
    }

    public void LoseMomentum()
    {
        speedP2 *= 0.5f; // Reduce speed by 50%
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish") && !isFinishedP2)
        {
            isFinishedP2 = true; // Mark the race as finished
            canInput = false; // Disable input for Player 2
            Debug.Log("Player 2 finished! Time: " + runTimer.ToString("F2") + "s"); // Log Player 2's finish time
            StartDeceleration(); // Start the deceleration process
        }
    }

    public void StartDeceleration()
    {
        StartCoroutine(DecelerateToZero());
    }

    private IEnumerator DecelerateToZero()
    {
        float initialSpeedP2 = speedP2;
        float decelerationRate = initialSpeedP2 / decelerationTime;

        while (speedP2 > 0)
        {
            speedP2 -= decelerationRate * Time.deltaTime;
            if (speedP2 < 0) speedP2 = 0; // Ensure speed doesn't go negative
            yield return null;
        }
        isMoving = false; // Stop moving after deceleration
    }
}
