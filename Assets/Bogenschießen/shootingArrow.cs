using UnityEngine;

public class ArrowShoot : MonoBehaviour
{
    public GameObject arrowPrefab;  // Pfeil-Prefab
    public Transform shootPoint;    // Startpunkt des Pfeils
    public float shootForce = 50f;  // Kraft des Pfeils
    private GameObject currentArrow; // Referenz zum aktuellen Pfeil

    void Update()
    {
        // Überprüfe, ob der Spieler die Schusstaste drückt und kein Pfeil aktiv ist
        if (Input.GetButtonDown("Fire1") && currentArrow == null)
        {
            ShootArrow();
        }
    }

    void ShootArrow()
    {
        // Erstelle einen neuen Pfeil nur, wenn noch keiner vorhanden ist
        currentArrow = Instantiate(arrowPrefab, shootPoint.position, shootPoint.rotation);

        // Greife auf die Rigidbody-Komponente des Pfeils zu
        Rigidbody rb = currentArrow.GetComponent<Rigidbody>();

        // Setze den Pfeil auf kinematisch (keine physikalischen Effekte), bevor er abgeschossen wird
        rb.isKinematic = false;

        // Wende Kraft auf den Pfeil an, um ihn zu schießen
        rb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);

        // Füge dem Pfeil das Treffererkennungs-Skript hinzu
        currentArrow.AddComponent<ArrowCollision>();
    }
}

// Skript für die Treffererkennung des Pfeils
public class ArrowCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        // Überprüfe, welches Tag das getroffene Objekt hat
        if (collision.gameObject.CompareTag("whiteCircle"))
        {
            Debug.Log("Weißer Kreis getroffen!");
        }
        else if (collision.gameObject.CompareTag("blackCircle"))
        {
            Debug.Log("Schwarzer Kreis getroffen!");
        }
        else if (collision.gameObject.CompareTag("blueCircle"))
        {
            Debug.Log("Blauer Kreis getroffen!");
        }
        else if (collision.gameObject.CompareTag("redCircle"))
        {
            Debug.Log("Roter Kreis getroffen!");
        }
        else if (collision.gameObject.CompareTag("yellowCircle"))
        {
            Debug.Log("Gelber Kreis getroffen!");
        }

        // Pfeil nach dem Treffer fixieren
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;  // Stoppe den Pfeil nach dem Treffer

        // Pfeil wird Teil des getroffenen Objekts
        transform.parent = collision.transform;

        // Optional: Pfeil nach einer gewissen Zeit zerstören oder deaktivieren
        Destroy(gameObject, 5f); // Zerstöre den Pfeil nach 5 Sekunden
    }
}
