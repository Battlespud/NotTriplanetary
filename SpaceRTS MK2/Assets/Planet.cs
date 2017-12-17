using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

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

public enum RegionTypes{
	Plain,
	Coast,
	Mountain,
	Hill,
	City,
	Sea
}

public interface IRecieveCombatResult{
	void ChangeOwner(Empire Victor);
}



public class Planet : MonoBehaviour, IMineable
{
	public static Dictionary<PlanetRegion,Planet> RegionToPlanet = new Dictionary<PlanetRegion, Planet>();
	public static Dictionary<Planet,List<PlanetRegion>> PlanetToRegions = new Dictionary<Planet, List<PlanetRegion>>();
	public static List<Planet> AllPlanets = new List<Planet>();

    public Dictionary<RawResources, ResourceDeposit> ResourceDeposits = new Dictionary<RawResources, ResourceDeposit>();
    public int[] composition;

	public List<PlanetRegion> Regions = new List<PlanetRegion>();



	public List<Fleet> OrbitingFleets = new List<Fleet>();
	void OnTriggerEnter(Collider col){
		Debug.LogError ("Fleet in orbit");
		if (col.GetComponent<Fleet> ()) {
			OrbitingFleets.Add (col.GetComponent<Fleet> ());
			col.GetComponent<Fleet> ().NearbyPlanets.AddExclusive (this);
		}

	}
	void OnTriggerExit(Collider col){
		if (col.GetComponent<Fleet> ()) {
			OrbitingFleets.Remove (col.GetComponent<Fleet> ());
			col.GetComponent<Fleet> ().NearbyPlanets.Remove (this);
		}

	}

	public List<PlanetRegion> GetNeighbouringRegions(PlanetRegion start){
		List<PlanetRegion> valid = new List<PlanetRegion> ();
		int i = Regions.IndexOf (start);
		if (!(i - 1 < 0)) 
			valid.Add(Regions[i-1]);
		if (i + 1 < Regions.Count)
			valid.Add (Regions[i+1]);

		return valid;
	}


	public List<Colony> GetColonyList(){
		List<Colony> colonies = new List<Colony> ();
		Regions.ForEach (x => {
			if(x.RegionColony != null)
				colonies.Add(x.RegionColony);
		});
		return colonies;
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

	public string PlanetName = "Unnamed Planet";
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
		GenerateRegions (Random.Range(2,6));
		StrategicClock.PhaseChange.AddListener (PhaseManager);
		AllPlanets.Add (this);
		PlanetToRegions.Add (this, Regions);

    }




    // Update is called once per frame
    void Update()
    {
    }

	private void GenerateRegions(int number){
		for (int i = 0; i < number; i++) {
			PlanetRegion r = new PlanetRegion ();
			Regions.Add (r);
			RegionToPlanet.Add (r, this);
		}
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