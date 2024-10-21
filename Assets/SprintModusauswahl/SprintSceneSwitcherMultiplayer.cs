using UnityEngine;
using UnityEngine.SceneManagement; // Für den Szenenwechsel
using UnityEngine.UI; // Für den Button

public class SprintSceneSwitcherMultiplayer : MonoBehaviour
{
    // Der Name oder Index der Szene, die geladen werden soll
    public string sceneToLoad;

    // Hole den Button und weise ihm den Listener zu
    void Start()
    {
        // Der Button ist an demselben GameObject
        Button button = GetComponent<Button>();

        if (button != null)
        {
            // Weisen Sie die Funktion "OnButtonClicked" zu, wenn der Button gedrückt wird
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    // Diese Methode wird aufgerufen, wenn der Button gedrückt wird
    void OnButtonClicked()
    {
        SingleplayerVarHolder.singleplayer = false;
        
        // Wechsel zur definierten Szene
        SceneManager.LoadScene(sceneToLoad);
    }
}
