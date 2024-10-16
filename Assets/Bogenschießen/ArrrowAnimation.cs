using UnityEngine;

public class BowWireAndArrowController : MonoBehaviour
{
    public Animator bowWireAnimator;
    public GameObject arrow;  // Referenz zum Pfeil
    private Vector3 arrowStartPosition = new Vector3(0.003253579f, 0.002195037f, 0.07041485f);
    public float resetDelay = 2f; // 1 Millisekunde = 0.001 Sekunden

    void Update()
    {
        // Wenn die linke Maustaste gedrückt gehalten wird
        if (Input.GetMouseButton(0))
        {
            bowWireAnimator.SetBool("startArrow", true);
            bowWireAnimator.speed = 0.5f;
        }
        // Wenn die linke Maustaste losgelassen wird
        else if (Input.GetMouseButtonUp(0))
        {
            bowWireAnimator.SetBool("startArrow", false);
            Invoke("ResetArrowPosition", resetDelay); // Verzögerung von 1 ms
        }
    }

    // Setzt den Pfeil nach dem Delay zurück an seine Startposition
    void ResetArrowPosition()
    {
        arrow.transform.localPosition = arrowStartPosition;
    }
}
