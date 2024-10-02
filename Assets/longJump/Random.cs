using System.Collections.Generic;
using UnityEngine;

public class LongJump : MonoBehaviour
{
    void Start()
    {
        List<string> results = new List<string>();

        int jumps = 10;

        for (int i = 0; i < jumps; i++)
        {
            int random = Random.Range(1, 101); 
            
            if (random > 10 && random < 20) 
            {
                results.Add("Foul");
            }
            else
            {
                double jumpResult = Mathf.Round(Random.Range(2.0f, 10.0f)*100)/100; //Damit Nachkommastellen ausgegeben werden
                results.Add(jumpResult + " m");
            }
        }

        Debug.Log("Weitsprung Ergebnisse:");
        foreach (var result in results)
        {
            Debug.Log(result);
        }
    }
}
