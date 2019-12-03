/*
* Copyright (C) Noah Greaves in Association with VFS
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    [SerializeField] private LineRenderer _lightningPrefab;
    [SerializeField] private float _yOffset = 8;
    [SerializeField] private float _lifetime;
    [SerializeField] private float _lightningMovement = 2f;
    [SerializeField] private float _damage;

    [SerializeField] private GameObject _noEnemytarget;

    [SerializeField, Tooltip("The max number of enemies that are allowed to get struck at once")]
    private int _maxNumOfStriked = 10;

    [SerializeField, Tooltip("The radius that the lightning will search for more targets in")]
    private float _radius;

    [Tooltip("The amount of time that the lightning takes before going to the next target")]
    [SerializeField] private float _transferTime = 0.2f;

    [SerializeField] private float _timeScale = 0.5f;

    private string _lightningSound = "Play_Chain_Lightning";
    private string _lightningImpactSound = "Play_Chain_Lightning_Impact";

    public float _manaCost;

    private LineRenderer _lightning;

    private BaseAI[] _enemies;
    private float _maxLifetime;
    private float _maxCooldown;
    private int _numOfStriked;

    private Vector3 startPos;
    private Vector3 endPos;
    private Vector3 spawnPos;

    private List<Collider> colliderToReset = new List<Collider>();

    private LayerMask playerMask;

    public void StartChainLightning()
    {
        _maxLifetime = _lifetime;
        StartCoroutine(StrikeEnemies());
    }

    private IEnumerator StrikeEnemies()
    {
        Collider[] allColliders;
        List<Collider> _enemyColliders = new List<Collider>();
        List<Collider> _bossCollider = new List<Collider>();

        _numOfStriked = 0;

        playerMask = LayerMask.GetMask("Player");
        allColliders = Physics.OverlapSphere(transform.position, _radius);                                 // get all of the enemies that are within a search radius
        for (int i = 0; i < allColliders.Length; i++)
        {
            // get the enemies and the boss and add them to a list from the colliders that are surrounding the player
            if (allColliders[i].GetComponent<BaseAI>() != null)
            {
                if(allColliders[i].GetComponent<BaseBoss>() != null)
                {
                    _bossCollider.Add(allColliders[i]);
                }
                else if(allColliders[i].GetComponent<Tank>() || allColliders[i].GetComponent<Ranger>() || allColliders[i].GetComponent<Healer>())
                {
                    _enemyColliders.Add(allColliders[i]);
                }
            }
        }

        if (_enemyColliders.Count <= 0 && _bossCollider.Count <= 0)
        {
            float yPos = transform.position.y;
            yPos += _yOffset;
            spawnPos = new Vector3(transform.position.x, yPos, transform.position.z);

            // line renderer lightning
            _lightning = Instantiate(_lightningPrefab.gameObject, spawnPos, gameObject.transform.rotation).GetComponent<LineRenderer>();

            for (int j = 0; j < _lightning.positionCount; j++)
            {
                // if the current enemies index is 0 make the start position the players position
                startPos = transform.position;

                // check if there is an enemy in the next index
                endPos = _noEnemytarget.transform.position;

                // give the lightning random offsets
                float progress = (float)j / _lightning.positionCount;
                Vector3 arcPos = Vector3.Lerp(startPos, endPos, progress);                                                          // get the last enemies position and lerp the line to the new enemies position
                arcPos += UnityEngine.Random.onUnitSphere * _lightningMovement * UnityEngine.Random.value;
                _lightning.SetPosition(j, arcPos);
            }
        }
        else
        {
            AkSoundEngine.PostEvent(_lightningSound, gameObject);
            if(_enemyColliders.Count > 0)
            {
                for (int i = 0; i < _enemyColliders.Count; i++)
                {
                    if (_numOfStriked >= _maxNumOfStriked) { break; }                                                                // check if the max number of striked enemies has been reached. if true, break the loop. if false, continue the loop
                    if (i > _enemyColliders.Count) { break; }                                                                              // make sure the index isn't a bigger number than the number of enemies
                    if (_enemyColliders[i] == null) { continue; }                                                                              // make sure the current index of enemies isn't null

                    if (_enemyColliders[i].GetComponent<BaseAI>().isStruck == false)
                    {
                        if (_enemyColliders[i].gameObject.layer == playerMask.value) { continue; }                                          // if the current enemy is on the player layer. Then ignore it
                        Collider enemyCollider = _enemyColliders[i].GetComponent<Collider>();
                        HealthScript health = _enemyColliders[i].GetComponent<HealthScript>();

                        spawnPos = new Vector3(_enemyColliders[i].gameObject.transform.position.x, _enemyColliders[i].gameObject.transform.position.y, _enemyColliders[i].gameObject.transform.position.z);

                        // line renderer lightning
                        _lightning = Instantiate(_lightningPrefab.gameObject, spawnPos, _enemyColliders[i].gameObject.transform.rotation).GetComponent<LineRenderer>();

                        // make an offset for each of the lightnings position to make it look zig-zagged and like lightning
                        for (int j = 0; j < _lightning.positionCount; j++)
                        {
                            // if the current enemies index is greater than 0
                            if (i > 0)
                            {
                                // check if the current enemy dead
                                if (health._currentHealth <= 0)
                                {
                                    // if true set the start position as the player the current enemy died
                                    startPos = health.deathPos;
                                }
                                else
                                {
                                    // if the enemy is alive start from the current enemies position
                                    startPos = _enemyColliders[i].transform.position;
                                }
                            }
                            else
                            {
                                // if the current enemies index is 0 make the start position the players position
                                startPos = transform.position;
                            }

                            // check if there is an enemy in the next index
                            if (i + 1 >= _enemyColliders.Count)
                            {
                                // if false, set the endposition as the current enemies position
                                endPos = _enemyColliders[i].transform.position;
                            }
                            else
                            {
                                // if there is an enemy in the next index make the end position the next enemies position
                                endPos = _enemyColliders[i + 1].transform.position;
                            }

                            float progress = (float)j / _lightning.positionCount;
                            Vector3 arcPos = Vector3.Lerp(startPos, endPos, progress);                                                          // get the last enemies position and lerp the line to the new enemies position
                            _lightning.SetPosition(j, arcPos);
                        }

                        if (_lightning == null)
                        {
                            _lightning = Instantiate(_lightningPrefab.gameObject, spawnPos, gameObject.transform.rotation).GetComponent<LineRenderer>();
                        }

                        health.Damage(_damage, AttackType.MAGICAL, gameObject);                                                                  // damage the hit object

                        _enemyColliders[i].gameObject.GetComponent<BaseAI>().isStruck = true;
                        _numOfStriked++;
                        AkSoundEngine.PostEvent(_lightningImpactSound, _enemyColliders[i].gameObject);                                                  // play the impact sound when the lightning strikes an enemy
                        Destroy(_lightning.gameObject, _lifetime);                                                                                         // destroy the lightning strike
                        _lightning = null;
                        yield return new WaitForSeconds(_transferTime);                                                                          // wait x seconds befroe striking the next enemy\
                    }
                }
            }
            else if(_bossCollider.Count > 0)
            {
                for (int i = 0; i < _bossCollider.Count; i++)
                {
                    if (_numOfStriked >= _maxNumOfStriked) { break; }                                                                // check if the max number of striked enemies has been reached. if true, break the loop. if false, continue the loop
                    if (i > _bossCollider.Count) { break; }                                                                              // make sure the index isn't a bigger number than the number of enemies
                    if (_bossCollider[i] == null) { continue; }                                                                              // make sure the current index of enemies isn't null

                    if (_bossCollider[i].GetComponent<BaseAI>().isStruck == false)
                    {
                        if (_bossCollider[i].gameObject.layer == playerMask.value) { continue; }                                          // if the current enemy is on the player layer. Then ignore it
                        Collider enemyCollider = _bossCollider[i].GetComponent<Collider>();
                        HealthScript health = _bossCollider[i].GetComponent<HealthScript>();

                        spawnPos = new Vector3(_bossCollider[i].gameObject.transform.position.x, _bossCollider[i].gameObject.transform.position.y, _bossCollider[i].gameObject.transform.position.z);

                        // line renderer lightning
                        _lightning = Instantiate(_lightningPrefab.gameObject, spawnPos, _bossCollider[i].gameObject.transform.rotation).GetComponent<LineRenderer>();

                        // make an offset for each of the lightnings position to make it look zig-zagged and like lightning
                        for (int j = 0; j < _lightning.positionCount; j++)
                        {
                            // if the current enemies index is greater than 0
                            if (i > 0)
                            {
                                // check if the current enemy dead
                                if (health._currentHealth <= 0)
                                {
                                    // if true set the start position as the player the current enemy died
                                    startPos = health.deathPos;
                                }
                                else
                                {
                                    // if the enemy is alive start from the current enemies position
                                    startPos = _bossCollider[i].transform.position;
                                }
                            }
                            else
                            {
                                // if the current enemies index is 0 make the start position the players position
                                startPos = transform.position;
                            }

                            // check if there is an enemy in the next index
                            if (i + 1 >= _bossCollider.Count)
                            {
                                // if false, set the endposition as the current enemies position
                                endPos = _bossCollider[i].transform.position;
                            }
                            else
                            {
                                // if there is an enemy in the next index make the end position the next enemies position
                                endPos = _bossCollider[i + 1].transform.position;
                            }

                            float progress = (float)j / _lightning.positionCount;
                            Vector3 arcPos = Vector3.Lerp(startPos, endPos, progress);                                                          // get the last enemies position and lerp the line to the new enemies position
                            _lightning.SetPosition(j, arcPos);
                        }

                        if (_lightning == null)
                        {
                            _lightning = Instantiate(_lightningPrefab.gameObject, spawnPos, gameObject.transform.rotation).GetComponent<LineRenderer>();
                        }

                        health.Damage(_damage, AttackType.MAGICAL, gameObject);                                                                  // damage the hit object     

                        _bossCollider[i].gameObject.GetComponent<BaseAI>().isStruck = true;
                        _numOfStriked++;
                        AkSoundEngine.PostEvent(_lightningImpactSound, _bossCollider[i].gameObject);                                                  // play the impact sound when the lightning strikes an enemy
                        Destroy(_lightning.gameObject, _lifetime);                                                                                         // destroy the lightning strike
                        _lightning = null;
                        yield return new WaitForSeconds(_transferTime);                                                                          // wait x seconds befroe striking the next enemy\
                    }
                }
                foreach(Collider collider in _bossCollider)
                {
                    // reset the isStruck boolean of the boss
                    collider.gameObject.GetComponent<DeathBoss>().isStruck = false;
                }
            }

        }
    }

    private void OnDrawGizmos()
    {
        // gizmo for the boss search radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}