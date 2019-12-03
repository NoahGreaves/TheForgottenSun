/*
* Copyright (C) Noah Greaves in Association with VFS
*/

using System;
using System.Collections;
using UnityEngine;

public class BeeBomb : MonoBehaviour
{
    public int _numOfBees = 50;
    public GameObject _beeSwarmController;
    public GameObject _beeSubSwarmController;
    [SerializeField] private GameObject _beeSwarmTarget;
    [SerializeField] private GameObject _beeSubSwarmTarget;
    [SerializeField] private GameObject _bomb;
    [SerializeField] private float _beeLifetime = 1.0f;

    // Bee behaviour variables
    [SerializeField] private int _maxBeeTargets = 1;

    [SerializeField] private float _fuse;
    [SerializeField] private float _radius = 5.0f;
    [SerializeField] private float _damage = 10.0f;

    [SerializeField] private LayerMask _reserectedEnemyLayer;

    [SerializeField,
    Tooltip("The amount of power the explosion has")]
    private float _power = 10.0f;

    [SerializeField] private float _throwPower = 500;
    private Rigidbody _rb;
    private WeakPoints _weakPoints = new WeakPoints();
    private GameObject _bombClone;
    private PlayerController _player;
    private Vector3 _pos;

    private GameObject beeSubSwarmTargetClone;
    private GameObject beeSubSwarmContollerClone;
    private JobsBoidController subController;

    private string _bombThrowSound = "Play_Throw_Bee_Bomb";
    private string _bombExplodeSound = "Ply_Bee_Bomb_Explode";

    public void StartBomb()
    {
        _player = GameMaster.instance.Player;
        // _player = FindObjectOfType<PlayerController>();

        // make an offset for the bomb to spawn at so it doesn't come out of the players feet
        float posY = _player.transform.position.y;
        float yOffset = 1;
        posY += yOffset;
        Vector3 spawnPos = new Vector3(_player.transform.position.x, posY, _player.transform.position.z);
        _bombClone = Instantiate(_bomb, spawnPos, new Quaternion(_player.transform.rotation.x, _player.transform.rotation.y, _player.transform.rotation.z, _player.transform.rotation.w));
        _rb = _bombClone.GetComponent<Rigidbody>();
        StartCoroutine(ThrowBomb());
    }

    private void Update()
    {
        if (_rb != null)
            _pos = _rb.transform.position;
    }

    // _rigidbody.transform.eulerAngles = new Vector3(transform.eulerAngles.x, degrees + _CameraBase.eulerAngles.y, transform.eulerAngles.z);

    // throwing the bomb forward, waiting for the fuse timer to end, and then explode the bomb
    [SerializeField] private Transform _camera;
    private IEnumerator ThrowBomb()
    {
        // throw the bomb forward
        Vector3 _throwDirection = _camera.forward * _throwPower;

        // make the bomb go farther in distance than it goes in height
        _throwDirection.y = transform.up.y * (_throwPower * 0.75f);

        _rb.AddForce(_throwDirection);
      //  Debug.Log($"throw direction: {_throwDirection}");
        AkSoundEngine.PostEvent(_bombThrowSound, gameObject);

        yield return new WaitForSeconds(_fuse);

        // explode into bees
        Explode();

        // destroy the bomb after the fuse timer
        Destroy(_bombClone);
        StopCoroutine(ThrowBomb());
    }

    private void Explode()
    {
        // play the bee bomb sound
        LayerMask playerMask = LayerMask.GetMask("Player");

        // spawn main swarm of the boid bees
        GameObject beeSwarmTargetClone = Instantiate(_beeSwarmTarget, _bombClone.transform.position, _bombClone.transform.rotation);
        GameObject beeSwarmContollerClone = Instantiate(_beeSwarmController, _bombClone.transform.position, _bombClone.transform.rotation);
        beeSwarmContollerClone.GetComponent<JobsBoidController>().SetPostion(beeSwarmTargetClone.transform);

        AkSoundEngine.PostEvent(_bombExplodeSound, beeSwarmContollerClone.gameObject);

        // Set the SubSwarm Targets
        SetBeeTargets();

        // explosion position
        Vector3 explosionPos = _rb.transform.position;

        // give all of the spawned bees explosion force
        Collider[] targets = Physics.OverlapSphere(explosionPos, _radius);
        foreach (Collider target in targets)
        {
            BaseAI enemy = target.GetComponent<BaseAI>();

            // if there is no enemy and/or if the overlapped collider is the player, skip this iteration of the loop
            if (target.gameObject.layer == playerMask.value) { continue; }

            // Find the boss and and if the player hit the weak point. deal extra damage TODO: Boss getting returned as NULL, FIX THIS
            if (enemy == null) { continue; }

            Rigidbody rb = target.gameObject.GetComponent<Rigidbody>();
            HealthScript enemyHealth = target.gameObject.GetComponent<HealthScript>();
            if (enemyHealth == null) { continue; }
            else
            {
                Debug.Log("boss is not null");
                _weakPoints.FindBosses(targets, _damage);
            }

            subController._boidPrefab.EnemyHealth = enemyHealth;

            DeathBoss deathBoss = enemyHealth.GetComponent<DeathBoss>();
            if (deathBoss == null)
            {
                Debug.Log("boss is null");
                continue;
            }
            // enemyHealth.Damage(_damage, AttackType.MAGICAL, gameObject);
            rb.AddExplosionForce(_power, explosionPos, _radius, 3.0f);
        }
    }

    // sets the target for the sub swarms
    private void SetBeeTargets()
    {
        Vector3 searchPosition = _rb.transform.position;
        Collider[] target = Physics.OverlapSphere(searchPosition, _radius);

        int enemyCounter = 0;
        for (int i = 0; i < target.Length; i++)
        {
            // make sure the target is an enemy and not a random object
            if (target[i].GetComponent<BaseAI>() != null)
            {
                enemyCounter++;
                if ((1 << target[i].gameObject.layer) == _reserectedEnemyLayer) { continue; }
                if (enemyCounter > _maxBeeTargets) { break; }
                if (target[i].gameObject.transform == null) { continue; }

                // spawn the boid bees
                beeSubSwarmContollerClone = Instantiate(_beeSubSwarmController, target[i].transform.position, target[i].transform.rotation);
                StartCoroutine(WaitForFrame());
                beeSubSwarmTargetClone = Instantiate(_beeSubSwarmTarget, target[i].transform.position, target[i].transform.rotation);
                subController = beeSubSwarmContollerClone.GetComponent<JobsBoidController>();
                subController.SetPostion(target[i].transform);
            }
        }
    }

    private IEnumerator WaitForFrame()
    {
        yield return new WaitForEndOfFrame();
        StopCoroutine(WaitForFrame());
    }

    private void OnDrawGizmos()
    {
        // gizmo for the boss search radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}