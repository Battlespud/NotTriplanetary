﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public Rigidbody rb;
	//Equipment
	public List<Equipment> Equips = new List<Equipment>();
	private int EquipIndex;
	private Equipment equip;
	[SerializeField]string equipName;
		//equip ui
	public Text RepairText;



	public float Health = 100;
	const float mHealth = 100;


	void Awake(){
		rb = GetComponent<Rigidbody> ();

	}

	void ConstructEquips(){
		Equips.Add (new EmptyEquipment ());
		Equips.Add(new Analyzer());
		Equips.Add (new ElectricRepairTool ());
	}

	// Use this for initialization
	void Start () {
		ConstructEquips();
		Equip (0);
	}
	
	// Update is called once per frame
	void Update () {

		CheckHealth ();
		//probably not neccessary because of parenting.
	//	equip.Prefab.transform.position = transform.position;
	//	equip.Prefab.transform.rotation = transform.rotation;
		if (Input.GetKeyDown (KeyCode.I))
			ScrollEquipment ();
	}



	public void Damage(float f){
		Health -= f;
	}

	void CheckHealth(){
		if (Health > mHealth)
			Health = mHealth;
		if (Health <= 0f)
			Die ();
	}

	void Die(){
		Debug.Log ("u r ded");
		Destroy (gameObject);
	}

	void ScrollEquipment(){
		EquipIndex++;
		if (EquipIndex >= Equips.Count)
			EquipIndex = 0;
		Equip ();
	}

	void Equip(){
		if(equip!=null)
			equip.Unequip ();
		equip = Equips [EquipIndex];
		equip.Equip (this);
		equipName = equip.ToString ();
	}
	void Equip(int a){
		if(equip!=null)
			equip.Unequip ();
		EquipIndex = a;
		equip = Equips [EquipIndex];
		equip.Equip (this);
		equipName = equip.ToString ();

	}

}