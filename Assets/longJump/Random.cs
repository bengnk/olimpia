using System.Collections.Generic;
using UnityEngine;

public class LongJump : MonoBehaviour
{
    private List<string> results = new List<string>();  // Ergebnisse der Gegner

    void Start()
    {
        int jumps = 5;  // Anzahl der Sprünge für Gegner

        for (int i = 0; i < jumps; i++)
        {
            int random = Random.Range(1, 101); 
            
            if (random > 10 && random < 20) 
            {
                results.Add("Foul");
            }
            else
            {
                double jumpResult = Mathf.Round(Random.Range(2.0f, 10.0f) * 100) / 100; // Zwei Nachkommastellen
                results.Add(jumpResult + " m");
            }
        }

        Debug.Log("Weitsprung Ergebnisse:");
        foreach (var result in results)
        {
            Debug.Log(result);
        }
    }

    // Gibt die Liste der Ergebnisse zurück
    public List<string> GetResults()
    {
        return results;
    }
}
