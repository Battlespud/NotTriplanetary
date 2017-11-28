using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colony {

	static float growth = .025f; //per minute

	public Empire empire;

	Dictionary<Race, int> Population = new Dictionary<Race, int> ();

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
