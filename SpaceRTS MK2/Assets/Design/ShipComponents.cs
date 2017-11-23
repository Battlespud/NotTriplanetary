using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;

public enum CompCategory{
	DEFAULT,
	REQUIRED,
	ENGINE,
	WEAPON,
	UTILITY
};

//probably ditch
public enum CompClass{
	SHIP = 0,
	FIGHTER
}

public enum AbilityCats{
	THRUST,  //rating is enginetype as int, rating 2 is power modifier, .25 = 1.25, -.25 = .75 etc, Thermal reduction, 0-.95
	TURN,    //*/s
	CONTROL, //rating is # of ships in fleet. default is not fleet command ship.
	CREW,    //Life support + quarters
	SENSOR,  //type(0,1,2) cast from enum, sensitivity(1-100), hardening(0-1)
	USEFUEL,  //uses this amount of fuel when active, per turn
	FUEL,	  //fuel storage. Rating = current. Rating 2 = Max, Rating 3 = Explosion chance 0.00-1.00
	HANGAR,   //fighter/fac storage
	MAINT,  //Engineering Space, rating is effective mass (at higher tech levels it may be more effective than its actual mass), rating2 is current Spare parts count. Rating 3 is Max spare parts count.
	POW //rating is generates.  rating 1 is requires
}

public enum SENSORTYPES{
	EM = 0,
	THERMAL = 1,
	ACTIVE = 2
}

//Adds functionality to a component.  Any number may be added without issue, but too many may not be balanced or make any sort of sense realistically.
public class Ability{
	public AbilityCats AbilityType;
	public float Rating;
	public float Rating2;
	public float Rating3;
	public float thrust; //only used by engines

	public Ability DeepClone(){
		Ability a = new Ability ();
		a.AbilityType = AbilityType;
		a.Rating = Rating;
		a.Rating2 = Rating2;
		a.Rating3 = Rating3;
		a.thrust = thrust;
		return a;
	}
}

//Contains all information about what a component or ship is putting out in terms of radiation.  Posesses basic utility functions.
public class Emissions{
	public float EM;
	public float TH;
	public float TCS;
	public Emissions(float em, float th, float tcs){
		EM = em;
		TH = th;
		TCS = tcs;
	}
	public Emissions Clone(){
		return new Emissions (EM, TH, TCS);
	}
	public void Clear(){
		EM = 0;
		TH = 0;
		TCS = 0;
	}
	public void Add(Emissions e){
		EM += e.EM;
		TH += e.TH;
		TCS += e.TCS;
	}
}

//Provides access to the Component Dictionary and is a general identifier for everything an empire or corporation involves itself in.
	//it is possible to grant others use of an empire or corporations token.  Corps will provide access for a price, while empires generally share their tokens with alliance partners or overlords.
public class DesignerToken{
	public string OwnerName;

	public DesignerToken(string s){
		OwnerName = s;
		ShipComponents.RegisterToken (this);
	}
}

public class ShipComponents {

	#region Static Component Storage and identities

	static List<DesignerToken>MasterDesignerTokenList = new List<DesignerToken>();

	//All designed components are added here in the list under their creators Designer Token.  Empire scan only build components in their own lists, or in those of whom they posess Tokens (protectorates, allies etc).
	static Dictionary<DesignerToken,List<ShipComponents>> MasterComponentDictionary = new Dictionary<DesignerToken, List<ShipComponents>>();

	//Public domain components are older components that through constant use have become common knowledge, they can be built by anyone with the appropriate tech.
	static List<ShipComponents> PublicDomainComponents = new List<ShipComponents> ();

	static public void AddComponentToPublicDomain(ShipComponents c){
		PublicDomainComponents.Add (c);
		UpdatePublicDomain ();
	}

	//todo, after adding a List<Empire> Contacted to each empire, add a check here that empires only get components from known others. just check tokens against contacted.
	static void UpdatePublicDomain(){
		foreach (List<ShipComponents> l in MasterComponentDictionary.Values) {
			foreach (ShipComponents c in PublicDomainComponents) {
				if (!l.Contains (c))
					l.Add (c);
			}
		}
	}

