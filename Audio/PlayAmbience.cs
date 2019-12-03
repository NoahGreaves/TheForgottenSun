using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAmbience : MonoBehaviour
{
    [SerializeField] private string _levelName;
    
    private string _startCaveAmbience = "Play_Crypt_Ambience";
    private string _stopCaveAmbience = "Stop_Crypt_Ambience";

    private string _startRainAmbience = "Play_Rain_Forest_Ambience";
    private string _stopRainAmbience = "Stop_Rain_Forest_Ambience";

    private void Start()
    {
        Play(_levelName);    
    }

    private void Play(string level)
    {
        string[] levelName = level.Split('~');

        if(levelName[1] == "Cave")
        {
            AkSoundEngine.PostEvent(_startCaveAmbience, gameObject);                //plays the cave ambience at start of level
        }
        else if(levelName[1] == "RainForest")
        {
            AkSoundEngine.PostEvent(_startRainAmbience, gameObject);                //plays the cave ambience at start of level
        }
    }

    private void Stop(string level)
    {
        string[] levelName = level.Split('~');

        if(levelName[1] == "Cave")
        {
            AkSoundEngine.PostEvent(_stopCaveAmbience, gameObject);                //plays the cave ambience at start of level
        }
        else if(levelName[1] == "RainForest")
        {
            AkSoundEngine.PostEvent(_stopRainAmbience, gameObject);                //plays the cave ambience at start of level
        }
    }

    private void OnDisable()
    {
        Stop(_levelName);    
    }
}
