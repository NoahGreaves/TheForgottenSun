/*
* Copyright (C) Noah Greaves in Association with VFS
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class SpearThrow : MonoBehaviour
{
    [SerializeField] private int _throwDistance;
    [SerializeField] private float _radius;
    [SerializeField] private float _power;
    [SerializeField] private float _damage;
    [SerializeField] private float _cooldown = 3.0f;
    [SerializeField] private GameObject _dashIndicator;
    [SerializeField] private float _dashTime = 0.5f;
    private Rigidbody _rb;

    [SerializeField] private PostProcessProfile _postProcessProfile;
    [SerializeField] private float _distortionAmount = 5.0f;
    private ChromaticAberration _chromaticAberration;

    [HideInInspector] public bool canDash = false;

    private float _dashTimer = 0.0f;
    private float curLerptime = 0.0f;
    private float _rayDistance = 100;
    private int _indecatorCounter;
    private float _xScale;
    private float _yScale;
    private WeakPoints _weakPoints = new WeakPoints();
    private Vector3 _destination;
    private bool _isDashing;
    private GameObject _dashIndicatorClone;
    private Collider _collider;
    private bool _uiActive;
    public bool _deactivate = false;

    public event Action<float> DashCooldownEvent = delegate { };
    public float CooldownPercentage => (float)_dashTimer / _cooldown;

    // [SerializeField] private float _maxDashAmount;
    // [SerializeField] private float _regenAmount;
    // [SerializeField] private float _dashSpeed;
    // private float _dashAmount;

    [SerializeField] private Image _dashGlow;
    [SerializeField] private RawImage _dashIcon;
    private bool cooldownDone = false;

    private string _playTeleportSound = "Play_Teleport";
    private string _stopTeleportSound = "Stop_Teleport";

    private void Awake()
    {;
        _deactivate = false;
        GetReferences();
        
    }

    private void Start()
    {
        _xScale = _dashIndicator.transform.localScale.x;
        _yScale = _dashIndicator.transform.localScale.y;
        _rayDistance = _throwDistance;
    }

    private void Update()
    {
    //     if(_deactivate == false)
    // {
        Cooldown();
    //}
    }

    private void GetReferences()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
    }


    private void Cooldown()
    {
        // checks if the player has dashed
        if (cooldownDone == false)
        {
            if (_dashTimer <= 0)
            {
                _dashTimer = _cooldown;
                cooldownDone = true;
                DashCooldownEvent?.Invoke(CooldownPercentage);
            }
            else
            {
                _dashGlow.enabled = false;
                _dashTimer -= Time.deltaTime;
                DashCooldownEvent?.Invoke(CooldownPercentage);
            }
        }
        else
        {
            // _dashIcon.enabled = true;
            _dashGlow.enabled = true;
            CheckForThrow();
        }
    }

    private void FixedUpdate()
    {
        if (_uiActive == true && _dashIndicatorClone == null)
        {
            if (canDash == true)
            {
                DashUI();
            }
        }

        // Keep the Dash indecator infront of the player to tell them the location they will teleport/dash to 
        if (_dashIndicatorClone != null)
        {
            Vector3 wallHitPos;
            if (HitWall(out wallHitPos))
            {
                _dashIndicatorClone.transform.position = wallHitPos;
            }
            else
            {
                Vector3 dest = transform.position + _rb.velocity.normalized * _throwDistance;
                Ray _downRay2 = new Ray(dest, Vector3.down);
                RaycastHit downHit;
                if (Physics.Raycast(_downRay2, out downHit, _rayDistance, levelLayer))
                {
                    dest = downHit.point;
                }

                // _dashIndicatorClone.transform.position = transform.position + transform.forward * _throwDistance;
                _dashIndicatorClone.transform.position = dest;
            }
        }
    }

    private void CheckForThrow()
    {
        // show the location indicator for the dash
        if (Input.GetButtonDown("Jump"))
        {
            _uiActive = true;
            canDash = true;
        }
        // destroy the indicator and dash the player forward
        else if (Input.GetButtonUp("Jump"))
        {
            _uiActive = false;
            // _flashDash = false;
            cooldownDone = false;
            Dash();

        }
    }

    private void DashUI()
    {
        _xScale = _radius;                                                              // the size of the indicator, same size as the explosion radius
        _yScale = _radius;

        _dashIndicatorClone = Instantiate(_dashIndicator, _destination, gameObject.transform.rotation);
    }

    private void Dash()
    {
        if (canDash == true)
        {
            Vector3 wallHitPos;
            if (HitWall(out wallHitPos))
            {
                // if the raycast hit a wall set the desination to that point on the wall
                _destination = wallHitPos;
            }
            else
            {
                // if the raycast didn't hit a wall set the destination to be the indicators positoin
                _destination = _dashIndicatorClone.transform.position;
                _destination.y += Vector3.up.y * 2;
                _isDashing = true;
            }

            Destroy(_dashIndicatorClone);
            StartCoroutine(LerpDash(_destination, Reset));
            AkSoundEngine.PostEvent(_playTeleportSound, gameObject);
            DealDamage();
        }
        else
        {
            return;
        }
    }

    public void Reset()
    {
        _collider.enabled = true;
        _chromaticAberration.intensity.value = 0.0f;
    }

    private IEnumerator LerpDash(Vector3 destination, Action reset)
    {
        Vector3 startPos = transform.position;
        destination = _destination;
        _collider.enabled = false;                                      // disable the collider so the player doesn't take damage while they are dashing

        _postProcessProfile.TryGetSettings(out _chromaticAberration);   // set the Chromatic Aberration to make the player feeling like they are moving fast
        _chromaticAberration.intensity.value = _distortionAmount;

        // Lerp the players position from the start position to the desitination
        while (curLerptime < _dashTime)
        {
            curLerptime += Time.deltaTime;
            if (curLerptime > _dashTime)
            {
                curLerptime = 0.0f;
                reset();
                break;
            }

            float perc = curLerptime / _dashTime;
            // Vector3 dashPos = Vector3.Lerp(startPos, destination, perc);
            startPos = Vector3.Lerp(startPos, destination, perc);

            // transform.position = dashPos;
            transform.position = startPos;
            yield return null;
        }
        AkSoundEngine.PostEvent(_stopTeleportSound, gameObject);
        StopCoroutine(LerpDash(_destination, Reset));
    }

    private void DealDamage()
    {
        LayerMask playerMask = LayerMask.GetMask("Player");
        Collider[] targets = Physics.OverlapSphere(transform.position, _radius);
        foreach (Collider target in targets)
        {
            BaseAI enemy = target.GetComponent<BaseAI>();

            // if there is no enemy and/or if the overlapped collider is the player, skip this iteration of the loop
            if (enemy == null) { continue; }
            if (target.gameObject.layer == playerMask.value) { continue; }

            // get the rigidbodies from the target. if the target doesn't have a rigidbody, skip this iteration
            Rigidbody rb = target.GetComponent<Rigidbody>();
            if (rb == null) { continue; }

            // get the health of the enemies/bosses and deal damage to the
            HealthScript enemyhealth = target.GetComponent<HealthScript>();

            enemyhealth.Damage(_damage, AttackType.PHYSICAL, gameObject);

            // Finaly add explosion force to the enemies and watch them get blasted away
            rb.AddExplosionForce(_power, transform.position, _radius, 3.0f);
        }
    }

    [SerializeField,
    Tooltip("The layer the level is on")]
    private LayerMask levelLayer = 14;   // the layer the level is on
    private Ray _forwardRay;
    private Ray _downRay;
    private Vector3 downHitPos;
    private bool HitWall(out Vector3 hitPosition)
    {
        Vector3 playerPos = transform.position;
        playerPos.y += gameObject.transform.localScale.y / 2;
        _forwardRay = new Ray(playerPos, _rb.velocity);

        RaycastHit forwardHit;
        RaycastHit downHit;
        if (Physics.Raycast(_forwardRay, out forwardHit, _rayDistance, levelLayer))
        {
            hitPosition = forwardHit.point;

            float downRayPosY = hitPosition.y;
            downHitPos = new Vector3(forwardHit.point.x, downRayPosY, forwardHit.point.z);
            _downRay = new Ray(downHitPos, Vector3.down);
            if (Physics.Raycast(_downRay, out downHit, _rayDistance, levelLayer))
            {
                hitPosition = downHit.point;     // TODO: get rid of -> (Vector3.up * 2) <- if dash doesn't work
            }

            hitPosition += Vector3.up;
            hitPosition += forwardHit.normal * 2;

            return true;
        }
        else
        {
            hitPosition = Vector3.zero;
            return false;
        }

    }

    private void OnCollisionEnter(Collision other) 
    {
        if( _isDashing || other.gameObject.GetComponent<Terrain>() != null)
        {
            StopAllCoroutines();
        }
    }
}
