using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricRepairTool : Equipment {
	public override void Load(){
		Prefab = GameObject.Instantiate (Resources.Load<GameObject> ("ElectricRepairTool") as GameObject);
		Prefab.GetComponent<ElectricRepairToolComp> ().ActivationToolTipText = p.RepairText;
	}

	public override void OnEquip(){
		Prefab.transform.position = p.GetComponent<MovementController> ().transform.position;
		Prefab.transform.rotation = p.GetComponent<MovementController> ().transform.rotation;
		Prefab.transform.parent =  p.GetComponent<MovementController> ().transform;
	}

	public override void OnUnequip(){
	}
}
