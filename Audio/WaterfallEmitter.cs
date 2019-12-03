using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterfallEmitter : MonoBehaviour
{
    private string _playWaterfall = "Play_WaterFall_Emiter";
    private string _stopWaterfall = "Stop_WaterFall_Emiter";

    // Start is called before the first frame update
    void Start()
    {
        AkSoundEngine.PostEvent(_playWaterfall, gameObject);
    }

    private void OnDisable()
    {
        AkSoundEngine.PostEvent(_stopWaterfall, gameObject);
    }
}