	static public void RegisterToken (DesignerToken T){
		if(!MasterComponentDictionary.ContainsKey(T)){
			MasterComponentDictionary.Add (T, new List<ShipComponents> ());
			MasterDesignerTokenList.Add (T);
			UpdatePublicDomain ();
		}
	}

	static public void AddDesign(DesignerToken T, ShipComponents C){
		if (!MasterComponentDictionary.ContainsKey (T)) 
			RegisterToken (T);
		MasterComponentDictionary [T].Add (C);
	}

	static public List<ShipComponents>GetComponents(DesignerToken T){
		if (!MasterComponentDictionary.ContainsKey (T)) 
			RegisterToken (T);
		return MasterComponentDictionary [T];
	}

	#endregion

	#region Fields
	public bool Obsolete = false; //Obsolete components will be hidden in UI.  Only affects the parent component. Players should set this to true on older components they dont want to see anymore.
	public bool Default = false;  //This component will be automatically added to all players available components list.  Used for required, basic stuff like crew quarters, life support and bridge.

	public string Name;
	public string Description;

	public ShipComponents OriginalReference; //This will be null for the original component design.  Its only so that built versions have a reference to the original if we need to check something.Also lets us implement an upgrade system later should we so choose.

	public DesignerToken Designer;  //Contains information about who originally designed this component.

	public string DesignDate; //set automatically when component is created and added to lists.
	public string BuildDate;  //set when component is cloned


	public CompCategory Category = CompCategory.DEFAULT;  //Where the component will be categorized in the design view.
	public CompClass compClass = CompClass.SHIP;          //Who can use this component.

	public List<Ability> Abilities = new List<Ability> (); //What this component does.
	public List<Tech> Requirements = new List<Tech> ();    //What techs are required for this component to be built

	public int Mass; //Bigger components generally take more hits to destroy.
	public int CrewRequired = 0; //Adds to the ships minimum crew requirement.
	public float MaintReq; //How many spare parts needed.

	public Emissions emissions;  //What type of radiation this component emits when active.

	public bool Enabled = true;   //will be checked for abilities and emissions when true
	public bool Toggleable = false;  //component can be toggled enabled or not.  Most components are always enabled.

	public bool Interior = true; //inside the ship, cant be targeted. External components, like weapons and sensors, can be targeted by fighters

	public Dictionary<RawResources, float> Cost = new Dictionary<RawResources, float>();  //what it costs to build this
	#endregion



	//Used for the design UI buttons
	public string GenerateDesignString(){
		if (Category == CompCategory.ENGINE) {
			return string.Format ("{0} {1}kt  {2}EP", Name, Mass, GetThrust());
		}
		return string.Format ("{0} {1}kt  {2}C", Name, Mass, CrewRequired); //todo
	}




	#region Damage
	//Hits To Kill.  An incoming shot of this damage or more is guarenteed to destroy the component if hit.  Anything lesser will cause a roll based on the disparity between damage and HTK.
		//HTK of 0 is valid and will be destroyed by any damage.  HTK 0 components do not absorb damage, they pass it on.  This means a DAM 1 hit can destroy an infinite number of HTK 0 components.  Use this to prevent players from abusing spam to absorb obscene amounts of damage.
	private int HTK = 1;
	public int GetHTK(){
		return HTK;
	}
	public void SetHTK(int? i = null){
		if(i==null)
			HTK = Mass / 100;
		else
			HTK = (int)i;
	}

	//Components are either destroyed or undamaged, there is no inbetween.  Chances to damage are based on incoming damage vs HTK.
	private bool Damaged = false; 
	public bool isDamaged(){
		return Damaged;
	}
	public void Damage(){
		Debug.Log(Name + " has been damaged.");
		Damaged = true;
		SetSpareParts (0);
	}
	public void Fix(){
		Damaged = false;
	}
	#endregion

