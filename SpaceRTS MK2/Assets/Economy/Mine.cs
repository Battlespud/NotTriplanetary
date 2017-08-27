using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour, IResources
{

    public RawResources resource;
    public IMineable target;
    float rate = 5f;

    public float Stockpile;



    // Use this for initialization
    void Start()
    {
        GameObject.FindGameObjectWithTag("Controller").GetComponent<Clock>().TurnEvent.AddListener(DoMine);
        target = GetComponent<IMineable>();
        Collections.Mines.Add(this);
        Collections.ResourceSources.Add(this);
        if (GetComponent<Asteroid>())
        {
            resource = GetComponent<Asteroid>().deposit.resource;
        }
    }

    void DoMine()
    {
        Stockpile += target.Mine(resource, rate);
    }

    public bool HasResource(RawResources r, float amount)
    {
        if (r == resource)
        {
            return Stockpile >= amount;
        }
        return false;
    }

    public float ResourceAmount(RawResources r)
    {
        if (r == resource)
        {
            return Stockpile;
        }
        return 0f;
    }

    public bool GiveResource(RawResources r, float amount)
    {
        return false;
    }

    public bool TakeResource(RawResources r, float amount)
    {
        if (r == resource)
        {
            if (Stockpile >= amount)
            {
                Stockpile -= amount;
                return true;
            }
        }
        return false;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    // Update is called once per frame
    void Update()
    {

    }
}