using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    // Enemy attack sounds
    private string _healerAttackSound = "Play_Healer_Attack";
    private string _rangerAttackSound = "Play_Ranger_Attack";
    private string _tankAttackSound   = "Play_Tank_Attack";

    // Enemy damaged sounds
    private string _healerDamagedSound = "Play_Healer_Damaged";
    private string _rangerDamagedSound = "Play_Ranger_Damaged";
    private string _tankDamagedSound = "Play_Tank_Damaged";

    // Attack Sound Functions
    public void HealerAttack() { AkSoundEngine.PostEvent(_healerAttackSound, this.gameObject); }
    public void RangerAttack() { AkSoundEngine.PostEvent(_rangerAttackSound, this.gameObject); }
    public void TankAttack()   { AkSoundEngine.PostEvent(_tankAttackSound, this.gameObject); }

    // Damaged Sound Functions
    public void HealerDamage() { AkSoundEngine.PostEvent(_healerDamagedSound, this.gameObject); }
    public void RangerDamage() { AkSoundEngine.PostEvent(_rangerDamagedSound, this.gameObject); }
    public void TankDamage()   { AkSoundEngine.PostEvent(_tankDamagedSound, this.gameObject); }
}
