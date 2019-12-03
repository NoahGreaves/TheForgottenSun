using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    private string _playMusic = "Play_Music";
    private string _stopMusic = "Stop_Music";

    private void Start() 
    {   
        // setting the states for the music     
        AkSoundEngine.SetState("BossHealth", "None");
        AkSoundEngine.SetState("GameState",  "None");
        AkSoundEngine.SetState("Intensity",  "None");

        // setting the switches for the music
        AkSoundEngine.SetSwitch("Location", "MainTheme", gameObject);

        // play the music
        AkSoundEngine.PostEvent(_playMusic, gameObject);
    }

    private void OnDisable()
    {
        AkSoundEngine.PostEvent(_stopMusic, gameObject);        
    }
}
