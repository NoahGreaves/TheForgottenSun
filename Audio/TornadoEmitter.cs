using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoEmitter : MonoBehaviour
{
    private string _startTornado = "Play_Death_Titan_Tornado";
    private string _stopTornado = "Stop_Death_Titan_Tornado";

    private void OnEnable()
    {
        AkSoundEngine.PostEvent(_startTornado, gameObject);
    }

    private void OnDisable()
    {
        AkSoundEngine.PostEvent(_stopTornado, gameObject);    
    }
}
