/*
* Copyright (C) Katherine Brough and Noah Greaves in Association with VFS
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthScript : MonoBehaviour
{
    // [SerializeField] private SO_HealthBar _;

    public float _currentHealth = 1;            //current/initial health
    private GameMaster gm;
    private float _hitParticlesduration = 2f;
    [SerializeField] private GameObject _hitParticles;
    [SerializeField] private float _partilceLifetime = 0.5f;
    [SerializeField] private float _gainAmount = 30;
    [SerializeField] private float stunAmount = 3;
    [HideInInspector] public float _maxHealth;
    //make current Health percentage visible
    public float HealthPercentage => (float)_currentHealth / _maxHealth;
    public event Action<float> HealthEvent = delegate { };

    private int _killCounter = 0;

    private GameObject hitParticlesClone;

    // Noah: player related sounds
    private string _playerTaunt = "Enemy_Taunt";
    private string _playerDamageSound = "Play_Kiin_Damaged";
    private string _playerDeathSound = "Play_Kiin_Dead";

    // Noah: enemy related sounds
    private string _tankDamagedSound = "Play_Tank_Damaged";
    private string _healerDamagedSound = "Play_Healer_Damaged";
    private string _rangerDamagedSound = "Play_Ranger_Damaged";

    private string _tankDeathSound = "Play_Enemy_Tank_Death";
    private string _rangerDeathSound = "Play_Enemy_Ranger_Death";
    private string _healerDeathSound = "Play_Enemy_Healer_Death";

    private string _playerGainMana = "Play_Mana_Restore";
    private string _playerGainHealth = "Play_Player_Heal";

    // Noah: UI related sounds
    private string _gameOverSound = "Play_UI_GameOver";

    public bool isStunned = false;

    [HideInInspector] public Vector3 deathPos;
    [SerializeField] private GameObject _healthParticles;
    [SerializeField] private GameObject _manaParticles;
    [SerializeField] private GameObject _explodingEnemy;
    private Vector3 enemyPosition;

    //on death Unity event
    //[SerializeField] private UnityEvent OnDeath;
    // private void OnEnable()
    // {
    //     if(CompareTag("Healer")) _healimage.healerfillamount += HealerDeath;
    // }
    // private void OnDisable()
    // {
    //     if(CompareTag("Healer")) _healimage.healerfillamount -= HealerDeath;
    // }

    private void Start()
    {
        _maxHealth = _currentHealth;                                        //Kate: setting Healthmax at start
        gm = GameMaster.instance;                                           //Kate: instance of game master singleton
    }

    public IEnumerator Stun()
    {
        isStunned = true;
        yield return new WaitForSeconds(stunAmount);
        isStunned = false;
    }

    //Kate: player damage and enemy damage when a resurrected enemy hits a normal enemy
    public void Damage(float amount)
    {
        //Kate:deal damage
        _currentHealth -= amount;


        //Kate:instantiate particles
        if (_hitParticles != null)
        {
            hitParticlesClone = Instantiate(_hitParticles, transform.position, transform.rotation);
        }

        //Kate:check if still alive
        if (_currentHealth <= 0)
        {
            // Debug.Log($"health: {_currentHealth}");
            Death();
        }
        HealthEvent?.Invoke(HealthPercentage);                                      //Noah: when the player damage, updates UI

        if (gameObject.GetComponent<PlayerController>())
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                GameMaster.instance._hurtshake2.GenerateImpulse();

                // Noah: play the damaged sound
                AkSoundEngine.PostEvent(_playerDamageSound, this.gameObject);
            }
            else
            {
                GameMaster.instance._hurtshake.GenerateImpulse();
            }
            GetComponent<PlayerController>()._Anim.Play("Hurt");                    //Kate:Play hurt animation for player 
        }


        Destroy(hitParticlesClone, _partilceLifetime);                              //destroy particles
    }

    private AttackType _attackType;
    //Noah: enemy damages when not resurrected, adds health and mana to player depending on attack type
    public void Damage(float amount, AttackType attack, GameObject player)          // PARAMS:  Amount: the amount of damage dealt, AttackType: AttackType: was the attack Magical or Physical, Player: the player object
    {
        //deal damage
        _currentHealth -= amount;                                                   // Noah:  damage the player

        _attackType = attack;

        //check if still alive
        if (_currentHealth <= 0.0f)                                                    // Noah: if the enemies health is below 0 kill them
        {
            Death(_attackType, gm.Player.gameObject, transform.position);
        }
        // Noah: play the damaged sound depending on what enemy was hit
        if (gameObject.GetComponent<Tank>())
        {
            GetComponent<Tank>()._anim.Play("Hurt");                          //Kate: Play Tank Hurt Animations
            AkSoundEngine.PostEvent(_tankDamagedSound, this.gameObject);
        }
        if (CompareTag("Boss"))
        {
            GetComponent<BaseBoss>()._anim.SetTrigger("Hurt");
        }

        if (CompareTag("Healer"))
        {
            AkSoundEngine.PostEvent(_healerDamagedSound, this.gameObject);
        }

        if (CompareTag("Ranger"))
        {
            AkSoundEngine.PostEvent(_rangerDamagedSound, this.gameObject);
        }

        //instantiate particles
        if (_hitParticles != null)                                                   // Noah: if the particles don't exist create them at the enemies position
        {
            Vector3 spawnPos = transform.position;
            spawnPos.y = transform.localScale.y / 2;
            hitParticlesClone = Instantiate(_hitParticles, spawnPos, transform.rotation);
        }

        HealthEvent?.Invoke(HealthPercentage);
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            GameMaster.instance._damageshake2.GenerateImpulse();
        }
        else
        {
            GameMaster.instance._damageshake.GenerateImpulse();
        }

        if (CompareTag("Tank"))
        {
            GetComponent<Tank>()._anim.SetTrigger("Hurt");                          //Kate: Play Tank Hurt Animations
        }
        if (CompareTag("Ranger"))
        {
            GetComponent<Ranger>()._animator.SetTrigger("Hurt");
        }

        Destroy(hitParticlesClone, _partilceLifetime);                              // destroy particles
    }

    //called on Death for player and for enemies the resurrected enemies kill
    public void Death()
    {
        if (gameObject.GetComponent<PlayerController>())
        {

           // GetComponent<PlayerController>()._Anim.SetTrigger("Deathy");
            GetComponent<PlayerController>()._Anim.SetBool("Death", true);
            AkSoundEngine.PostEvent(_gameOverSound, this.gameObject);                    // Noah: Play the Game Over sound when the player dies
            AkSoundEngine.PostEvent(_playerDeathSound, gameObject);

            //Kate: setting death count to 0
            GameMaster.instance.DeathCount["Ranger"] = 0;
            GameMaster.instance.DeathCount["Tank"] = 0;
        }
        else
        {
            if (_explodingEnemy != null)
            {
                ExplodeEnemy();
            }
            Destroy(gameObject);
        }
    }

    //Noah: called on Death for enemies 
    public void Death(AttackType attack, GameObject player, Vector3 position)       // PARAMS: AttackType: was the attack Magical or Physical, Player: the player object, Position: The position the enemy died at
    {
        deathPos = position;                                                        // Noah: The position that the enemy died at

        if (player == null) { return; }

        // TODO: Everytime the player kills 5 enemies the player will taunt the enemies
        // _killCounter += 1;
        // Debug.Log($"kill counter: {_killCounter}");
        // if(_killCounter >= 5)
        // {
        //     AkSoundEngine.PostEvent(_playerTaunt, player);                          // Noah: Play the Taunt Voice
        //     Debug.Log("hello");
        //     _killCounter = 0;
        // }
        if (CompareTag("Boss"))
        {
            GetComponent<BaseBoss>()._anim.SetTrigger("Death");
        }

        if (GameMaster.instance.DeathCount.ContainsKey(tag) == false)                //Kate: null check for Deathcount list
        {
            GameMaster.instance.DeathCount.Add(tag, 0);
        }
        if (GameMaster.instance._ressurectionbuildup.ContainsKey(tag) == false)                //Kate: null check for Deathcount list
        {
            GameMaster.instance._ressurectionbuildup.Add(tag, 0);
        }
        GameMaster.instance._ressurectionbuildup[tag]++;

        if (GameMaster.instance._ressurectionbuildup.ContainsKey("Ranger"))
        {
            // Debug.Log("RANGER DEATH: " + GameMaster.instance._ressurectionbuildup[tag]);
        }
        if (GameMaster.instance._ressurectionbuildup[tag] == 3)                            //makes it so you you ressurect 1 enemy for every 3 you kill
        {
            GameMaster.instance.DeathCount[tag]++;                                      //Kate: Adds name and number to death count list in gamemaster for resurrection spell                                                                           // GameMaster.instance._ressurectionbuildup[tag] = 0;
        }

        if (attack == AttackType.MAGICAL)                                            // Noah: if the attack was a magic attack give the player health
        {
            GiveHealth(player);
        }
        else if (attack == AttackType.PHYSICAL)                                      // Noah: if the attack was physical give the player mana
        {
            GiveMana(player);
        }

        if (gameObject.GetComponent<Tank>() || gameObject.GetComponent<Ranger>())
        {
            //Debug.Log("titan deathcount:" + GameMaster.instance.DeathCount["Tank"] + "titan ressurect build up:" + GameMaster.instance._ressurectionbuildup["Tank"]);
            BaseAI.AIList.Remove(gameObject.GetComponent<Tank>());                           //Kate: removes tank from tank list for enemy distance
        }

        if (_explodingEnemy != null)
        {
            ExplodeEnemy();
        }
        if(!gameObject.GetComponent<BaseBoss>())
        {
        Destroy(gameObject);
        }
    }

    private void ExplodeEnemy()
    {
        if (CompareTag("Tank"))
        {
            Instantiate(_explodingEnemy, gameObject.transform.position, gameObject.transform.rotation);
            AkSoundEngine.PostEvent(_tankDeathSound, gameObject);
        }

        if (CompareTag("Ranger"))
        {
            Instantiate(_explodingEnemy, gameObject.transform.position, gameObject.transform.rotation);
            AkSoundEngine.PostEvent(_rangerDeathSound, gameObject);
        }

        if (CompareTag("Healer"))
        {
            Instantiate(_explodingEnemy, gameObject.transform.position, gameObject.transform.rotation);
            AkSoundEngine.PostEvent(_healerDeathSound, gameObject);
        }
    }

    private void GiveHealth(GameObject player)
    {
        HealthScript playerHealth = player.GetComponent<HealthScript>();                        // Noah: get the players health
        if (playerHealth == null) { return; }

        // Noah: Add to the players health
        playerHealth._currentHealth += _gainAmount;
        if (playerHealth._currentHealth < _maxHealth)
        {
            playerHealth._currentHealth = _maxHealth;
        }

        // Noah: update the health bar
        HealthEvent?.Invoke(HealthPercentage);
        HealthBar.RuntimeSOBar.HealthBarRef.UpdateBar(playerHealth.HealthPercentage);

        // Noah: Instantiate the particles and lerp them to the enemies position
        enemyPosition = transform.position;
        _healthParticles = Instantiate(_healthParticles, transform.position, transform.rotation);
        _healthParticles.GetComponent<ParticleLerp>().Particle = _healthParticles;
        _healthParticles.GetComponent<ParticleLerp>().EnemyPosition = enemyPosition;
        AkSoundEngine.PostEvent(_playerGainMana, gameObject);
    }

    private void GiveMana(GameObject player)
    {
        Mana playerMana = player.GetComponent<Mana>();
        if (playerMana == null) { return; }

        // Noah: Add to the player mana and update the mana bar
        playerMana._manaAmount += _gainAmount;
        if (playerMana._manaAmount < playerMana._maxManaAmount)
        {
            playerMana._manaAmount = playerMana._maxManaAmount;
        }

        playerMana.InvokeEventMana();

        // Noah: Instantiate the particles and lerp them to the enemies position
        enemyPosition = gameObject.transform.position;
        _manaParticles = Instantiate(_manaParticles, transform.position, transform.rotation);
        _manaParticles.GetComponent<ParticleLerp>().Particle = _manaParticles;
        _manaParticles.GetComponent<ParticleLerp>().EnemyPosition = enemyPosition;
        AkSoundEngine.PostEvent(_playerGainHealth, gameObject);
    }

    public void InvokeEventHealth()
    {
        HealthEvent?.Invoke(HealthPercentage);
    }
}
