using UnityEngine;
using UnityEngine.UI;

public class ShotCooldownBar : MonoBehaviour
{
    public Slider cooldownSlider; // Referenz zur Slider-Komponente
    public float cooldownTime = 3.5f; // Zeit, die die Bar zum Füllen braucht
    private float currentCooldown = 0f;
    private bool isCharging = false;

    void Update()
    {
        // Wenn die linke Maustaste gedrückt wird, beginne mit dem Aufladen
        if (Input.GetMouseButton(0))
        {
            isCharging = true;
            currentCooldown += Time.deltaTime; // Erhöhe die Ladezeit
            cooldownSlider.value = currentCooldown / cooldownTime; // Slider-Wert aktualisieren

            // Überprüfe, ob die Ladezeit abgeschlossen ist
            if (currentCooldown >= cooldownTime)
            {
                currentCooldown = cooldownTime; // Stelle sicher, dass der Wert nicht über 1 hinausgeht
                Debug.Log("Schuss bereit!"); // An dieser Stelle kann der Schuss abgegeben werden
            }
        }
        // Sobald die Maustaste losgelassen wird, setze die Ladezeit und die Bar zurück
        else if (Input.GetMouseButtonUp(0))
        {
            isCharging = false;
            currentCooldown = 0f; // Reset der Ladezeit
            cooldownSlider.value = 0f; // Setzt den Slider-Wert auf 0
        }
    }
}
