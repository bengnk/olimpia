using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Player2Movement : MonoBehaviour
{
    public float speedP2 = 0f;
    public float acceleration = 1.5f;
    private bool isMoving = false;

    // Timer variables
    public float countdownTime = 5f;
    private bool countdownFinished = false;
    private bool timerStarted = false;
    private float runTimer = 0f;

    // UI references for Up, Down, Left, Right elements
    public Image upElement;
    public Image downElement;
    public Image leftElement;
    public Image rightElement;

    private Image currentElement;
    private string currentKey;

    // Countdown Text UI
    public Text countdownText;  // UI Text for countdown

    public float decelerationTime = 1.5f;
    public bool isFinishedP2 = false;
    private bool canInput = true;

    private string previousKey = "";
    private RaceManager raceManager;
    private Animator animator;
    private int counter = 0;

    void Start()
    {
        animator = GetComponent<Animator>();
        raceManager = FindObjectOfType<RaceManager>(); // Find RaceManager in the scene
        HideAllElements();

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
            if (!timerStarted)
            {
                timerStarted = true;
                runTimer = 0f;
            }

            if (!isFinishedP2)
            {
                runTimer += Time.deltaTime;

                if (currentElement == null)
                {
                    ShowRandomElement();
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
                countdownText.text = "";
            }
            ShowRandomElement();
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

            transform.Translate(Vector3.forward * speedP2 * Time.deltaTime);
            speedP2 *= 0.9999f;
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
            currentElement.color = new Color(1, 1, 1, 1);
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
        upElement.color = new Color(1, 1, 1, 0);
        downElement.color = new Color(1, 1, 1, 0);
        leftElement.color = new Color(1, 1, 1, 0);
        rightElement.color = new Color(1, 1, 1, 0);
    }

    private void CheckInput()
    {
        if (canInput)
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
                if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow) ||
                    Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (currentKey != GetPressedKey())
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
        return "";
    }

    private void GainMomentum()
    {
        IncreaseSpeedP2();
        HideCurrentElement();
        ShowRandomElement();
    }

    private void LoseMomentum()
    {
        speedP2 *= 0.5f;
    }

    private void HideCurrentElement()
    {
        if (currentElement != null)
        {
            currentElement.color = new Color(1, 1, 1, 0);
            currentElement = null;
        }
    }

    public void IncreaseSpeedP2()
    {
        speedP2 += acceleration;
        isMoving = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish") && !isFinishedP2)
        {
            isFinishedP2 = true;
            canInput = false;
            raceManager.PlayerFinished(2, runTimer);
            StartDeceleration();
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
            if (speedP2 < 0) speedP2 = 0;
            yield return null;
        }
        isMoving = false;
        animator.SetBool("cheering", true);
        animator.speed = 1.0f;
    }

    private void UpdateAnimationSpeed()
    {
        animator.speed = Mathf.Clamp(speedP2 / 20f, 0.25f, 1.5f);
    }
}
