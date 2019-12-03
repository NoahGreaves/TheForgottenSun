/*
* Copyright (C) Noah Greaves in Association with VFS
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyWave : MonoBehaviour
{
    [SerializeField] private GameObject _energywave;
    [SerializeField] private float _heatwaveSize;
    [SerializeField] private float _damage;
    [SerializeField] public float _waveExpandTime = 2.0f;
    [SerializeField] private LayerMask playerMask;             // get the layer the player is on
    private float _curLerpTime = 0.0f;

    private WeakPoints _weakPoints = new WeakPoints();

    public float _manaCost;
    private Vector3 _pos;

    private int _waveCounter = 0;
    private bool _waveCalled = false;

    private Collider[] enemyTargets;
    private float debugWaveSize;

    private string _energyWaveSound = "Play_Energy_Wave";
    private bool _lerpDone  = true;

    // starts the energy wave expansion
    public void StartEnergywave()
    {
        _curLerpTime = 0.0f;
        if (_waveCalled == false && _lerpDone == true)
        {
            StartCoroutine(ExpandWave());
        }
    }

    private void ResetIsCalled()
    {
        _waveCalled = false;
        StopCoroutine(ExpandWave());
    }

    private IEnumerator ExpandWave()
    {
        AkSoundEngine.PostEvent(_energyWaveSound, gameObject);
        _waveCounter += 1;
        _waveCalled = true;
        _lerpDone = false;
        GameObject energywaveClone = Instantiate(_energywave, transform.position, transform.rotation);     // create a clone of the energy wave
        Vector3 waveStartSize = energywaveClone.transform.localScale;                                      // get the original size of the wave
        float waveYSize = energywaveClone.transform.localScale.y;
        Vector3 waveTargetSize = new Vector3(_heatwaveSize, waveStartSize.y, _heatwaveSize);               // set the target size
        while (_curLerpTime < _waveExpandTime)                                                              // while the lerp is still in progress expand the wave
        {
            energywaveClone.transform.position = transform.position;                                       // set the waves position as the players position
            _curLerpTime += Time.deltaTime;
            if (_curLerpTime > _waveExpandTime)
            {
                _curLerpTime = _waveExpandTime;
                _lerpDone = true;
                Reset(enemyTargets);
                ResetIsCalled();
                Destroy(energywaveClone);
                StopCoroutine(ExpandWave());
                break;
            }
            float perc = _curLerpTime / _waveExpandTime;                                                   // get the percentage of completed time
            Vector3 newWaveSize = Vector3.Lerp(waveStartSize, waveTargetSize, perc);                       // lerp the original wave size to the target wave size
            energywaveClone.transform.localScale = newWaveSize;                                            // set waves new size

            DamageEnemies(newWaveSize.magnitude);                                                          // damage enemies that collide with the wave
            yield return null;
        }
    }

    private void DamageEnemies(float waveSize)
    {
        debugWaveSize = waveSize;
        Collider[] targets = Physics.OverlapSphere(transform.position, waveSize);              // do a overlap sphere that is the same size as the wave
        enemyTargets = targets;
        foreach (Collider target in targets)
        {
            BaseAI enemy = target.GetComponent<BaseAI>();                                      // get the enemies that the wave collided with
            if (enemy == null) { continue; }
            if (enemy.IsHit == true) { continue; }
            if (target.gameObject.layer == playerMask) { continue; }                            // if the wave overlapped the player. ignore it

            HealthScript enemyHealth = enemy.GetComponent<HealthScript>();                     // get the enemies health
            if (enemyHealth == null) { continue; }                                              // if the object doesn't have health ignore it

            enemy.IsHit = true;
            enemyHealth.Damage(_damage, AttackType.MAGICAL, gameObject);                       // deal the regular amount of damage to all enemies
        }
    }

    private void Reset(Collider[] targets)
    {
        _waveCounter = 0;
        foreach (Collider target in targets)
        {
            if(target.GetComponent<BaseAI>() == null) { continue; }
            BaseAI enemy = target.GetComponent<BaseAI>();                                      // get the enemies that the wave collided with
            if (enemy == null) { continue; }
            if (enemy.IsHit == false) { continue; }
            if (target.gameObject.layer == playerMask) { continue; }                            // if the wave overlapped the player. ignore it

            enemy.IsHit = false;                                                               // reset the IsHit boolean 
        }
    }

    private void OnDrawGizmos()
    {
        // gizmo for the boss search radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, debugWaveSize);
    }
}
