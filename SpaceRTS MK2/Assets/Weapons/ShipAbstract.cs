using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAbstract : MonoBehaviour, ICAPTarget {

	public static ShipEvent OnDeath = new ShipEvent();
	public ShipClass shipClass;


	public FAC faction;



	public void DealDamage(float dam, Vector3 origin, Transform en){
		shipClass.Damage (dam, origin, en);
			}

	public void DealPhysicsDamage(float dam, Vector3 origin, float fMag, Transform s){
		shipClass.PhysicsDamage (dam,origin,fMag, s);
	}

	public bool isHostile(FAC caller){
		return FactionMatrix.IsHostile (caller, faction);
	}

	public FAC GetFaction(){
		return faction;
	}

	public GameObject GetGameObject(){
		return gameObject;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
