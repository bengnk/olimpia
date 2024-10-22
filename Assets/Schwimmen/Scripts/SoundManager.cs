using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource countdownSound;
    public PauseManager pauseScript;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!countdownSound.isPlaying && !pauseScript.isPaused && !gameManager.isGoTime && gameManager.gameStarted) {
            countdownSound.Play();
        } else if(pauseScript.isPaused){
            countdownSound.Pause();
        }
    }
}