	//GETTERS
	#region Ability Getters
	public float GetThrust(){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.THRUST)
				return a.thrust;
		}
		return 0f;
	}
	public float GetTurnThrust(){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.TURN)
				return a.Rating;
		}
		return 0f;
	}
	public float GetQuarters(){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.CREW)
				return a.Rating;
		}
		return 0f;
	}
	public bool isControl(){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.CONTROL)
				return true;
		}
		return false;
	}
	public bool isMaint(){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.MAINT)
				return true;
		}
		return false;
	}
	public float getMaintMass(){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.MAINT)
				return a.Rating;
		}
		return 0f;
	}
	public float GetCurrentSpareParts(){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.MAINT)
				return a.Rating2;
			}
		return 0f;
	}
	public float GetMaxSpareParts(){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.MAINT)
				return a.Rating3;
		}
		return 0f;
	}
	public void UseSpareParts(float f){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.MAINT) {
				a.Rating2 -= f;
			}
		}
		return;
	}
	public void SetSpareParts(float f){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.MAINT)
				a.Rating2 = f;
			if (a.Rating2 > a.Rating3)
				a.Rating2 = a.Rating3;
		}
	}

	public bool isFuel(){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.FUEL)
				return true;
		}
		return false;
	}
	public void SetFuel(float f){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.FUEL)
				a.Rating = f;
			if (a.Rating > a.Rating2)
				a.Rating = a.Rating2;
		}
	}
	public float GetFuelUse(){
		foreach (Ability a in Abilities) {
			if (a.AbilityType == AbilityCats.USEFUEL)
				return a.Rating;
		}
		return 0;
	}
	#endregion


	#region AddAbility //Use this to add abilities to the component, its dense but its the easiest way tbh.  To understand what each field stands for, check the enum at the top of this file.
	/// <summary>
	/// Adds the ability.
	/// </summary>
	/// <param name="ability">Ability.</param>
	/// <param name="rate">(float)EngineType for engines. Otherwise just amount. For shields: 1 is directional, 0 is wall</param>
	/// <param name="rate2">Powermodifier as decimal. Only required for engines. For shields, x coord, only if directional</param>
	/// <param name="rate3">If engine, thermal reduction, 0-.95, higher is stealthier. For shields, y coord, only if directional</param>
	public void AddAbility(AbilityCats ability, float rate = 0, float rate2 = 0f, float rate3 = 0f){
		Ability a = new Ability ();
		a.AbilityType = ability;
		a.Rating = rate;
		a.Rating2 = rate2;
		a.Rating3 = rate3;
		if (a.AbilityType == AbilityCats.THRUST) {
		//	Debug.Log ("Designing engine...");
			a.thrust = (Mass / 50) * (1 + rate2 * rate);
			Category = CompCategory.ENGINE;
		}
		Abilities.Add (a);
	}
	#endregion

	#region ConstructorEquivalents
	//Use this whenever adding a component to a ship.
	public ShipComponents CloneProperties(){
		ShipComponents dest = new ShipComponents ();
		dest.OriginalReference = this;
		dest.Mass = Mass;
		foreach (Ability a in Abilities) {
			Ability d;
			d = a.DeepClone();
			dest.Abilities.Add (d);
		}
		if(emissions != null)
			dest.emissions = emissions.Clone ();

		dest.CrewRequired = CrewRequired;

		dest.HTK = HTK;
		dest.Name = Name;

		dest.Obsolete = Obsolete;
		dest.Category = Category;

		dest.BuildDate = StrategicClock.GetDate ();
		dest.DesignDate = DesignDate;
		return dest;
	}


	//Only use constructor for reference components built to be added to the appropriate empire lists.  Do not use for adding to ships.  Use clone instead.
	public ShipComponents(){
		DesignDate = StrategicClock.GetDate ();
		BuildDate = DesignDate;
	}
	#endregion

}
