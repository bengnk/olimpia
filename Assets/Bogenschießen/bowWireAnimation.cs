using UnityEngine;

public class BowWireController : MonoBehaviour
{
    public Animator animator;

    void Update()
    {
        // Wenn die linke Maustaste gedrückt gehalten wird
        if (Input.GetMouseButton(0))
        {
            animator.SetBool("shot", true);
            animator.speed = 0.5f;
        }
        // Wenn die linke Maustaste losgelassen wird
        else if (Input.GetMouseButtonUp(0))
        {
            animator.SetBool("shot", false);
        }
    }
}
