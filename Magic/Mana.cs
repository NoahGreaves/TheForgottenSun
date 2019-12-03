/*
* Copyright (C) Noah Greaves in Association with VFS
*/

using System;
using UnityEngine;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
    [SerializeField, Tooltip("The amount of mana the player gets after the cooldown")]
    private float _regenAmount;
    
    [Tooltip("The max amount of mana the player will have")]
    public float _maxManaAmount;

    [HideInInspector]
    public float _manaAmount;
    public float manaPercentage => (float)_manaAmount / _maxManaAmount;
    public event Action<float> ManaEvent = delegate {};

    private float _maxManaCooldown;

    private void Awake()
    {
        _manaAmount = _maxManaAmount;
    }

    private void Update()
    {
        if(_manaAmount >= _maxManaAmount || _manaAmount == _maxManaAmount)
        {
            _manaAmount = _maxManaAmount;
        }
        if(_manaAmount <= 0)
        {
            _manaAmount = 0;        // if the amount of mana the player has drops below 0 set the mana value back to 0
        }
    }

    public void InvokeEventMana()
    {
        ManaEvent?.Invoke(manaPercentage);
    }
}
