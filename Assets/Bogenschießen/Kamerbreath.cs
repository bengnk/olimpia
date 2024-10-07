using UnityEngine;

public class CameraControllerWithBreathing : MonoBehaviour
{
    public float mouseSensitivity = 100f;  // Empfindlichkeit der Mausbewegung
    public float rotationSmoothing = 0.05f;  // Verzögerung der Mausbewegung (kleiner = langsamer)
    public Transform playerBody;  // Referenz auf den Spieler-Body, um die Y-Achse zu rotieren
    public float breathAmplitude = 0.1f;  // Die Amplitude (Höhe und Breite) der Atmung
    public float breathSpeed = 1.0f;      // Geschwindigkeit der Atmung

    private Vector3 originalCameraPosition;  // Ursprüngliche Position der Kamera
    private Vector3 currentRotation;  // Für glatte Mausbewegung
    private Vector3 targetRotation;   // Zielrotation für die Verzögerung

    void Start()
    {
        // Cursor ausblenden und sperren, damit die Maus nur die Kamera steuert
        Cursor.lockState = CursorLockMode.Locked;

        // Speichere die ursprüngliche Position der Kamera
        originalCameraPosition = transform.localPosition;

        // Initialisiere die aktuelle und Zielrotation
        currentRotation = transform.eulerAngles;
        targetRotation = currentRotation;
    }

    void Update()
    {
        HandleMouseLook();
        ApplyBreathingEffect();
    }

    void HandleMouseLook()
    {
        // Erfasse Mausbewegungen
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Berechne die Zielrotation basierend auf der Mausbewegung
        targetRotation.y += mouseX;  // Rotation entlang der Y-Achse (links/rechts)
        targetRotation.x -= mouseY;  // Rotation entlang der X-Achse (hoch/runter)
        targetRotation.x = Mathf.Clamp(targetRotation.x, -90f, 90f);  // Limitiere die X-Achse

        // Interpoliere sanft zur Zielrotation (Verzögerung)
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, rotationSmoothing);

        // Wende die Rotation auf die Kamera an
        transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);

        // Drehe den Spieler-Body entlang der Y-Achse (links und rechts)
        playerBody.localRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
    }

    void ApplyBreathingEffect()
    {
        // Simuliere die Atmung durch eine sinusförmige Veränderung der X- und Y-Position
        float breathOffsetY = Mathf.Sin(Time.time * breathSpeed) * breathAmplitude;  // Auf/Ab (Y-Achse)
        float breathOffsetX = Mathf.Cos(Time.time * breathSpeed * 0.5f) * (breathAmplitude / 2);  // Leichtes Wanken (X-Achse)

        // Aktualisiere die Position der Kamera mit dem Atmungseffekt (X und Y beeinflusst)
        transform.localPosition = new Vector3(
            originalCameraPosition.x + breathOffsetX,  // Bewegung nach links/rechts
            originalCameraPosition.y + breathOffsetY,  // Bewegung nach oben/unten
            originalCameraPosition.z
        );
    }
}
