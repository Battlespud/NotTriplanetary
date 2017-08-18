using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyEquipment : Equipment {

	public override void Load(){
		Prefab = new GameObject ();
		Prefab.name = "Nothing";
	}

	public override void OnEquip(){
		Prefab.transform.position = p.GetComponent<MovementController> ().cam.transform.position;
		Prefab.transform.rotation = p.GetComponent<MovementController> ().cam.transform.rotation;
		Prefab.transform.parent =  p.GetComponent<MovementController> ().cam.transform;
	}

	public override void OnUnequip(){
	}

}
