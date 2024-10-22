using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundtrackManager : MonoBehaviour
{
    private static SoundtrackManager instance = null;

    public AudioSource audioSource;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        // Subscribe to the scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the event when disabled
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Check the scene when a new one is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // List the scenes where the soundtrack should play
        if (scene.name == "Start" || scene.name == "Menü" || scene.name == "SprintModusauswahl")
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();  // Play the soundtrack in these scenes
            }
        }
        else
        {
            audioSource.Stop();  // Stop the soundtrack in other scenes
        }
    }
}
