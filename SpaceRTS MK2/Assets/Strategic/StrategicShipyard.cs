using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//A Shipyard has multiple slipways.  Each slipway can build 1 ship at a time.
public class Slipway{
	public StrategicShipyard parent;
	public float BuildCost;
	public float TurnsToCompletion;
	public string NameOverride; 
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

	public void Complete(){
		parent.CompleteShip (Design);
		InUse = false;
	}
}

//Where ships are built
public class StrategicShipyard : MonoBehaviour, IContext, ILocation{

	//DEBUG
	public bool AddSlipwayPls = true;


	public string GetLocationName(){
		return ShipYardName;
	}
	public object GetLocation(){
		return (object)this;
	}
	public System.Type GetLocType(){
		return this.GetType ();
	}
	public void MoveCharacterToThis(Character c){
		UpdateSeniorOfficer ();
	}
	public void MoveCharacterFromThis(Character c){
		UpdateSeniorOfficer ();
	}
	public Vector3 GetPosition()
	{
		return transform.position;
	}

	public string GetSearchableString()
	{
		return ShipYardName + " Shipyard" + "Spaceyard" + "Build";
	}
	//Called whenever someone transfers in or out.
	void UpdateSeniorOfficer(){
		List<Character> Here = empire.GetCharactersAtLocation (this,OfficerRoles.Navy);
		if (Here[0] != null && Here [0] != SeniorOfficer) {
			if(SeniorOfficer != null)
				SeniorOfficer.StepDownSeniorOfficer (this);
			SeniorOfficer = empire.GetCharactersAtLocation (this) [0];
			SeniorOfficer.AppointSeniorOfficer (this);
		}
		else if (Here[0] == null)
			SeniorOfficer = null;
	}

	public void RequestDock(Fleet f){
		Debug.LogError (f.FleetName + " Requests Docking Clearance (Not an Error)");
		DockedShips.AddRange (f.Ships);
		DockedFleets.Add (f);
		f.PerformDock ();
	}

	public void RequestUndock(Fleet f){
		f.Ships.ForEach(x=>{
			if(DockedShips.Contains(x))
				DockedShips.Remove(x);
		});
		foreach (StrategicShip s in f.Ships) {
			float amount;
			if (s.GetFuelNeeded () < FuelSupply) {
				amount = s.GetFuelNeeded ();
			} else {
				amount = FuelSupply;
			}
			s.Refuel (amount);
			FuelSupply -= amount;
		}
		Debug.LogError ("Fleet refueled and cleared to depart.");

		DockedFleets.Remove (f);
		f.PerformUndock ();
	}


	//The commander of the station, automatically updated whenever someone transfers in or out.
	public Character SeniorOfficer;


	public string ShipYardName = "Imperial Yardworks";

	public ShipHull CurrentTooling;
	public ShipHull NextTooling;

	public Empire empire;

	//Constructed ships are added to this list so they can be moved to fleets and assigned officers.
	public List<StrategicShip>DockedShips = new List<StrategicShip>();
	public List<Fleet> DockedFleets = new List<Fleet> ();

	public void Retool(ShipHull newHull)
	{
			NextTooling = newHull;
			TimeToNextTooling = CalcToolingTime (newHull);
	}

	public int CalcToolingTime(ShipHull newHull)
	{
		int currSize = 0;
		if (CurrentTooling != null)
			currSize = CurrentTooling.Size;
		return (Mathf.CeilToInt(System.Math.Abs(currSize - newHull.Size) / empire.Stats.ShipyardToolRate));
	}

	public int TimeToNextTooling;

	public float FuelSupply = 1000000f;

	//build rate
	public float Rate = 1f;

	public List<Slipway>Slipways = new List<Slipway>();

	void Awake(){
		StrategicClock.PhaseChange.AddListener (PhaseManager);
	}

	// Use this for initialization
	void Start () {
		empire.Yards.Add (this);
		Empire.AllLocations.Add (this);
	}
	
	// Update is called once per frame
	void Update () {
		if (AddSlipwayPls) {
			AddSlipway ();
			AddSlipwayPls = false;
		}
	}

	//Creates the actual ship
	public void CompleteShip(ShipDesign design){
		StrategicShip s = new StrategicShip (design, empire);
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.MILITARY,4,empire,"STARSHIP CONSTRUCTED",string.Format("{0} has <color=green>finished construction</color> of a <color=silver>{1}</color>-Class {2}, the {3}.",ShipYardName,s.DesignClass.DesignName,s.DesignClass.HullDesignation.HullType,s.ShipName));
		DockedShips.Add (s);
		//TODO actually make the ship reference for the fleet list.  TODO is this still necessary?
	}

	//A slipway automatically starts working towards building a ship of type CurrentTooling.
	public void AssignSlipway(){
		bool done = false;
		int i = 0;
		while(!done && i < Slipways.Count){
			if (!Slipways [i].InUse) {
				done = true;
				Slipways[i].Assign(CurrentTooling);
			}
		}
	}

	//Call each turn.  Updates all retooling and builds.
	void ProcessToolingandBuilding(){
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
				ProcessToolingandBuilding(); 
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
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.TECH,5,empire,"SLIPWAY ADDED",string.Format("{0} has <color=green>added</color> a new <color=purple>Slipway</color>, bringing the total count up to {1}.",ShipYardName,Slipways.Count));
	}

	void OpenMenu(){
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
	
	
	//UI Interfacing

	public List<ShipDesign> GetValidDesigns()
	{
		List<ShipDesign> ValidDesigns = new List<ShipDesign>();
	}
	
	
}
