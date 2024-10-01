using UnityEngine;

public class TargetBehavior : MonoBehaviour
{
    public float speed = 2f; // Geschwindigkeit des Zielrings
    private Vector3 direction;

    // Reduzierte Bewegungsreichweite
    private const float moveRange = 2f; // maximale Bewegungsreichweite in jeder Richtung

    // Timer für die Bewegungsänderung
    private float changeDirectionTime = 3f; // Zeit in Sekunden bis zur nächsten Richtungsänderung
    private float timer; // Timer zur Verfolgung der Zeit

    void Start()
    {
        // Zufällige Bewegungsrichtung
        direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
        timer = changeDirectionTime; // Timer initialisieren
    }

    void Update()
    {
        // Bewege den Zielring in die aktuelle Richtung
        transform.position += direction * speed * Time.deltaTime;

        // Überprüfe die Grenzen und wende die Richtung um
        if (transform.position.x > moveRange || transform.position.x < -moveRange)
            direction.x *= -1;

        if (transform.position.z > moveRange || transform.position.z < -moveRange)
            direction.z *= -1;

        // Aktualisiere den Timer und ändere die Richtung alle 3 Sekunden
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            ChangeDirection();
            timer = changeDirectionTime; // Timer zurücksetzen
        }
    }

    // Funktion zum Ändern der Bewegungsrichtung
    private void ChangeDirection()
    {
        direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)).normalized;
    }
}
