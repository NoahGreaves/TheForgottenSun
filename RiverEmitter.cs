using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverEmitter : MonoBehaviour
{
    private string _playRiver = "Play_River_Emitter";
    private string _stopRiver = "Stop_River_Emitter";

    private void OnEnable()
    {
        AkSoundEngine.PostEvent(_playRiver, gameObject);
    }

    private void OnDisable()
    {
        AkSoundEngine.PostEvent(_stopRiver, gameObject);
    }
}
