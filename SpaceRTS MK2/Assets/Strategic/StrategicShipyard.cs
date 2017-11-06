using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Slipway{
	public StrategicShipyard parent;
	public float BuildCost;
	public float TurnsToCompletion;
	public string NameOverride; 
	public bool InUse;

	public void Assign(){
		InUse = true;
		BuildCost = parent.CurrentTooling.BuildCost;
	}

	public void Cancel(){
		InUse = false;
		BuildCost = 0f;
	}

	public void Progress(){
		if (!InUse)
			return;
		BuildCost -= parent.Rate * (1 + (((parent.CurrentTooling.mass / 5000) - 1) / 2)); 
		TurnsToCompletion = (int)System.Math.Ceiling(BuildCost / (1 + (((parent.CurrentTooling.mass / 5000) - 1) / 2)));
		if (BuildCost <= 0)
			TurnsToCompletion = -1;
			Complete ();
	}

	public void Complete(){
		parent.CompleteShip ();
		InUse = false;
	}
}

public class StrategicShipyard : MonoBehaviour, IContext{

	//DEBUG
	public bool AddSlipwayPls = true;

	public ShipDesign CurrentTooling;
	public ShipDesign NextTooling;

	public Empire empire;

	List<StrategicShip>DockedShips = new List<StrategicShip>();

	public void Retool(ShipDesign newDes){
		NextTooling = newDes;
		TimeToNextTooling = CalcToolingTime (newDes);
	}

	public int CalcToolingTime(ShipDesign newDes){
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
		if (AddSlipwayPls) {
			AddSlipway ();
			AddSlipwayPls = false;
		}
	}

	float CalculateTime(){
		return	CurrentTooling.BuildCost / Rate;
	}

	public void CompleteShip(){
		StrategicShip s = new StrategicShip (CurrentTooling, empire);
		DockedShips.Add (s);
		//TODO actually make the ship reference for the fleet list.
	}

	public void AssignSlipway(){
		bool done = false;
		int i = 0;
		while(!done && i < Slipways.Count){
			if (!Slipways [i].InUse) {
				done = true;
				Slipways[i].Assign();
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

	public void AddSlipway(){
		Slipway s = new Slipway ();
		s.parent = this;
		Slipways.Add (s);
	}

	void OpenMenu(){
		Debug.Log ("Opening spaceyard ui");
		StrategicShipyardUIManager.Manager.Open (this);
	}

	public List<UnityAction> ContextActions()
	{
		List<UnityAction> actions = new List<UnityAction>(){new UnityAction(OpenMenu)};
		return actions;
	}

	public GameObject getGameObject(){
		return gameObject;
	}
}
