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
	public ShipDesign NextTooling;

	public void Retool(ShipDesign newDes){
		NextTooling = newDes;
		TimeToNextTooling = CalcToolingTime (newDes);
	}

	int CalcToolingTime(ShipDesign newDes){
		int currM = 0;
		if (CurrentTooling != null) {
			currM = (int)CurrentTooling.BuildCost;
		} 
		return (int)(3+Mathf.Abs(currM - newDes.BuildCost)/Rate*3f);
	}
	public int TimeToNextTooling;

	public int MaxTonnage;
	public int Berths;


	//build rate
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
		if (NextTooling != null) {
			TimeToNextTooling -=1;
			if (TimeToNextTooling <= 0) {
				CurrentTooling = NextTooling;
				NextTooling = null;
			}
		}
		foreach (Slipway s in Slipways) {
			s.Progress();
		}
	}

	void PhaseManager(Phase p){
		switch (p) {
		case(Phase.ORDERS):
			{
				Process ();
				break;
			}
		case(Phase.GO):
			{
				break;
			}
		case (Phase.REVIEW):
			{
				break;
			}
		case (Phase.INTERRUPT):
			{
				break;
			}

		}	
	}
}
