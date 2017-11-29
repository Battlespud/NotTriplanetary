using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colony :ILocation {

	static float growth = .025f; //per minute

	public Empire empire;

	Dictionary<Race, int> Population = new Dictionary<Race, int> ();
	public List<Character> Characters = new List<Character>();
	public Character Governor;


	public Planet planet;
	public string ColonyName;

	public float morale = 0f;
	public float taxRate = .08f;

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
		empire = newOwner;
	}

	public Colony(Empire emp, Race r, int pop, string nam = "Pinoy Land"){
		empire = emp;
		empire.AddColony (this);
		Population.Add (r, pop);
		ColonyName = nam;
		StrategicClock.PhaseChange.AddListener (PhaseManager);
	}

}
