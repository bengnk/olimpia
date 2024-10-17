using UnityEngine;

public class BowController : MonoBehaviour
{
    public Transform bow; // Referenz auf das Bow-Objekt
    public float rotationSpeed = 0.1f; // Geschwindigkeit der Rotation, reduziert für einen kleineren Radius

    void Update()
    {
        // Erhalte die Mausbewegung
        float mouseX = Input.GetAxis("Mouse X");

        // Berechne die neue Y-Rotation basierend auf der Mausbewegung
        float newYRotation = bow.eulerAngles.y + mouseX * rotationSpeed;

        // Wende die neue Rotation an, wobei die X- und Z-Werte unverändert bleiben
        bow.rotation = Quaternion.Euler(bow.eulerAngles.x, newYRotation, bow.eulerAngles.z);
        
        // Halte die Position des Bogens konstant
        bow.position = new Vector3(bow.position.x, 0.712031f, bow.position.z); // Setze hier den Y-Wert auf den gewünschten Wert
    }
}
