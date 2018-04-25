using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;



public class StrategicShipyard : MonoBehaviour, IContext, ILocation{

	//DEBUG //TODO REMOVE
	public bool AddSlipwayPls = true;



	public string ShipYardName = "Imperial Yardworks";

	////////////////////////////////////////////////////////////////////////////////////////////////////
	//LOCATION
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//Called whenever someone transfers in or out.
	
	
	//Owner
	public Empire empire;

	
	//The commander of the station, automatically updated whenever someone transfers in or out.
	public Character SeniorOfficer;
	
	//Called whenever a character transfers in or out of this location.
	void UpdateSeniorOfficer()
	{
		List<Character> Here = new List<Character>();
		Here.AddRange(empire.GetCharactersAtLocation (this,OfficerRoles.Navy));
		if(Here.Count <1)
			Here.AddRange(empire.GetCharactersAtLocation (this,OfficerRoles.Government));
		if (Here[0] != null && Here [0] != SeniorOfficer) {
			if(SeniorOfficer != null)
				SeniorOfficer.StepDownSeniorOfficer (this);
			SeniorOfficer = empire.GetCharactersAtLocation (this) [0];
			SeniorOfficer.AppointSeniorOfficer (this);
		}
		
		else if (Here[0] == null)
			SeniorOfficer = null;
	}



	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//FLEETS 
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//The fuel supply of this Shipyard.  Departing ships are automatically refueled.
	public float FuelSupply = 1000000f; //TODO set to 0 to start and set an  upper limit.

	
	//Constructed ships are added to this list so they can be moved to fleets and assigned officers.
	public List<StrategicShip>DockedShips = new List<StrategicShip>();
	public List<Fleet> DockedFleets = new List<Fleet> ();
	
	
	
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
		Debug.LogError ("Fleet refueled and cleared to depart. (Not an Error)");
		DockedFleets.Remove (f);
		f.PerformUndock ();
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//CONSTRUCTION 
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	
	public int TimeToNextTooling;

	//build rate of this specific shipyard, this is multiplied by the empires build rate as a whole.
	public float Rate = 1f;
	
	//The Hull this shipyard can currently build, or what it is working towards
	public ShipHull CurrentTooling;
	public ShipHull NextTooling;

	//Slipways represent a single drydock where a single ship can be built.
	public List<Slipway>Slipways = new List<Slipway>();



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
	
	//Called each turn.  Updates all builds.
	void ProcessBuilding()
	{
		foreach (Slipway s in Slipways) {
			s.Progress();
		}
	}
	
	//Called each turn.  Updates all retooling.
	void ProcessTooling()
	{
		if (NextTooling == null)
			return;
		//Shipyard can't retool while a slipway is in use.
		foreach (Slipway s in Slipways)
		{
			if (s.InUse)
				return;
		}
			TimeToNextTooling -=1;
			if (TimeToNextTooling <= 0) {
				CurrentTooling = NextTooling;
				NextTooling = null;
		}
	}

	//Creates the actual strategic ship
	public void CompleteShip(ShipDesign design){
		StrategicShip s = new StrategicShip (design, empire);
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.MILITARY,4,empire,"STARSHIP CONSTRUCTED",string.Format("{0} has <color=green>finished construction</color> of a <color=silver>{1}</color>-Class {2}, the {3}.",ShipYardName,s.DesignClass.DesignName,s.DesignClass.HullDesignation.HullType,s.ShipName));
		DockedShips.Add (s);
		//TODO actually make the ship reference for the fleet list.  TODO is this still necessary?
	}
	
	//Returns a list of ship designs that can be built for the currently tooled Hull.
	public List<ShipDesign> GetValidDesigns()
	{
		List<ShipDesign> ValidDesigns = new List<ShipDesign>();
		try
		{
			ValidDesigns.AddRange(ShipDesign.DesignsByHullDictionary[CurrentTooling]);
		}
		catch
		{
			Debug.LogError("No valid ship designs for Hull: " + CurrentTooling.HullName);
		}
		return ValidDesigns;
	}
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//UNITY 
	////////////////////////////////////////////////////////////////////////////////////////////////////

	
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
	
	void PhaseManager(Phase p){
		switch (p) {
			case(Phase.ORDERS):
			{
				ProcessBuilding();
				ProcessTooling();
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
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//Actions
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//A slipway automatically starts working towards building a ship of type CurrentTooling. Triggered by UI.
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

	public void AddSlipway(){
		Slipway s = new Slipway ();
		s.parent = this;
		Slipways.Add (s);
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.TECH,5,empire,"SLIPWAY ADDED",string.Format("{0} has <color=green>added</color> a new <color=purple>Slipway</color>, bringing the total count up to {1}.",ShipYardName,Slipways.Count));
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	//INTERFACE
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
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
	
	
}
