/*
* Copyright (C) Noah Greaves in Association with VFS
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK.Wwise;

// put on the player/enemies to play footstep sounds relative to the player/enemies
// DO NOT USE
public class FootstepSwitch : MonoBehaviour
{
    [SerializeField]
    LayerMask groundLayer;

    public void CheckFootstepSurface()
    {
        float yOffset = 0.5f;                               // set the offset of the raycast to be from the center of the player model
        Vector3 pos = transform.position;
        pos.y += yOffset;
        Ray ray = new Ray(pos, Vector3.down);               // set the ray to check the gorund 
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1.5f, groundLayer))
        {
            GroundType ground = hit.collider.GetComponent<GroundType>();        // get the type of surface the raycast hit
            ground.groundSwitchValue = ground.GetGroundSwitchValue();
            //Could do this: AkSoundEngine.SetSwitch("footstep_surfaces", ground.name, gameobject);
            // AkSoundEngine.SetSwitch("footstep_surfaces", ground.name, gameObject);
            switch(ground.groundSwitchValue)
            {
                // check what ground surface the player is walking on
                case GroundSwitchValue.DIRT:
                    AkSoundEngine.SetSwitch("Forgotten_Sun", "dirt", gameObject);       // PARAMS: SetSwitch(<SoundBank To Use>, <Name Of Sound>, <The Game Object to produce the sound>)
                    break;
                case GroundSwitchValue.MUD:
                    AkSoundEngine.SetSwitch("Forgotten_Sun", "mud", gameObject);
                    break;
                case GroundSwitchValue.GRAVEL:
                    AkSoundEngine.SetSwitch("Forgotten_Sun", "gravel", gameObject);
                    break;
                case GroundSwitchValue.WATER:
                    AkSoundEngine.SetSwitch("Forgotten_Sun", "water", gameObject);
                    break;
                case GroundSwitchValue.GRASS:
                    AkSoundEngine.SetSwitch("Forgotten_Sun", "grass", gameObject);
                    break;
                case GroundSwitchValue.STONE:
                    AkSoundEngine.SetSwitch("Forgotten_Sun", "grass", gameObject);
                    break;
                default:
                    //Default it so it's always valid
                    AkSoundEngine.SetSwitch("Forgotten_Sun", "grass", gameObject);
                    break;
            }
        }
    }
}