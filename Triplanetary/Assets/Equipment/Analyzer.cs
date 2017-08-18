using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analyzer : Equipment {
	public override void Load(){
		Prefab = GameObject.Instantiate(Resources.Load<GameObject>("PowerCamera") as GameObject);
	}

	public override void OnEquip(){
		Prefab.transform.position = p.GetComponent<MovementController> ().cam.transform.position;
		Prefab.transform.rotation = p.GetComponent<MovementController> ().cam.transform.rotation;
		Prefab.transform.parent =  p.GetComponent<MovementController> ().cam.transform;
	}

	public override void OnUnequip(){
		//No extra requirements for analyzers
	}
}
