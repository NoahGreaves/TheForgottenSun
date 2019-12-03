/*
* Copyright (C) Noah Greaves in Association with VFS
*/

using UnityEngine;
using ObjectPooling;
using System;
using System.Collections;

public class DeathBoss : BaseBoss
{
    // Hands Of Dead related variables
    [SerializeField] private int _maxNumOfSpawnedHands;
    [SerializeField] private float _spawnHandsInRadius;
    [SerializeField] private float _handDamageRadius;
    [SerializeField] private PoolableObject _hands;


    // Lifesteal related variables
    // [SerializeField] private LineRenderer _lifesteal;
    [SerializeField] private GameObject _lifesteal;
    [SerializeField] private float _lifestealMovement = 2f;

    [SerializeField] private Canvas _playerWin;

    //spawn enemies
    [SerializeField] private float _spawnradius;
    [SerializeField] private GameObject _Healer;
    [SerializeField] private GameObject _Ranger;
    [SerializeField] private GameObject _Tank;
    [SerializeField] private int _rangerspawned = 1;
    [SerializeField] private int _tanksspawned = 3;
    [SerializeField] private int _healersspawned = 2;

    [SerializeField] private float _tauntCooldown = 5.0f;
    [SerializeField] private GameObject _bossparticle;
    private float _tauntTimer;

    // sounds
    private string _playerTauntBoss = "Boss_Taunt";
    private string _bossDefeatSound = "Boss_Defeat";
    private string _summonDead = "Play_Summon_Dead";
    private string _bossTaunt = "Play_Death_God_Taunt";
    private string _deathSound = "Play_Death_God_Dead";
    private string _scytheSound = "Play_Death_God_Scyth_Swing";
    private string _magicEffort = "Play_Death_God_Magic_Efforts";
    private string _raiseDeadSound = "Play_Death_God_Summon_Dead";
    private string _healthStateChangeSound = "Play_Death_God_Transform";

    private int _numOfSpawned;
    private int _numOfHands;

    private float _currentHealth;
    private float _newHealth;

    private Vector3 startPos;
    private Vector3 endPos;

    protected override void Start()
    {
        base.Start();
        _playerWin.gameObject.SetActive(false);
        _bossHealth = gameObject.GetComponent<HealthScript>();
        //   Debug.Log($"boss health: {_bossHealth.gameObject.name}");
        _numOfHands = _maxNumOfSpawnedHands;

        _anim = gameObject.GetComponent<Animator>();
        _anim.Play("Movement");

        _player = GameMaster.instance.Player;

        GameMaster.instance.ObjectPooling();

        // set the starting state to idle 
        _currentState = SetState(State.IDLE);
    }

    private void SummonDeadHands()
    {
        _anim.Play("CastMagic");
        AkSoundEngine.PostEvent(_magicEffort, gameObject);

        Vector3 center = transform.position;                                                                // the center of the circle is the player
        _numOfSpawned = 0;                                                                                  // keeps track of the number of spawned hands
        for (int i = 0; i < _numOfHands; i++)
        {
            // spawn the hands at random point on a radius around the Boss
            if (_maxNumOfSpawnedHands >= _numOfSpawned)
            {
                Vector3 position = RandCircleSpawn(center, _spawnHandsInRadius);                            // spawn the hands in a cirlce around the player
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, center - position);        // make the hands face away from the player
                PoolManager.GetNext(_hands, position, rotation);
                _numOfSpawned++;
            }
        }

