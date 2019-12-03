using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandDamage : MonoBehaviour
{
    [SerializeField, Tooltip("The distance from the hands that will inflict damage to the player")]
    private float _damageRadius = 3;

    [SerializeField] private float _damage = 5;
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private LayerMask _bossLayer;

    public GameObject Player;

    private bool _dealtDamage = false;

    private float _damageCooldown = 1;
    private float timer = 0;

    private void OnTriggerEnter(Collider other)
    {
        // DealDamage();
        if(_dealtDamage == false)
        {
            if((1<<other.gameObject.layer) == _enemyLayer || (1<<other.gameObject.layer) == _bossLayer) { return; }
            HealthScript playerHealth = GameMaster.instance.Player.GetComponent<HealthScript>();
            if(playerHealth == null || other.CompareTag("Player") == false) { return; }

            Debug.Log($"damaged player: {playerHealth.gameObject.name}");
            playerHealth.Damage(_damage); 
            _dealtDamage = true;
            StartCoroutine(Wait());
        }
    }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(_damageCooldown);
        _dealtDamage = false;
    }

    private void DealDamage()
    {
        _dealtDamage = true;
        Collider[] targets = Physics.OverlapSphere(transform.position, _damageRadius);        // get all of the enemies within a radius around the spawned hand
        foreach(Collider target in targets)                                                         // make the hands deal damage
        {
            if((1<<target.gameObject.layer) == _enemyLayer || (1<<target.gameObject.layer) == _bossLayer) { continue; }
            HealthScript playerHealth = GameMaster.instance.Player.GetComponent<HealthScript>();
            if(playerHealth == null) { continue; }

            Debug.Log($"damaged player: {playerHealth.gameObject.name}");
            playerHealth.Damage(_damage);                                                   // damage the player if they are within the radius
        }
    }

    private void OnDrawGizmos()
    {
        // gizmo for the boss search radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _damageRadius);
    }
}
