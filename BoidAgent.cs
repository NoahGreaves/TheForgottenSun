/*
* Copyright (C) Noah Greaves with assistance from Quin Henshaw in Association with VFS
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// goes on bees
public class BoidAgent : MonoBehaviour
{
    public float NoiseOffset { get; private set; }

    [HideInInspector]
    public HealthScript EnemyHealth;

    private void Awake()
    {
        NoiseOffset = Random.value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(EnemyHealth != null)
        {
            StartCoroutine(EnemyHealth.Stun());
        }
    }
}
