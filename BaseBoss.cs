/*
* Copyright (C) Noah Greaves in Association with VFS
*/
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Cinemachine;

public enum State
{
    IDLE,
    ATTACK
}

public enum HealthState
{
    FULL,
    STAGEONE,
    STAGETWO,
    STAGETHREE,
    STAGEFOUR,
    DEAD
}

public class BaseBoss : BaseAI
{
    // Boss related variables
    [SerializeField] protected float _damage;
    [SerializeField] protected float _cooldown;                       // the amount of time it takes for the boss to attack again
    [SerializeField] protected float _searchForTargetRadius;        // the radius the boss will look for the player within
    protected float attackTimer = 0;                                // the timer for the cooldown
    protected HealthScript _bossHealth;

    // Boss Health related variables
    [SerializeField] protected float _stageOneHealth = 0.85f;       // 75%  total health    // the amount of health the boss has before being able to use their next move after the basic attack
    [SerializeField] protected float _stageTwoHealth = 0.60f;       // 50%  total health    // the amount of health the boss has before being able to use their next move after the stage one attack
    [SerializeField] protected float _stageThreeHealth = 0.50f;     // 25%  total health    // the amount of health the boss has before being able to use their next move after the stage two attack
    [SerializeField] protected float _stageFourHealth = 0.25f;      // 10%  total health    // the amount of health the boss has before being able to use their next move after the stage two attack
    [SerializeField] private GameObject _bossspawnparticle;
    //kate: adding titan and boss switch script for boss fight
    [SerializeField] GameObject _titanControllerObject;
    private BossTitanController _bosstitanscript;
    public bool _switchbaseboss = false;
    private float _bothcounter = 0;
    
    [SerializeField] private MusicContorller _musicController;

    [SerializeField] protected GameObject _titan;

    protected HealthState _curHealthState = HealthState.FULL;

    // Player related variables
    protected PlayerController _player;
    protected HealthScript _playerHealth;
    protected Transform _playerTransform;
    
    // AI behavior related variables
    protected State _currentState = State.IDLE;
    protected Attack _currentAttack;

    // Animation related variablesa
    [HideInInspector]
    public Animator _anim;

    private Collider[] _targetColliders;


    // A virtual function so the children classes can choose their attacks based on what God/Boss they are
    protected virtual void SetAttack(Attack newAttack) {}

    // A virtual function that determines the gods attack based on the amount of health they have 
    protected virtual void DetAttack() {}

    // A virtual function that will randomize the attacks the God/Boss will have 
    protected virtual Attack RandAttack(float atk1, float atk2) { return Attack.NULL; }

    protected override void Awake()
    {
        base.Awake();    
    }

    protected override void Start()
    {
        base.Start();
        GetReferences();
        SetValues();
        _titan.SetActive(false);
    }

    private void GetReferences()
    {
        _bosstitanscript = _titanControllerObject.GetComponent<BossTitanController>();
        _playerHealth = GameMaster.instance.Player.GetComponent<HealthScript>();
        self = GetComponent<NavMeshAgent>();
        // Debug.Log($"self: {self}");
    }

    private void SetValues()
    {
        _currentAttack = Attack.NULL;
        _player = GameMaster.instance.Player;
        _playerTransform = GameMaster.instance.Player.transform;
    }

    private void SpawnTitan()
    {
        // Instantiate(_titan, new Vector3(996.7f, -68.0f, 21.9f), Quaternion.identity);
    }

    // look for the player within a radius
    protected State SearchForTarget()
    {
        _targetColliders = Physics.OverlapSphere(transform.position, _searchForTargetRadius);
        foreach (Collider collider in _targetColliders)
        {
            _player = collider.gameObject.GetComponent<PlayerController>();
            if (_player == null) 
            { 
                // Debug.Log("player is null");
                _currentState = SetState(State.IDLE);
            }

            else if (_player != null)
            {
                // Debug.Log("player is NOT null");
                _currentState = SetState(State.ATTACK);
                MoveTowards();
            }
        }
        return _currentState;
    }

