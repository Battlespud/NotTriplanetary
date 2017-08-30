using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class Freighter : ShipAbstract{
	GameObject controller;

	NavMeshAgent agent;
	ResourceRequest mission;

	public List<IResources> Orbiting = new List<IResources> ();

	IResources target;

	bool loading;

	RawResources raw;
	public float maxLoad = 200f;
	public float load  = 0f;


	// Use this for initialization
	public override void Start () {
		try{
		gameObject.name = NameGen.GenerateName ("L4N2");
		}
		catch{
			Debug.Log ("NameGen failed");
			gameObject.name = "NAMEGEN_ERROR";
		}
		shipClass = GetComponent<ShipClass> ();
		Collections.Freighters.Add (this);
		Collections.Available.Add (this);
		agent = GetComponent<NavMeshAgent> ();
		controller = GameObject.FindGameObjectWithTag ("Controller");
	}


	public void AssignMission(ResourceRequest r){
		target = null;
		loading = true;
		Collections.Available.Remove (this);
		mission = r;
		List<IResources> Source = new List<IResources> ();
		Source.AddRange (Collections.ResourceSources);
		Source.OrderBy(
			targ => Vector3.Distance(this.transform.position,targ.GetGameObject().transform.position)).ToList();
		target = null;
		int i = 0;
		try{
		while (target == null) {
			if ( Source[i] != mission.patron && Source [i].HasResource(mission.resource,mission.amount)) {
				target = Source [i];
			}
			i++;
		}
		}
		catch{
			Debug.Log ("No valid sources.");
			EndMission ();
			return;
		}
		agent.SetDestination (target.GetGameObject().transform.position);
		Debug.Log (gameObject.name + " is enroute to " + target.GetGameObject().name);
	}

	void EndMission(){
		Collections.Available.Add (this);
		Debug.Log ("Mission complete!");
		mission = null;
		target = null;
	}
	void OnTriggerEnter(Collider col){
		if (col.GetComponent<IResources>() != null ) {
			Orbiting.Add (col.GetComponent<IResources> ());
		}
	}
	void OnTriggerExit(Collider col){
		if (col.GetComponent<IResources> () != null) {
			Orbiting.Remove (col.GetComponent<IResources> ());
		}
	}


	// Update is called once per frame
	void Update () {

		if (target != null && Orbiting.Contains (target) && loading) {
			Load ();

		}
		else if (target != null && Orbiting.Contains (target) && !loading) {
			Unload ();
		}
	}

	void Unload(){
		target.GiveResource (raw, load);
		load = 0f;
		EndMission ();
	}

	void Load(){
		if (mission.exact) {
			if (target.TakeResource (mission.resource, mission.amount)) {
				load = mission.amount;
				target = mission.patron;
				raw = mission.resource;
				loading = false;
			}
		} else {
			if (target.HasResource (mission.resource, maxLoad)) {
				if (target.TakeResource (mission.resource, maxLoad))
					load = maxLoad;
			} else {
				float f = target.ResourceAmount (mission.resource);
				if (target.TakeResource (mission.resource, target.ResourceAmount (mission.resource)))
					load = f;
			}
				target = mission.patron;
				raw = mission.resource;
				loading = false;			
		}
		MoveTo(target.GetGameObject().transform.position);

	}

	void MoveTo(Vector3 pos){
		agent.destination = pos;
		Debug.Log (gameObject.name + " is enroute to " + target.GetGameObject().name);
	}

}
