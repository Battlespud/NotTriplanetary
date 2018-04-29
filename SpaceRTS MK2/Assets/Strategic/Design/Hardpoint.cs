using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HardpointTypes
{
    Engine,
    Weapon,
    Sensor
}

public struct Hardpoint
{
    public int Size;
    public HardpointTypes HardpointType;
    public bool inUse;

    public Hardpoint(int s, HardpointTypes type)
    {
        HardpointType = type;
        Size = s;
        inUse = false;
    }

    public Hardpoint Clone()
    {
        return new Hardpoint(this.Size, this.HardpointType);
    }
    
}

