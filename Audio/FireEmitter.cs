using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEmitter : MonoBehaviour
{
    private string _startFireSound = "Play_Shadow_Wall";
    private string _endFireSound = "Play_Shadow_Wall";

    private void OnEnable()
    {
        AkSoundEngine.PostEvent(_startFireSound, gameObject);
    }

    private void OnDisable()
    {
        AkSoundEngine.PostEvent(_endFireSound, gameObject);
    }
}
