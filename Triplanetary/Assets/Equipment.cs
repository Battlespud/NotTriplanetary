using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Equipment : MonoBehaviour {

	public bool Equipped = false;
	Player p;

	public virtual void Equip(Player p){
		Equipped = true;
		gameObject.SetActive (true);
	}

	public virtual void Unequip(){
		Equipped = false;
		gameObject.SetActive (false);
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
