using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//A Shipyard has multiple slipways.  Each slipway can build 1 ship at a time.
public class Slipway{
	
	//The Shipyard that this slipway is located at and whose instructions it follows. Empire can be accessed as parent.empire.
	public StrategicShipyard parent;
	
	//Based on a combination of Hull cost and ShipDesign cost.
	public float BuildCost;
	
	//Purely for ui purposes.
	public float TurnsToCompletion;
	
	//If we want to change the name of the ship that is being built, we can use this field. Unimplemented.
	public string NameOverride; 
	
	//The shipyard cannot retool while a slilpway is in use.
	public bool InUse;
	public ShipDesign Design;

	public void Assign(ShipDesign des){
		InUse = true;
		BuildCost = des.BuildCost;
		Design = des;
	}

	public void Cancel(){
		InUse = false;
		BuildCost = 0f;
	}

	//Progresses ship building by 1 turn
	public void Progress(){
		if (!InUse)
			return;
		float BuildRate = (parent.Rate * parent.empire.Stats.ShipyardBuildRate);
		BuildCost -= BuildRate ;   //TODO
		TurnsToCompletion = Mathf.CeilToInt(BuildCost / BuildRate) ;
		if (TurnsToCompletion <= 0)
			TurnsToCompletion = 1;
		if (BuildCost <= 0)
			Complete ();
	}

	//When we run down the build timer, we pass it back to the Shipyard to create the actual strategicship.
	public void Complete(){
		parent.CompleteShip (Design);
		InUse = false;
	}
}
