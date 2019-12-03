using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraveEmitter : MonoBehaviour
{
    private string _startVoices = "Play_Grave_Emitter";
    private string _stopVoices = "Stop_Grave_Emitter";

    private void Start()
    {
        PlayVoices();
    }

    private void PlayVoices()
    {
        AkSoundEngine.PostEvent(_startVoices, gameObject);
    }

    private void StopVoices()
    {
        AkSoundEngine.PostEvent(_stopVoices, gameObject);
    }

    private void OnDisable()
    {
        StopVoices();
    }
}
