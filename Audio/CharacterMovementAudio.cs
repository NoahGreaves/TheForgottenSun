/*
* Copyright (C) Noah Greaves in Association with VFS
*/

using System.Linq;
using UnityEngine;

// USE FOR FOOTSTEPS
[RequireComponent(typeof(FootstepSwitch))]
public class CharacterMovementAudio : MonoBehaviour
{
    private bool _hasSetSwitch = false;
    private string _speedSwitchGroup = "Footstep_Speed";
    private string _walkSwitch = "walk";
    private string _runSwitch = "run";

    private string _kiinPlayFootstep = "Play_Kiin_Footsteps";

    // TODO: When we get the rest of the footstep sounds replace the null variables
    private string _healerPlayFootstep = "Play_Healer_Footstep";
    private string _rangerPlayFootstep = "Play_Ranger_Footstep";
    private string _tankPlayFootstep = "Play_Tank_Footstep";
    private string _deathGodPlayFootstep = "Play_Death_God_Footsteps";

    private string[] _SurfaceSwitches = {"Dirt", "Grass", "Gravel", "Leaves", "Mud", "Stone", "Water"};

    private void OnCollisionEnter(Collision collision) 
    {
        if (_SurfaceSwitches.Contains(collision.gameObject.tag))
        {
            AkSoundEngine.SetSwitch("Footstep_Surfaces", collision.gameObject.tag, this.gameObject);
           // Debug.Log($"{this.ToString()} | Footstep_Surfaces = {collision.gameObject.tag}");
        }
    }

    public void PlayFootstep(string animParameters)
    {
        string[] parameters = animParameters.Split('~');

        // Set Switch
        if (!_hasSetSwitch)
        {
            // walk = 0, speed = 1
            if (parameters[1] == "walk")
            {
                AkSoundEngine.SetSwitch(_speedSwitchGroup, _walkSwitch, this.gameObject);
            }
            else if (parameters[1] == "run")
            {
                AkSoundEngine.SetSwitch(_speedSwitchGroup, _walkSwitch, this.gameObject);
            }
            else
            {
                Debug.Log("Invalid switch for footsteps.");
            }

            _hasSetSwitch = true;
        }

        // Play Footstep ( K'iin )
        if (parameters[0] == "K'iin")
        {
            AkSoundEngine.PostEvent(_kiinPlayFootstep, gameObject);
            // Debug.Log("Triggered footstep - K'iin.");
        }

        // Play Footstep ( Healer )
        else if (parameters[0] == "Healer")
        {
            AkSoundEngine.PostEvent(_healerPlayFootstep, gameObject);
            // Debug.Log("Triggered footstep - Healer.");
        }

        // Play Footstep ( Ranger )
        else if (parameters[0] == "Ranger")
        {
            AkSoundEngine.PostEvent(_rangerPlayFootstep, gameObject);
            // Debug.Log("Triggered footstep - Ranger.");
        }

        // Play Footstep ( Tank )
        else if (parameters[0] == "Tank")
        {
            AkSoundEngine.PostEvent(_tankPlayFootstep, gameObject);
            // Debug.Log("Triggered footstep - Tank.");
        }

        // Play Footstep ( Boss )
        else if (parameters[0] == "Boss")
        {
            AkSoundEngine.PostEvent(_deathGodPlayFootstep, gameObject);
            // Debug.Log("Triggered footstep - Tank.");
        }
     }
}
