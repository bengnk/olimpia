using System.Collections;
using UnityEngine;

public class RandomAnimationSwitcher : MonoBehaviour
{
    private Animator animator;
    public float minSwitchTime = 2.0f;  // Minimale Zeit, bevor die Animation gewechselt wird
    public float maxSwitchTime = 5.0f;  // Maximale Zeit, bevor die Animation gewechselt wird
    private bool isAnimation1Playing;  // Bool für den aktuellen Animationszustand
    private int animation2PlayCount = 0;  // Zählt, wie oft Animation 2 gespielt wurde

    void Start()
    {
        animator = GetComponent<Animator>();

        // Zufällig auswählen, welche Animation zuerst gespielt wird
        isAnimation1Playing = Random.Range(0, 2) == 0;

        // Startet die zufällige Animation
        if (isAnimation1Playing)
        {
            animator.Play("Animation1");  // Startet Animation 1
        }
        else
        {
            animator.Play("Animation2");  // Startet Animation 2
        }

        // Starte den Animationenwechsel
        StartCoroutine(SwitchAnimation());
    }

    IEnumerator SwitchAnimation()
    {
        while (true)
        {
            float randomTime = Random.Range(minSwitchTime, maxSwitchTime);  // Zufällige Wartezeit
            yield return new WaitForSeconds(randomTime);

            // Wenn Animation 2 gerade läuft, spiele sie zweimal so lange
            if (!isAnimation1Playing)
            {
                if (animation2PlayCount < 1)
                {
                    animation2PlayCount++;  // Erhöht den Zähler, um Animation 2 nochmal zu spielen
                    continue;  // Springe zurück zum Anfang der Schleife, um Animation 2 erneut zu spielen
                }
                else
                {
                    animation2PlayCount = 0;  // Setze den Zähler zurück
                }
            }

            // Wechselt den Bool-Wert des Parameters, um die Animation zu wechseln
            isAnimation1Playing = !isAnimation1Playing;

            if (isAnimation1Playing)
            {
                animator.Play("Animation1");  // Wechselt zu Animation 1
            }
            else
            {
                animator.Play("Animation2");  // Wechselt zu Animation 2
            }
        }
    }
}
