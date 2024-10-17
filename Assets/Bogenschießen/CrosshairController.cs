using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    public RectTransform crosshair;  // UI-Element für das Fadenkreuz
    public float breathAmplitude = 15f;  // Amplitude der Atmung
    public float breathSpeed = 2f;      // Geschwindigkeit der Atmung
    public float maxOffsetAmount = 50f; // Maximaler Offset, wenn die Maus stillsteht
    public float speedFactor = 5f;       // Faktor, um den Offset zu reduzieren, je schneller die Maus bewegt wird

    private Vector2 lastMousePosition; // Letzte Mausposition
    private float currentOffsetAmount;  // Aktueller Offset
    private Vector2 driftDirection;      // Drift-Richtung

    void Start()
    {
        

        // Initialisiere die letzte Mausposition
        lastMousePosition = Input.mousePosition;

        // Setze eine zufällige Drift-Richtung zu Beginn
        SetRandomDriftDirection();
    }

    void Update()
    {
        // Berechne die aktuelle Mausposition
        Vector2 mousePosition = Input.mousePosition;

        // Berechne die Mausbewegung
        float mouseMovement = Vector2.Distance(mousePosition, lastMousePosition);

        // Berechne den aktuellen Offset basierend auf der Mausbewegung
        currentOffsetAmount = maxOffsetAmount - (mouseMovement * speedFactor);
        currentOffsetAmount = Mathf.Max(currentOffsetAmount, 0);

        // Berechne den vertikalen Offset basierend auf der Zeit
        float breathOffsetY = Mathf.Sin(Time.time * breathSpeed) * breathAmplitude;

        // Wende den Offset auf die Mausposition an
        Vector2 targetPosition = mousePosition + driftDirection * currentOffsetAmount + new Vector2(0, breathOffsetY);

        // Interpoliere die Position des Fadenkreuzes für einen Verzögerungseffekt
        crosshair.position = Vector2.Lerp(crosshair.position, targetPosition, Time.deltaTime * 5f);

        // Aktualisiere die letzte Mausposition
        lastMousePosition = mousePosition;
    }

    void SetRandomDriftDirection()
    {
        // Generiere eine zufällige Drift-Richtung einmal zu Beginn
        float angle = Random.Range(0f, 360f); // Zufälliger Winkel in Grad
        driftDirection = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

        // Setze die Drift-Richtung für eine gewisse Zeit (z.B. 2 Sekunden)
        Invoke("SetRandomDriftDirection", 2f); // Ändere die Richtung alle 2 Sekunden
    }
}
