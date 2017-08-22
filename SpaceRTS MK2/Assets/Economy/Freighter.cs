using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Freighter : MonoBehaviour {
	GameObject controller;

	NavMeshAgent agent;
	ResourceRequest mission;

	City target;


	// Use this for initialization
	void Start () {
		gameObject.name = NameGen.GenerateName ("L4N2L1");
		Collections.Freighters.Add (this);
		Collections.Available.Add (this);
		agent = GetComponent<NavMeshAgent> ();
		controller = GameObject.FindGameObjectWithTag ("Controller");

	}


	public void AssignMission(ResourceRequest r){
		Collections.Available.Remove (this);
		mission = r;
		List<City> Source = new List<City> ();
		Source.AddRange (Collections.Cities);
		Source.OrderBy(
			targ => Vector3.Distance(this.transform.position,targ.transform.position)).ToList();
		target = null;
		int i = 0;
		while (target == null) {
			if ( Source[i] != mission.patron && Source [i].ResourceStockpile [mission.resource].GetAmount () >= mission.amount) {
				target = Source [i];
			}
			i++;
		}
		agent.SetDestination (target.transform.position);
		Debug.Log (gameObject.name + " is enroute to " + target.name);
	}



	// Update is called once per frame
	void Update () {
		
	}
}
