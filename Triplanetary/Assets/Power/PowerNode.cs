using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerNode : MonoBehaviour, IRepairable {

	public string NodeID ="";

	public bool Functional = true;

	public bool IsFunctional(){
		return Functional;
	}

	public string GetName(){
		return "Node: " + NodeID;
	}

	public void Repair(){
		Functional = true;
	}

	// Use this for initialization
	void Start () {
		NodeID = NameGen.GenerateName ("L3N2L1");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