        _currentAttack = Attack.NULL;                                      // after the attack set the attack back to null so it randomizes the attack again
    }

    private void SpawnEnemies()                                         //Kate: spawn enemies attack for boss
    {
        //   Debug.Log("IN SPAEWN ENEMIES");

        //Vector3 spawnposition = RandCircleSpawn(transform.position, _spawnradius);
        for (int i = 0; i < _healersspawned; i++)
        {
            Vector3 spawnposition = RandCircleSpawn(transform.position, _spawnradius);
            //int randomnum = UnityEngine.Random.Range(1,20);
            Destroy(Instantiate(_bossparticle, spawnposition, transform.rotation), 2.0f);
            Instantiate(_Healer, spawnposition, transform.rotation);
        }

        for (int i = 0; i < _rangerspawned; i++)
        {
            Vector3 spawnposition = RandCircleSpawn(transform.position, _spawnradius);
            // int randomnum = UnityEngine.Random.Range(1,20);
            Destroy(Instantiate(_bossparticle, spawnposition, transform.rotation), 2.0f);
            Instantiate(_Ranger, spawnposition, transform.rotation);
        }

        for (int i = 0; i < _tanksspawned; i++)
        {
            Vector3 spawnposition = RandCircleSpawn(transform.position, _spawnradius);
            //int randomnum = UnityEngine.Random.Range(1,20);
            Destroy(Instantiate(_bossparticle, spawnposition, transform.rotation), 2.0f);
            Instantiate(_Tank, spawnposition, transform.rotation);
        }
        //   Debug.Log("LEAVING SPAEWN ENEMIES");

        AkSoundEngine.PostEvent(_raiseDeadSound, gameObject);
        AkSoundEngine.PostEvent(_summonDead, gameObject);
        _currentAttack = Attack.NULL;
    }

    private Vector3 RandCircleSpawn(Vector3 center, float radius)
    {
        // make a random angle between 0 and 360
        float ang = UnityEngine.Random.value * 360;
        Vector3 position;
        position.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        position.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        position.y = center.y;

        return position;
    }

    private void DeathCheck()
    {
        if (_curHealthState == HealthState.DEAD)
        {
            //   Debug.Log("boss is dead");

            AkSoundEngine.PostEvent(_deathSound, gameObject);
            AkSoundEngine.PostEvent(_bossDefeatSound, _player.gameObject);
            Debug.Log("in deathcheck deathboss");
            _playerWin.gameObject.SetActive(true);
            Time.timeScale = 0.0f;
            Destroy(_titan);
            Destroy(gameObject);
            return;
        }
    }


    // Determines the gods attack depending on the amount of health they have and when the cooldown has finished
    protected override void DetAttack()
    {
        attackTimer += Time.deltaTime;
        if (_cooldown < attackTimer)
        {
            attackTimer = 0;
            Attack chosenAttack = Attack.NULL;                       // set the attack to null

            // depending on the amount of health that the boss has determine their next attack
            switch (_curHealthState)
            {
                case HealthState.FULL:
                    chosenAttack = Attack.SCYTHE;
                    break;

                case HealthState.STAGEONE:
                    AkSoundEngine.PostEvent(_healthStateChangeSound, gameObject);
                    chosenAttack = RandAttack(0.0f, 1.0f);
                    break;

                case HealthState.STAGETWO:
                    AkSoundEngine.PostEvent(_healthStateChangeSound, gameObject);
                    chosenAttack = RandAttack(0.0f, 2.0f);
                    break;

                case HealthState.STAGETHREE:
                    AkSoundEngine.PostEvent(_healthStateChangeSound, gameObject);
                    chosenAttack = RandAttack(0.0f, 3.0f);
                    break;

                case HealthState.STAGEFOUR:
                    AkSoundEngine.PostEvent(_healthStateChangeSound, gameObject);
                    chosenAttack = RandAttack(0.0f, 3.0f);
                    break;

                default:
                    Debug.Log("ERROR: Default case when determing the attack");
                    break;
            }
            // Debug.Log($"chosen attack: {chosenAttack}");
            SetAttack(chosenAttack);
        }
    }

    protected override Attack RandAttack(float atk1, float atk2)
    {
        Attack chosenAttack = Attack.NULL;                      // SetAttack the attack to null
        float rand = UnityEngine.Random.Range(atk1, atk2);        // come up with a random value
        rand = Mathf.Ceil(rand);
        if (rand > 2)
        {
            rand = 2;
        }
        // depending on that random value choose that attack
        //  Debug.Log("rand is:" + rand);
        switch (rand)
        {
            case 0.0f:
                // Debug.Log("SCYTHE");
                chosenAttack = Attack.SCYTHE;
                return chosenAttack;

            case 1.0f:
                //Debug.Log("SPAWNENEMIES");
                chosenAttack = Attack.SPAWNENEMIES;
                return chosenAttack;

            case 2.0f:
                //   Debug.Log("SUMMONDEADHANDS");
                chosenAttack = Attack.SUMMONDEADHANDS;
                return chosenAttack;

            default:
                // Debug.Log("NULL");
                chosenAttack = Attack.NULL;
                return chosenAttack;
        }
    }

    // based on the new attack that was passed in, use it on the player
    protected override void SetAttack(Attack newAttack)
    {
        if (newAttack == Attack.SCYTHE)
        {
            // plays the basic melee attack
            _anim.Play("Attack");
            AkSoundEngine.PostEvent(_scytheSound, gameObject);
            return;
        }

        else if (newAttack == Attack.SPAWNENEMIES)
        {
            SpawnEnemies();
        }

        else if (newAttack == Attack.SUMMONDEADHANDS)
        {
            // Summon the Hands of the Dead spell
            SummonDeadHands();
        }
    }

    // a function to check if the boss has been damged and to play a sound if they have been
    private void HealthCheck(float health)
    {
        _newHealth = health;
        if (_newHealth < health)
        {
            AkSoundEngine.PostEvent(_healthStateChangeSound, gameObject);
        }
    }

    // after a cooldown the boss will taunt the player 
    private void BossTaunt()
    {
        if (_player != null && _tauntTimer < _tauntCooldown)
        {
            AkSoundEngine.PostEvent(_bossTaunt, gameObject);
        }
        else if (_player != null)
        {
            _tauntTimer += Time.deltaTime;
        }
    }

    protected override void Update()
    {
        _currentHealth = _bossHealth._currentHealth;
        HealthCheck(_currentHealth);
        base.DetHealthState();
        DeathCheck();
        BossTaunt();
        base.Update();
        base.LookAtTarget();
    }

    private void OnDrawGizmos()
    {
        // gizmo for the boss search radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _searchForTargetRadius);

        // gizmo for the radius that the hands spawn in
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _spawnHandsInRadius);
    }

    //  private void MoveTitan(Vector3 _distancebetweenfist)                   //kate: moves boss to an accurate distance to hit player
    // {
    //         _currentLerpTime += Time.deltaTime;
    //         if (_currentLerpTime > _lerptime)
    //         {
    //             _currentLerpTime = _lerptime;
    //             _currentLerpTime = 0;
    //         }
    //        // Debug.Log("in MoveTitan Titan");
    //         float perc = _currentLerpTime / _lerptime;
    //         Vector3 _pointofinterest = _player.position - _distancebetweenfist;
    //         _pointofinterest.y = this.gameObject.transform.position.y;
    //         this.gameObject.transform.position = Vector3.Lerp(transform.position, _pointofinterest, perc);

    // }
}
