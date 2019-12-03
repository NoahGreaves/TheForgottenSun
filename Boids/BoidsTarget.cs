/*
* Copyright (C) Noah Greaves with assistance from Quin Henshaw in Association with VFS
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidsTarget : MonoBehaviour
{
    [SerializeField] private float _radius = 30f;
    [SerializeField] private float _searchRadius = 5.0f;
    [SerializeField] private float _rotationSpeed = 30f;
    [SerializeField] private float _perlinScale = 30f;
    [SerializeField] private float _offsetHeight = 30f;
}
