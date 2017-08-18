using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment {

	public bool Equipped = false;
	public Player p;

	public GameObject Prefab;

	public void Equip(Player pa){
		p = pa;
		Equipped = true;
		if (Prefab)
			Prefab.SetActive (true);
		else {
			Load ();
		}
		OnEquip ();
	}

	public void Unequip(){
		Equipped = false;
		if (Prefab)
			Prefab.SetActive (false);
		OnUnequip ();
	}

	public abstract void OnEquip();
	public abstract void OnUnequip();
	public abstract void Load();
}
