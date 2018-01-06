using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using NUnit.Framework;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

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

public interface IReceiveCombatResult{
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

	public PlanetHistory planetHistory;

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
			PlanetRegion r = new PlanetRegion (this);
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

public class PlanetHistory
{
	
	
	
}

public class Species
{
	
	System.Random rng = new System.Random();
	
	public string SpeciesName;
	public int SpeciesNumbers;

	public int GeneCount = 48;
	public List<Gene> Baseline = new List<Gene>();
	
	public List<KeyValuePair<PlanetRegion,int>> Regions = new List<KeyValuePair<PlanetRegion,int>>();
	public List<RegionTypes> PreferredRegions = new List<RegionTypes>();

	public List<Individual> Members = new List<Individual>();
	
	
	
	public int MutationPropensity = 48;
	
	public float AvgMass;

	public Species(string n)
	{
		SpeciesName = n;



		for (int i = 0; i < GeneCount; i++)
		{
			Sex s = Sex.Either;
			s.RandomEitherSex();
			Baseline.Add(new Gene((byte) rng.Next(0, 256), s));
		}
		Debug.LogError(Baseline.Count);

		for (int i = 0; i < 40; i++)
		{
			Individual x = new Individual(this);
		}

		for (int generation = 0; generation < 15; generation++)
		{
			int count = Members.Count;
			int sanityLimit = 1000;
			int sanity = 0;
			for (int i = 0; i < count / 2; i++)
			{
				Individual a = Members.GetRandom();
				Debug.LogWarning(a.Generation + " A Generation");
				Individual b = Members.GetRandom();
				while ((b.sex == a.sex || a == b) && sanity < sanityLimit)
				{
					b = Members.GetRandom();
					sanity++;
				}
				if(sanity > sanityLimit)
					Debug.LogError("Sanity limit reached");
				if (a.sex == Sex.Male)
					Individual.Reproduce(a, b);
				else
				{
					Individual.Reproduce(b, a);
				}
			}

		}
	}

	static Species()
	{
	}

}


public class Gene
{
	public byte val;
	public Sex carrier;

	public void SetVal(byte b)
	{
		val = b;
	}
	
	public Gene(byte b, Sex s = Sex.Either)
	{
		val = b;
		carrier = s;
	}

	
	public Gene Clone()
	{
		return new Gene(val, carrier);
	}

	public void Clone(Gene source)
	{
		val = source.val;
		carrier = source.carrier;
	}
}

public class Individual
{
	static System.Random rng = new System.Random();

	static float CalculateDivergence(Individual i)
	{
		int index = 0;
		int diverged = 0;
		foreach (var g in i.Genes)
		{
			if (g.val != i.species.Baseline[index].val)
				diverged++;
			index++;
		}
		return diverged / i.species.Baseline.Count;
	}
	public Sex sex;
	public Species species;
	public float Divergence;
	public int Generation = 0;
	public List<Gene> Genes = new List<Gene>();

	public static void Reproduce(Individual m, Individual f)
	{
		Individual c = new Individual(m, f, f.species);
		Individual d = new Individual(m, f, f.species);

	}

	public Individual(Species s)
	{
		species = s;
		s.Members.Add(this);
		Genes.AddRange(s.Baseline);
		Divergence = 0;
		sex.RandomSex();
		Generation = 0;
	}
	
	public Individual(Individual M, Individual F, Species s)
	{
		if (M.Generation > F.Generation)
			Generation = M.Generation + 1;
		else
			Generation = F.Generation + 1;
		s.Baseline.ForEach(g =>
		{
			Genes.Add(g.Clone());
		});
		int index = 0;
		foreach (var x in Genes)
		{
			switch (x.carrier)
			{
				case Sex.Female:
					x.Clone(F.Genes[index]);
					break;
				case Sex.Male:
					x.Clone(M.Genes[index]);
					break;
				case Sex.Either:
					if (rng.NextBool())
						x.Clone(F.Genes[index]);
					else
						x.Clone(M.Genes[index]);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			if (rng.NextFloat(0, s.MutationPropensity) < 1)
			{
				byte n = (byte) rng.Next(0, 256);
			//	Debug.LogWarning( x.val + " is now " + n);
				x.SetVal(n);
			}
			index++;

		}

		species = s;		
		s.Members.Remove(M);
		s.Members.Remove(F);
		s.Members.Add(this);
		
		Divergence = CalculateDivergence(this);
		sex.RandomSex();
		string GenesString ="";
		foreach (var g in Genes)
		{
			GenesString += " " + g.val;
		}
		Debug.LogWarning("Gen: " + Generation + " | Div: " + Divergence + "% Genes: " + GenesString);
	}

}