using System.Collections.Generic;
using UnityEngine;
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
    public int[] composition;

	Dictionary<Empire,Colony> Colonies = new Dictionary<Empire, Colony> ();

	public Colony GetColony(Empire e){
		Colony c = null;
			Colonies.TryGetValue (e, out c);
		return c;
	}

	public bool AddColony(Empire owner, Race r, int startingPop, string name){
		if (Colonies.ContainsKey (owner))
			return false;
		Colony c = new Colony (owner, r, startingPop, name);
		Colonies.Add (owner, c);
		return true;
	}

	void PhaseManager(Phase p){
		switch (p) {
		case(Phase.ORDERS):
			{
				break;
			}
		case(Phase.GO):
			{
				break;
			}
		case (Phase.REVIEW):
			{

				break;
			}
		case (Phase.INTERRUPT):
			{
				break;
			}
		}	
	}

	public string Description ="";



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
		StrategicClock.PhaseChange.AddListener (PhaseManager);

    }



    // Update is called once per frame
    void Update()
    {
    }

    private void SetPlanetType(int planetType)
    {
        composition = CheckAndGenerateComposition((PlanetType)planetType);
        Debug.Log(string.Format("Planet type set to {0} {1}", ((PlanetType)composition[0]).ToString(), ((AtmosphereType)composition[1]).ToString()));
    }
    private static int[] CheckAndGenerateComposition(PlanetType type)
    {
        int pt = -1;
        int at = -1;
        int swap = -1;

        switch (type)
        {
            case PlanetType.Gas:
            case PlanetType.Terran:
                pt = (int)type;
                at = Random.Range(1, 4);
                break;
            case PlanetType.Ocean:
                pt = (int)type;
                at = Random.Range(0f, 1f) >= .5f ? 1 : 3;
                break;
            case PlanetType.Jungle:
                pt = (int)type;
                swap = Random.Range(2, 5);
                if (swap == 2)
                    at = 1;
                else
                    at = swap;
                break;
            case PlanetType.Lava:
                pt = (int)type;
                at = Random.Range(0, 2) == 1 ? 2 : 3;
                break;
            case PlanetType.Barren:
                pt = (int)type;
                swap = Random.Range(1, 4);
                if (swap == 1)
                    at = 0;
                else
                    at = swap;
                break;
            case PlanetType.Silicon:
                pt = (int)type;
                swap = Random.Range(2, 5);
                if (swap == 2)
                    at = 0;
                else
                    at = swap;
                break;
            default:
                break;
        }

        return new int[2] { pt, at };
    }
}