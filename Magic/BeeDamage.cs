using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeDamage : MonoBehaviour
{
    [SerializeField] private float _damage = 2.0f;

    private HealthScript _victimHealth;

    private void OnCollisionEnter(Collision other)
    {
        _victimHealth = other.gameObject.GetComponent<HealthScript>();
        if (_victimHealth != null)
        {
            _victimHealth.Damage(_damage, AttackType.PHYSICAL, GameMaster.instance.Player.gameObject);
        }
    }
}
