using UnityEngine;
using UnityEngine.UI;
using TMPro; // Namespace für Text Mesh Pro
using System.Collections;

public class ShotCooldownBar : MonoBehaviour
{
    public Slider cooldownSlider; // Referenz zur Slider-Komponente
    public TextMeshProUGUI readyText; // Referenz zum Text Mesh Pro Text-Objekt für "ready"
    public float cooldownTime = 3.5f; // Zeit, die die Bar zum Füllen braucht
    private float currentCooldown = 0f;
    private bool isCharging = false;
    private bool isReady = false; // Verhindert mehrfaches Starten der Coroutine
    private bool canCharge = true; // Steuert, ob der Spieler aufladen darf
    public AudioSource arrowSoundtrack;

    void Start()
    {
        readyText.gameObject.SetActive(false); // Stelle sicher, dass der Text am Anfang unsichtbar ist
        arrowSoundtrack.Play();
    }

    void Update()
    {
        // Wenn der Spieler aufladen darf und die Maustaste gedrückt wird
        if (canCharge && Input.GetMouseButton(0))
        {
            isCharging = true;
            currentCooldown += Time.deltaTime; // Erhöhe die Ladezeit
            cooldownSlider.value = currentCooldown / cooldownTime; // Slider-Wert aktualisieren

            // Überprüfe, ob die Ladezeit abgeschlossen ist
            if (currentCooldown >= cooldownTime && !isReady)
            {
                currentCooldown = cooldownTime; // Stelle sicher, dass der Wert nicht über die maximale Zeit hinausgeht
                readyText.gameObject.SetActive(true); // Zeige den "ready"-Text an
                Debug.Log("Schuss bereit!"); // An dieser Stelle kann der Schuss abgegeben werden
                
                isReady = true;
            }
        }
        // Sobald die Maustaste losgelassen wird, beginne mit der Sperre
        else if (Input.GetMouseButtonUp(0) && isReady)
        {
            isCharging = false;
            canCharge = false; // Sperre das erneute Aufladen
            StartCoroutine(WaitAndReset());
        }
    }

    IEnumerator WaitAndReset()
    {
        cooldownSlider.value = 0f; // Setzt den Slider-Wert auf 0
        yield return new WaitForSeconds(3.8f);
        readyText.gameObject.SetActive(false); // "ready"-Text nach 3 Sekunden ausblenden
        isReady = false;
        currentCooldown = 0f; // Reset der Ladezeit
        canCharge = true; // Spieler darf wieder aufladen
    }
}
