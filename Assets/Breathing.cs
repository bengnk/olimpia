using UnityEngine;

public class Breathing : MonoBehaviour
{
    public float amplitude = 0.1f; // Höhe der Bewegung
    public float speed = 1f; // Geschwindigkeit der Bewegung

    private Vector3 initialPosition; // Startposition der Kamera

    void Start()
    {
        // Speichere die Startposition der Kamera
        initialPosition = transform.position;
    }

    void Update()
    {
        // Berechne die neue Position basierend auf der Zeit
        float newY = initialPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        
        // Setze die neue Position der Kamera
        transform.position = new Vector3(initialPosition.x, newY, initialPosition.z);
    }
}
