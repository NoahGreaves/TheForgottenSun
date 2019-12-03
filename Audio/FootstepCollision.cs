/*
* Copyright (C) Noah Greaves in Association with VFS
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// put on the ground to play sounds reletive to the ground 
// DO NOT USE
public class FootstepCollision : MonoBehaviour
{
    private GroundType groundType;

    private void Start()
    {
        groundType = GetComponent<GroundType>();
    }

    private void OnTriggerEnter(Collider otherGameObject)
    {
        groundType.groundSwitchValue = groundType.GetGroundSwitchValue();
        if (otherGameObject.gameObject.tag == "Player")
        {
            AkSoundEngine.SetSwitch("Forgotten_Sun", groundType.groundSwitchValue.ToString(), otherGameObject.gameObject);       // PARAMS: SetSwitch(<SoundBank To Use>, <Name Of Sound>, <The Game Object to produce the sound>)
        }
    }
}