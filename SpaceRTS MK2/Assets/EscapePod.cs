﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod : MonoBehaviour {


	public int Survivors;
	public List<Character>SurvivorCharacters = new List<Character>();
	float lifespan = 240f;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.GetComponent<Ship> ()) {
			col.GetComponent<Ship> ().RescueSurvivors (this);
			foreach (Character c in SurvivorCharacters) {
				col.GetComponent<ShipClass> ().TransferCharacterTo (c);
			}
			SurvivorCharacters.Clear ();

		}
	}
	
	// Update is called once per frame
	void Update () {
		lifespan -= Time.deltaTime;
		if (lifespan <= 0f)
			foreach(Character m in SurvivorCharacters){
				m.Die();
			}
			Destroy (gameObject);
	}
}
