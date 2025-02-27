using UnityEngine;

public class CameraControllerWithBreathing : MonoBehaviour
{
    public float mouseSensitivity = 100f;  // Standard-Mausempfindlichkeit
    public float rotationSmoothing = 0.05f;  // Verzögerung der Mausbewegung
    public Transform playerBody;           // Referenz auf den Spieler-Body zur Rotation
    public float breathAmplitude = 0.1f;     // Amplitude der Atmung
    public float breathSpeed = 1.0f;         // Geschwindigkeit der Atmung

    private Vector3 originalCameraPosition; // Ursprüngliche Kameraposition
    private Vector3 currentRotation;        // Für glatte Rotation
    private Vector3 targetRotation;         // Zielrotation

    private float breathAmplitudeIncreaseRate = 0.2f; // Standard-Zuwachsrate der Atmung
    private float currentBreathAmplitude = 0f;        // Aktuelle Atmungsamplitude
    private float maxBreathAmplitude = 1.5f;            // Maximalwert der Amplitude

    void Start()
    {
        // Werte aus PlayerPrefs laden (falls in der Auswahl-Szene gesetzt)
        if (PlayerPrefs.HasKey("breathAmplitudeIncreaseRate"))
            breathAmplitudeIncreaseRate = PlayerPrefs.GetFloat("breathAmplitudeIncreaseRate");
        if (PlayerPrefs.HasKey("breathSpeed"))
            breathSpeed = PlayerPrefs.GetFloat("breathSpeed");
        if (PlayerPrefs.HasKey("mouseSensitivity"))
            mouseSensitivity = PlayerPrefs.GetFloat("mouseSensitivity");

        // Setze die Kamera relativ zum Elternobjekt (z. B. um 45°)
        transform.localRotation = Quaternion.Euler(0, 45, 0);

        // Speichere die ursprüngliche Position der Kamera
        originalCameraPosition = transform.localPosition;

        // Speichere die Startrotation
        currentRotation = transform.localEulerAngles;
        targetRotation = currentRotation;
    }

    void Update()
    {
        HandleMouseLook();
        ApplyBreathingEffect();

        // Erhöhe die Atmungsamplitude, solange die linke Maustaste gedrückt wird
        if (Input.GetMouseButton(0) && currentBreathAmplitude <= maxBreathAmplitude)
        {
            currentBreathAmplitude += breathAmplitudeIncreaseRate * Time.deltaTime;
        }

        // Reduziere die Amplitude sanft wieder auf 0
        currentBreathAmplitude = Mathf.Lerp(currentBreathAmplitude, 0f, rotationSmoothing);
    }

    void HandleMouseLook()
    {
        // Erfasse Mausbewegungen
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Berechne die Zielrotation basierend auf der Mausbewegung
        targetRotation.y += mouseX;
        targetRotation.x -= mouseY;
        targetRotation.x = Mathf.Clamp(targetRotation.x, -90f, 90f);

        // Interpoliere sanft zur Zielrotation
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, rotationSmoothing);

        // Wende die Rotation auf die Kamera an
        transform.localRotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0f);

        // Drehe den Spieler-Body entsprechend
        playerBody.localRotation = Quaternion.Euler(0f, currentRotation.y, 0f);
    }

    void ApplyBreathingEffect()
    {
        // Simuliere die Atmung als sinusförmige Verschiebung
        float breathOffsetY = Mathf.Sin(Time.time * breathSpeed) * currentBreathAmplitude;
        float breathOffsetX = Mathf.Cos(Time.time * breathSpeed * 0.5f) * (currentBreathAmplitude / 2);

        // Wende den Atmungseffekt auf die Kameraposition an
        transform.localPosition = new Vector3(
            originalCameraPosition.x + breathOffsetX,
            originalCameraPosition.y + breathOffsetY,
            originalCameraPosition.z
        );
    }
}
