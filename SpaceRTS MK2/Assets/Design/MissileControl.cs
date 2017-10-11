using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Threading;

public class MissileControl : MonoBehaviour, IPDTarget {

	public Engine MissileEngine;
	public Warhead MissileWarhead;
	public FuelTank MissileFuelTank;

	NavMeshAgent Agent;

	GameObject Target;

	FAC Faction;

	// Use this for initialization
	void Awake () {
		Agent = GetComponent<NavMeshAgent> ();
	}

	Vector3 CalculateClosest(Collider col, Transform t){
	return	col.ClosestPointOnBounds (t.position);
	}

	public void Launch(GameObject Source, ICAPTarget target)
	{
		Target = target.GetGameObject();
		Agent.Warp (CalculateClosest(Source.GetComponent<Collider>(), target.GetGameObject().transform));
		transform.LookAt (Target.transform.position);
		Agent.SetDestination (Target.transform.position);
		Agent.angularSpeed = .01f;
		Agent.speed = 14f;
	}

	void OnCollisionEnter(Collision col){
		if (!MissileWarhead.armed)
			return;
		if (col.collider.GetComponent<ICAPTarget> () != null)
			Detonate (col.collider.GetComponent<ICAPTarget> ());
	}

	public void Detonate(ICAPTarget targ){
		targ.DealDamage (12f-(2f*Vector3.Distance(targ.GetGameObject().transform.position,transform.position)), transform.position,transform, MissileWarhead.Pattern);
		MissileWarhead.armed = false;
		StartCoroutine("Destroy");
	}

	public void HitByPD(int dam){
		StartCoroutine (Destroy());
	}

	public FAC GetFaction(){
		return Faction;
	}

	public GameObject GetGameObject(){
		return gameObject;
	}

	public bool isHostile(FAC caller){
		return FactionMatrix.IsHostile (caller, Faction);
	}

	IEnumerator Destroy(){
		PDTargetAbstract.pdDeath.Invoke (this);
		Agent.Warp(new Vector3(0f,-1000f,0f));
		float f = 1f;
		while (f > 0f) {
			f -= Time.deltaTime;
			yield return null;
		}
		Destroy (gameObject);
	}

	bool fueled = true;
	// Update is called once per frame
	void Update () {
		if (fueled) {
			Agent.SetDestination (Target.transform.position);
			if (!MissileFuelTank.UseFuel (MissileEngine.FuelConsumption * Time.deltaTime)) {
				StartCoroutine("Destroy");
				fueled = false;
			}
		}

	}
}
