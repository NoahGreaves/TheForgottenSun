/*
* Copyright (C) Noah Greaves with assistance from Quin Henshaw in Association with VFS
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;

// goes on the transform that the bees should navigate around
public class JobsBoidController : BoidController
{
    private struct BoidData
    {
        public float3 Position;
        public float3 Forward;
        public float NoiseOffset;
    }

    // these collection can be passed into our job
    private NativeArray<BoidData> _boids;
    private TransformAccessArray _boidTransforms;

    private void Start()
    {
        StartCoroutine(DestroyTimer());
    }

    private void OnEnable()
    {
        _boids = new NativeArray<BoidData>(_boidCount, Allocator.Persistent);
        _boidTransforms = new TransformAccessArray(_boidCount);

        SpawnBoids();

        for (int i = 0; i < _boidCount; i++)
        {
            _boidTransforms.Add(Boids[i].transform);
        }
    }

    private void Update()
    {
        if (distroyedAll == false)
        {
            // update boid data
            for (int i = 0; i < _boidCount; i++)
            {
                if (Boids.Count <= 0) { continue; }

                _boids[i] = new BoidData
                {
                    Position = Boids[i].transform.position,
                    Forward = Boids[i].transform.forward,
                    NoiseOffset = Boids[i].NoiseOffset
                };
            }
        }

        // create boid job
        UpdateBoidJob boidJob = new UpdateBoidJob
        {
            Boids = _boids,
            BoidTuning = _boidTuning,
            TargetPosition = _boidTarget.position,
            Time = Time.timeSinceLevelLoad,
            DeltaTime = Time.deltaTime
        };

        boidJob.Schedule(_boidTransforms).Complete();
    }

    // sets the position of the boid
    public void SetPostion(Transform position)
    {
        _boidTarget = position;
    }

    // clears the memory of the boids, the boid list, and the boid data native array
    private void OnDisable()
    {
        Boids.Clear();
        _boidTransforms.Dispose();
        _boids.Dispose();
    }

    // the function that destroys the boids and the controller after the lifetime
    private IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(_lifetime);
        EndJob();
        Destroy(gameObject);
        StopCoroutine(DestroyTimer());
    }

    // destroys the boids in the list
    bool distroyedAll = false;
    public void EndJob()
    {
        distroyedAll = true;
        int boidsLength = Boids.Count;
        for (int i = 0; i < Boids.Count; i++)
        {
            if (i > Boids.Count) { return; }
            Destroy(Boids[i].gameObject);
        }
    }

    [BurstCompile]
    private struct UpdateBoidJob : IJobParallelForTransform
    {
        [ReadOnly] public NativeArray<BoidData> Boids;
        [ReadOnly] public BoidTuning BoidTuning;
        [ReadOnly] public float3 TargetPosition;
        [ReadOnly] public float DeltaTime;
        [ReadOnly] public float Time;

        public void Execute(int index, TransformAccess transform)
        {
            float3 currentPosition = transform.position;

            // boid vectors
            float3 seperation = Vector3.zero;
            float3 alignment = Vector3.zero;
            float3 cohesion = TargetPosition;

            // slightly optimized distance (by avoiding square root)
            float neighborDistanceSquared = BoidTuning.NeighborDistance * BoidTuning.NeighborDistance;

            // accumulate vectors
            int neighborCount = 1;
            for (int i = 0; i < Boids.Length; i++)
            {
                float3 neighborPosition = Boids[i].Position;
                float distance = math.lengthsq(currentPosition - neighborPosition);

                // skip accumulation if we're comparing to ourselces, or agent is too far away
                if (distance < 0.01f || distance > neighborDistanceSquared) { continue; }

                // determining the position
                float3 vectorDiff = currentPosition - neighborPosition;
                float inverseDistance = math.clamp(1f - distance / BoidTuning.NeighborDistance, 0f, 1f);
                float3 seperationVector = vectorDiff * (inverseDistance / distance);

                // add vectors
                seperation += seperationVector;

                alignment += Boids[i].Forward;
                cohesion += neighborPosition;

                neighborCount++;
            }

            // average out vectors
      
               float averageMultiplier = 1f / neighborCount;


            alignment *= averageMultiplier;
            cohesion *= averageMultiplier;
            float3 cohesionDirection = math.normalize(cohesion - currentPosition);

            // find target rotation
            float3 aimDir = math.normalize(seperation + alignment + cohesionDirection);
            quaternion targetRotation = quaternion.LookRotation(aimDir, new float3(0f, 1f, 0f));
            quaternion finalRotation = math.slerp(transform.rotation, targetRotation, DeltaTime * BoidTuning.RotationLerp);

            // find target position
            float noise = Unity.Mathematics.noise.cnoise(new float2(Time, Boids[index].NoiseOffset));
            float velocity = BoidTuning.Velocity * (1f + noise * BoidTuning.VelocityVariation);
            float3 finalPosition = currentPosition + math.forward(finalRotation) * velocity * DeltaTime;

            // set positon and rotation
            transform.position = finalPosition;
            transform.rotation = finalRotation;
        }
    }
}