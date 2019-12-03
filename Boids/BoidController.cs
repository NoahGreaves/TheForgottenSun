/*
* Copyright (C) Noah Greaves with assistance from Quin Henshaw in Association with VFS
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoidController : MonoBehaviour
{
    [Serializable]
    public struct BoidTuning
    {
        public float Velocity;
        public float VelocityVariation;
        public float RotationLerp;
        public float NeighborDistance;
        // public float SpawnRadius;
    }

    // [SerializeField] protected BoidAgent _boidPrefab;
    [SerializeField] public BoidAgent _boidPrefab;
    [SerializeField] protected int _boidCount;
    [SerializeField] protected float _spawnRadius = 20f;
    [SerializeField] protected BoidTuning _boidTuning;
    [SerializeField] protected Transform _boidTarget;
    [SerializeField] protected float _lifetime = 3;

    [SerializeField]
    protected List<BoidAgent> Boids = new List<BoidAgent>();

    [HideInInspector]
    public BoidAgent boid;

    protected void SpawnBoids()
    {
        for(int i = 0; i < _boidCount; i++)
        {
            Vector3 spawnPosition = transform.position + Random.onUnitSphere * _spawnRadius;
            // Debug.Log($"spawn position: {spawnPosition}");
            // Vector3 spawnPosition = spawnPos;
            boid = Instantiate(_boidPrefab, spawnPosition, Quaternion.identity);
            Boids.Add(boid);
        }
    }

    private void OnDrawGizmos()
    {
        // gizmo for the boss search radius
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _spawnRadius);
    }
}
