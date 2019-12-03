/*
* Copyright (C) Noah Greaves in Association with VFS
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// USE FOR DIFFERENT GROUND SURFACES
public enum GroundSwitchValue
{
    DIRT, 
    GRASS,
    GRAVEL,
    LEAVES,
    MUD, 
    STONE,
    WATER 
}

public class GroundType : MonoBehaviour
{
    // [SerializeField] private GroundSwitchValue groundSwitchValue;
    public GroundSwitchValue groundSwitchValue;
    
    public GroundSwitchValue GetGroundSwitchValue()
    {
        // return groundSwitchValue.ToString().ToLower();
        return groundSwitchValue;
    }
}
