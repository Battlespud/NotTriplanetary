using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour, IRepairable {

	string GeneratorID = "";

	public bool Functional = true;


	public bool IsFunctional(){
		return Functional;
	}

	public void Repair(){
		Functional = true;
	}


	public string GetName(){
		return "Generator: " + GeneratorID;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