    // set the current state to the new state and start the new state
    protected State SetState(State newState)
    {
        // Debug.Log($"State was set to {newState}");
        _currentState = newState;
        if(_currentState == State.ATTACK)
        {
           // Debug.Log("State is ATTACK");
            DetAttack();
        }
        return _currentState;
    }

    // determines the state of the bosses health and depending on the state determine the bosses next attack
    protected HealthState DetHealthState()
    {
        if(_bossHealth.HealthPercentage <= _stageOneHealth && _bossHealth.HealthPercentage > _stageTwoHealth)
        {
            if(_switchbaseboss == false) 
            {
                GameMaster.instance._explosionshake.GenerateImpulse();
            Destroy(Instantiate(_bossspawnparticle, this.gameObject.transform.position, this.gameObject.transform.rotation), 2f);
            _bosstitanscript.TitanActive();                //kate: calling the function that makes deathboss inactive and titan active

            }
            _curHealthState = HealthState.STAGEONE;
            _musicController.BossHealthSwitchChange("BossHealth", "High");
          //  Debug.Log("STAGE 1");
        }

        if(_bossHealth.HealthPercentage <= _stageTwoHealth && _bossHealth.HealthPercentage > _stageThreeHealth)
        {
            _curHealthState = HealthState.STAGETWO;
            _musicController.BossHealthSwitchChange("BossHealth", "Medium");
            _switchbaseboss = false;
         //   Debug.Log("STAGE 2");
        }

        if(_bossHealth.HealthPercentage <= _stageThreeHealth && _bossHealth.HealthPercentage > _stageFourHealth)
        {
            _curHealthState = HealthState.STAGETHREE;
            if(_switchbaseboss == false)
            {
                GameMaster.instance._explosionshake.GenerateImpulse();
            Destroy(Instantiate(_bossspawnparticle, this.gameObject.transform.position, this.gameObject.transform.rotation), 2f);
            _bosstitanscript.TitanActive();                              //kate: calling the function that makes deathboss inactive and titan active
            }
                
            _musicController.BossHealthSwitchChange("BossHealth", "Low");
         //   Debug.Log("STAGE 3");
        }

        if(_bossHealth.HealthPercentage <= _stageFourHealth && _bossHealth.HealthPercentage  > 0.01f)
        {
            _curHealthState = HealthState.STAGEFOUR;
            if(_bothcounter == 0)
            {
            GameMaster.instance._explosionshake.GenerateImpulse();
            Destroy(Instantiate(_bossspawnparticle, this.gameObject.transform.position, this.gameObject.transform.rotation), 2f);
            _bosstitanscript.BothActive(); 
            _bothcounter++;                                     //Kate: calling the function that makes the death god and the titan active
            }
            _musicController.BossHealthSwitchChange("BossHealth", "Very_Low");
         //   Debug.Log("STAGE 4");
        }

        if(_bossHealth.HealthPercentage <= 0.01f)
        {
            _curHealthState = HealthState.DEAD;
            _musicController.BossHealthSwitchChange("BossHealth", "Dead");
        //    Debug.Log($"health state DEAD: {_bossHealth.HealthPercentage}");
        }

    //    Debug.Log($"health state: {_curHealthState}");
        return _curHealthState;
    }

    protected override void Update()
    {
        base.target = GameMaster.instance.Player.transform;
        
        if (base.target == null) { return; }

        base.Update();
        
       // base.Update();

        // every frame look for the player
        _currentState = SearchForTarget();

        if(_currentState == State.IDLE)
        {
            //TODO: When we get the Boss Animations, Play Idle animation
        }
        else if(_currentState == State.ATTACK)
        {
            transform.LookAt(_player.transform);
            transform.eulerAngles = new Vector3(0f, transform.eulerAngles.y, 0f);       
        }
    }
}