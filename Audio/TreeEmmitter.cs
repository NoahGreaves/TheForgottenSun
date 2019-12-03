using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeEmmitter : MonoBehaviour
{
    private string _playTreeEmitter = "Play_Tree_Emitter_Loop";
    private string _stopTreeEmitter = "Stop_Tree_Emitter_Loop";

    private void Start()
    {
        AkSoundEngine.PostEvent(_playTreeEmitter, gameObject);
    }

    private void OnDisable()
    {
        AkSoundEngine.PostEvent(_stopTreeEmitter, gameObject);    
    }
}
