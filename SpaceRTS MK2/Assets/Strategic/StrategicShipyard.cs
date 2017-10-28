using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Slipway{
	public StrategicShipyard parent;
	public float TurnsToCompletion;
	public string NameOverride; 
	public bool InUse;

	public void Assign(float time){
		InUse = true;
		TurnsToCompletion = time;
	}

	public void Progress(){
		if (!InUse)
			return;
		TurnsToCompletion -= 1;
		if (TurnsToCompletion <= 0)
			Complete ();
	}

	public void Complete(){
		parent.CompleteShip ();
		InUse = false;
	}
}

public class StrategicShipyard : MonoBehaviour {

	public ShipDesign CurrentTooling;


	public int MaxTonnage;
	public int Berths;

	public float Rate = 1f;

	public List<Slipway>Slipways = new List<Slipway>();

	void Awake(){
		StrategicClock.PhaseChange.AddListener (PhaseManager);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	float CalculateTime(){
		return	CurrentTooling.BuildCost / Rate;
	}

	public void CompleteShip(){
		//TODO actually make the ship reference for the fleet list.
	}

	public void AssignSlipway(){
		bool done = false;
		int i = 0;
		while(!done && i < Slipways.Count){
			if (!Slipways [i].InUse) {
				done = true;
				Slipways[i].Assign(CalculateTime());
			}
		}
	}

	void Process(){
		foreach (Slipway s in Slipways) {
			s.Progress();
		}
	}

	void PhaseManager(Phase p){
		switch (p) {
		case(Phase.ORDERS):
			{
				break;
			}
		case(Phase.GO):
			{
				break;
			}
		case (Phase.REVIEW):
			{
				Process ();
				break;
			}
		case (Phase.INTERRUPT):
			{
				break;
			}

		}	
	}
}
