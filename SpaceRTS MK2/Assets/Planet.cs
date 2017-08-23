using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlanetType{
	Gas,
	Terran,
	Ocean,
	Jungle,
	Lava
};
public enum AtmosphereType{
	V, //vacuum
	O, //o2
	C, //co2
	M, //methane
	N //Nitrogen

};


public class ResourceDeposit{



	public float access;
	public float prevalence;
	public float amount;
	public RawResources resource;

	public ResourceDeposit(RawResources res, float acc, float pre, float am){
		resource = res;
		access = acc;
		prevalence = pre;
		amount = am;
	}
	public ResourceDeposit(RawResources res){
		resource = res;
		access = Random.Range (0.0f, 1f);
		prevalence = Random.Range (0.0f, 1f);
		if (access > .4f && prevalence > .4f) {
			amount = Random.Range (1000f, 5000f);
		} else {
			amount = Random.Range (200f, 3000f);
		}
	}
}

public class Planet : MonoBehaviour, IMineable {



	public Dictionary<RawResources,ResourceDeposit> ResourceDeposits = new Dictionary<RawResources, ResourceDeposit>();

	void AddDeposit(ResourceDeposit r){
		ResourceDeposits.Add (r.resource, r);
	}

	void AddDeposit(RawResources r){
		ResourceDeposits.Add (r,new ResourceDeposit(r));
	}

	public float Mine(RawResources r,float amount){
		if (ResourceDeposits[r] == null)
			return 0f;
		amount = amount * ResourceDeposits[r].access;
		if (amount <= ResourceDeposits[r].amount) {
			ResourceDeposits[r].amount -= amount;
			return amount;
		}
		return  0f;
	}

	// Use this for initialization
	void Awake () {
		for (int i = 1; i < 6; i++) {
			AddDeposit ((RawResources)i);
		}
		Collections.Mineable.Add (this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
