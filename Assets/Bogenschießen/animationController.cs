using UnityEngine;

public class BowAnimatorController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        // Holt den Animator des GameObjects
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Überprüft, ob die linke Maustaste gedrückt wird
        if (Input.GetMouseButtonDown(0)) // 0 steht für die linke Maustaste
        {
            // Setzt die Animator-Variable startBowWire auf false
            animator.SetBool("startBowWire", false);
            Debug.Log("startBowWire wurde auf false gesetzt.");
        }
    }
}
