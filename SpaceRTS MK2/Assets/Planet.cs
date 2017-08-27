﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Events;

//valid planet compositions
/* 
 * 01, 02, 03, 04
 * 11, 12, 13, 14
 * 21, 23
 * 31, 33, 34
 * 42, 43
 * 50, 52, 53
 * 60, 63, 64
 */

public enum PlanetType
{
    Gas, //0 OXYGEN, CARBONDIOXIDE, METHANE, NITROGEN
    Terran, //1 OXYGEN, CARBONDIOXIDE, METHANE, NITROGEN
    Ocean, //2 OXYGEN, METHANE
    Jungle, //3 OXYGEN, METHANE, NITROGEN
    Lava, //4 CARBONDIOXIDE, METHANE
    Barren, //5 VACUUM, CARBONDIOXIDE, METHANE
    Silicon //6 VACUUM, METHANE, NITROGEN
}

public enum AtmosphereType
{
    Vacuum, //0
    Oxygen, //1
    CO2, //2
    Methane, //3
    Nitrogen //4
}

public class ResourceDeposit
{
    public float access;
    public float prevalence;
    public float amount;
    public RawResources resource;

    public ResourceDeposit(RawResources res, float acc, float pre, float am)
    {
        resource = res;
        access = acc;
        prevalence = pre;
        amount = am;
    }
    public ResourceDeposit(RawResources res)
    {
        resource = res;
        access = UnityEngine.Random.Range(0.0f, 1f);
        prevalence = UnityEngine.Random.Range(0.0f, 1f);
        if (access > .4f && prevalence > .4f)
        {
            amount = UnityEngine.Random.Range(1000f, 5000f);
        }
        else
        {
            amount = UnityEngine.Random.Range(200f, 3000f);
        }
    }
}

public class Planet : MonoBehaviour, IMineable
{
    public Dictionary<RawResources, ResourceDeposit> ResourceDeposits = new Dictionary<RawResources, ResourceDeposit>();
    public StringBuilder planetComposition = new StringBuilder();

    void AddDeposit(ResourceDeposit r)
    {
        ResourceDeposits.Add(r.resource, r);
    }

    void AddDeposit(RawResources r)
    {
        ResourceDeposits.Add(r, new ResourceDeposit(r));
    }

    public float Mine(RawResources r, float amount)
    {
        if (ResourceDeposits[r] == null)
            return 0f;
        amount = amount * ResourceDeposits[r].access;
        if (amount <= ResourceDeposits[r].amount)
        {
            ResourceDeposits[r].amount -= amount;
            return amount;
        }
        return 0f;
    }

    // Use this for initialization
    void Awake()
    {
        for (int i = 1; i < 6; i++)
        {
            AddDeposit((RawResources)i);
        }
        Collections.Mineable.Add(this);
    }

    void Start()
    {
        SetPlanetType(UnityEngine.Random.Range(0, 7));
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SetPlanetType(int planetType)
    {
        int[] comp = CheckTypeCompatibility((PlanetType)planetType);
        string pt = Enum.GetName(typeof(PlanetType), comp[0]);
        string at = Enum.GetName(typeof(AtmosphereType), comp[1]);
        planetComposition.AppendFormat("{0} {1}", pt, at);
        Debug.Log(string.Format("Planet type set to {0}", planetComposition.ToString()));
    }

    //valid planet compositions
    /* 
     * 01, 02, 03, 04 - 1234
     * 11, 12, 13, 14 - 1234
     * 21, 23 - 13
     * 31, 33, 34 - 134
     * 42, 43  - 23
     * 50, 52, 53 - 023
     * 60, 63, 64 - 034
     */
    private int[] CheckTypeCompatibility(PlanetType type)
    {
        int[] composition = new int[2];
        int pt = -1;
        int at = -1;
        int swap = -1;

        switch (type)
        {
            case PlanetType.Gas:
            case PlanetType.Terran:
                pt = (int)type;
                at = UnityEngine.Random.Range(1, 4);
                break;
            case PlanetType.Ocean:
                pt = (int)type;
                at = UnityEngine.Random.Range(0f, 1f) >= .5f ? 1 : 3;
                break;
            case PlanetType.Jungle:
                pt = (int)type;
                swap = UnityEngine.Random.Range(2, 5);
                if (swap == 2)
                    at = 1;
                else
                    at = swap;
                break;
            case PlanetType.Lava:
                pt = (int)type;
                at = UnityEngine.Random.Range(0f, 1f) >= .5f ? 2 : 3;
                break;
            case PlanetType.Barren:
                pt = (int)type;
                swap = UnityEngine.Random.Range(1, 4);
                if (swap == 1)
                    at = 0;
                else
                    at = swap;
                break;
            case PlanetType.Silicon:
                pt = (int)type;
                swap = UnityEngine.Random.Range(2, 5);
                if (swap == 2)
                    at = 0;
                else
                    at = swap;
                break;
            default:
                break;
        }

        composition[0] = pt;
        composition[1] = at;
        return composition;
    }
}