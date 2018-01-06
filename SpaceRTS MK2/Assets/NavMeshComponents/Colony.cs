using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Colony :ILocation {

	static float growth = .025f; //per minute

	public string ColonyName;

	public List<FacilityType> Facilities;

	public Empire empire;
	public Planet planet;
	public PlanetRegion region;

	Dictionary<Race, int> Population = new Dictionary<Race, int> ();
	public List<Character> Characters = new List<Character>();
	public Character Governor;


	public float Morale = 0f;
	public float TaxRate = .08f;

	public float Wealth;

	public string Description ="";

	public int GetPopulationTotal(){
		int i = 0;
		foreach (int n in Population.Values) {
			i += n;
		}
		return i;
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

	public string GetLocationName(){
		return ColonyName;
	}
	public object GetLocation(){
		return (object)this;
	}
	public System.Type GetLocType(){
		return this.GetType ();
	}
	public void MoveCharacterToThis(Character c){
		UpdateSeniorOfficer ();
	}
	public void MoveCharacterFromThis(Character c){
		UpdateSeniorOfficer ();
	}

	public string GetSearchableString()
	{
		return planet.PlanetName + ColonyName;
	}

	public Vector3 GetPosition()
	{
		return planet.transform.position;
	}

	//Called whenever someone transfers in or out.
	void UpdateSeniorOfficer(){
		List<Character> Here = empire.GetCharactersAtLocation (this,OfficerRoles.Government);
		if (Here[0] != null && Here [0] != Governor) {
			if(Governor != null)
				Governor.StepDownGovernor (this);
			Governor = empire.GetCharactersAtLocation (this) [0];
			Governor.StepDownGovernor (this);
		}
		else if (Here[0] == null)
			Governor = null;
	}

	public List<Race> GetRaces(){
		List<Race> Present = new List<Race> ();
		Race.AllRaces.ForEach (x => {
			if(Population.ContainsKey(x))
				Present.Add(x);
		});
		return Present;
	}

	public void ChangeOwner(Empire newOwner){
		empire.RemoveColony (this);
		newOwner.AddColony (this);
		EmpireLogEntry log = new EmpireLogEntry (LogCategories.MILITARY, 1, empire, "COLONY LOST", string.Format ("{0} on {1} has fallen to the forces of {2}.", ColonyName, planet.PlanetName,newOwner.EmpireName));
		EmpireLogEntry log2 = new EmpireLogEntry(LogCategories.MILITARY,1,newOwner,"COLONY CONQUERED",string.Format ("{0} on {1} has fallen to our forces.", ColonyName,planet.PlanetName));
		empire = newOwner;
	}

	public Colony(Empire emp, Race r, int pop, string nam = "Terra"){
		empire = emp;
		empire.AddColony (this);
		Population.Add (r, pop);
		ColonyName = nam;
		Empire.AllLocations.Add (this);
		StrategicClock.PhaseChange.AddListener (PhaseManager);
		EmpireLogEntry log = new EmpireLogEntry (LogCategories.DEFAULT, 3, empire, "COLONY ESTABLISHED", string.Format ("{0} has been established on {1}.", ColonyName, planet.PlanetName));
	}

}

public class Pop
{
	public Race race;
	public int Happiness; //0 - 100
	

}