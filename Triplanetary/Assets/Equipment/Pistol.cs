using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Equipment {
	public override void Load(){
		Prefab = GameObject.Instantiate(Resources.Load<GameObject>("Pistol") as GameObject);
	}

	public override void OnEquip(){
		Prefab.transform.position = p.GetComponent<Slots> ().Hand.transform.position;
		Prefab.transform.rotation = p.GetComponent<Slots> ().Hand.transform.rotation;
		Prefab.transform.parent =  p.GetComponent<Slots> ().Hand.transform;
	}

	public override void OnUnequip(){
		//No extra requirements for analyzers
	}
}
