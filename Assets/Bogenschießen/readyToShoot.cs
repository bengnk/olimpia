using UnityEngine;
using UnityEngine.UI;
using TMPro; // Namespace für Text Mesh Pro

public class ShotCooldownBar : MonoBehaviour
{
    public Slider cooldownSlider; // Referenz zur Slider-Komponente
    public TextMeshProUGUI readyText; // Referenz zum Text Mesh Pro Text-Objekt für "ready"
    public float cooldownTime = 3.5f; // Zeit, die die Bar zum Füllen braucht
    private float currentCooldown = 0f;
    private bool isCharging = false;
    public AudioSource arrowSoundtrack;


    void Start()
    {
        readyText.gameObject.SetActive(false); // Stelle sicher, dass der Text am Anfang unsichtbar ist
        arrowSoundtrack.Play();
    }

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
                currentCooldown = cooldownTime; // Stelle sicher, dass der Wert nicht über die maximale Zeit hinausgeht
                readyText.gameObject.SetActive(true); // Zeige den "ready"-Text an
                Debug.Log("Schuss bereit!"); // An dieser Stelle kann der Schuss abgegeben werden
            }
        }
        // Sobald die Maustaste losgelassen wird, setze die Ladezeit und die Bar zurück
        else if (Input.GetMouseButtonUp(0))
        {
            isCharging = false;
            currentCooldown = 0f; // Reset der Ladezeit
            cooldownSlider.value = 0f; // Setzt den Slider-Wert auf 0
            readyText.gameObject.SetActive(false); // Verberge den "ready"-Text
        }
    }
}
