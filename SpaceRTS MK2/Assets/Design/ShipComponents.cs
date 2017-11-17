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

}

public enum SENSORTYPES{
	EM = 0,
	THERMAL = 1,
	ACTIVE = 2
}

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

public struct Emissions{
	public float EM;
	public float TH;
	public float TCS;
}

public class ShipComponents {
	public static List<ShipComponents> DesignedComponents = new List<ShipComponents> ();   //TODO: Make these dictionaries that take Empire and return a list
	public static List<ShipComponents> DesignedFighterComponents = new List<ShipComponents> ();
	public static List<int>UsedID = new List<int>();
	public static Dictionary<int,ShipComponents> IDComp = new Dictionary<int, ShipComponents> ();

	public bool Obsolete = false; //Obsolete components will be hidden in UI
	public bool Default = false;

	public string Name;
	public int ID; //unique identifier

	public string Description;

	public CompCategory Category = CompCategory.DEFAULT;
	public CompClass compClass = CompClass.SHIP;

	public List<Ability> Abilities = new List<Ability> ();


	public int Mass; //tons
	public int CrewRequired = 0;
	public float powerReq;
	public float ThermalSig;
	public float EMSig;
	public float MaintReq; //How many spare parts needed.

	public bool Enabled = true;
	public bool toggleable = false;

	public bool Interior = true; //inside the ship, cant be targeted

	Dictionary<RawResources, float> Cost = new Dictionary<RawResources, float>();  //what it costs to buidl this


	public string GenerateDesignString(){
		if (Category == CompCategory.ENGINE) {
			return string.Format ("{0} {1}kt  {2}EP", Name, Mass, GetThrust());
		}
		return string.Format ("{0} {1}kt  {2}C", Name, Mass, CrewRequired); //todo
	}

	int HTK = 1;
	public int GetHTK(){
		return HTK;
	}
	public void SetHTK(int i){
		HTK = i;
	}

	public void AutoSetHTK(){
		HTK = Mass / 100;
	}

	bool Damaged = false;   
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
			Debug.Log ("Designing engine...");
			a.thrust = (Mass / 50) * (1 + rate2 * rate);
			Category = CompCategory.ENGINE;
		}
		Abilities.Add (a);
	}

	public ShipComponents CloneProperties(){
		ShipComponents dest = new ShipComponents ();
		dest.Mass = Mass;
		foreach (Ability a in Abilities) {
			Ability d;
			d = a.DeepClone();
			dest.Abilities.Add (d);
		}
		dest.ThermalSig = ThermalSig;
		dest.EMSig = EMSig;
		dest.CrewRequired = CrewRequired;

		dest.HTK = HTK;
		dest.Name = Name;

		dest.Obsolete = Obsolete;
		dest.Category = Category;

		return dest;
	}


	public void GetFields(){
		List<FieldInfo> Fields = new List<FieldInfo> ();
		Fields.AddRange (GetType ().GetFields ());
		List<FieldInfo> AbilityFields = new List<FieldInfo> ();
	//	Debug.Log ("Fields found: " + Fields.Count);
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Components/" + Name + "REFLECTED.txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			foreach (FieldInfo f in Fields) {
				//Debug.Log (f.GetValue(this));\
				if (f.GetValue (this) != null) {
					writer.WriteLine (
						"Name: " + f.Name + ":\t" + f.GetValue (this).ToString ());
				}
			}
			foreach (Ability a in Abilities) {
				writer.Write ("\n");
				AbilityFields.AddRange(a.GetType ().GetFields ());
				foreach (FieldInfo g in AbilityFields) {
//					Debug.Log(AbilityFields.Count + " fields per ability detected");
					writer.WriteLine(
						g.Name + ":\t" + g.GetValue (a).ToString());
				}
				AbilityFields.Clear ();
			}
		}
	}
	/*


	public static void Save(ShipComponents c){
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Components/" + c.Name + ".txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			writer.WriteLine (c.Name ); //0
			writer.WriteLine (((int)c.Category).ToString ());
			writer.WriteLine (c.Mass);  //2
			writer.WriteLine (c.toggleable);
			writer.WriteLine (c.PassiveSig); //4
			writer.WriteLine (c.CrewRequired);
			writer.WriteLine (c.quarters ); //6
			writer.WriteLine (c.lifeSupport);
			writer.WriteLine (c.powerReq); //8
			writer.WriteLine (c.control);
			writer.WriteLine (c.flagControl ); //10
			writer.WriteLine (c.HTK);
			writer.WriteLine (c.isEngine); //12
			writer.WriteLine (c.Thrust);
			writer.WriteLine (c.TurnThrust); //14
			writer.WriteLine (c.FuelConsumption); //15
		}
	}

	static bool ToBool(string s){
		if (s == "True")
			return true;
		return false;
	}

	//TODO add component ID, enginetype, power modifier 

	public static ShipComponents LoadPath(string path, bool addToList){
		ShipComponents c = new ShipComponents ();
		string[] data = File.ReadAllLines (path);
		c.Name = data [0];
		c.Category = (CompCategory)(int.Parse(data [1] ));
		c.Mass = int.Parse(data [2]);
		c.toggleable = ToBool (data[3]);
		c.PassiveSig = float.Parse(data[4]);
		c.CrewRequired = int.Parse(data[5]);
		c.quarters = int.Parse(data [6]);
		c.lifeSupport = int.Parse(data [7]);
		c.powerReq = float.Parse(data [8]);
		c.control = ToBool(data[9]);
		c.flagControl= ToBool(data[10]);
		c.HTK = int.Parse(data[11]);
		c.isEngine = ToBool(data[12]);
		c.Thrust = float.Parse(data [13]);
		c.TurnThrust = float.Parse(data [14]);
		c.FuelConsumption = float.Parse(data [15]);
		if (addToList) {
			DesignedComponents.Add (c);
		}
		return c;
	}

	public static void LoadAllComponents(){
		string[] filePaths = Directory.GetFiles (System.IO.Path.Combine (Application.streamingAssetsPath, "Components/"), "*.txt");
		foreach (string s in filePaths) {
			LoadPath (s, true);
		}
	}

	public static ShipComponents Load(string name, bool addToList){
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Components/" + name + ".txt"); 
		ShipComponents c = new ShipComponents ();
		string[] data = File.ReadAllLines (path);
		c.Name = data [0];
		c.Category = (CompCategory)(int.Parse(data [1] ));
		c.Mass = int.Parse(data [2]);
		c.toggleable = ToBool (data[3]);
		c.PassiveSig = float.Parse(data[4]);
		c.CrewRequired = int.Parse(data[5]);
		c.quarters = int.Parse(data [6]);
		c.lifeSupport = int.Parse(data [7]);
		c.powerReq = float.Parse(data [8]);
		c.control = ToBool(data[9]);
		c.flagControl= ToBool(data[10]);
		c.HTK = int.Parse(data[11]);
		c.isEngine = ToBool(data[12]);
		c.Thrust = float.Parse(data [13]);
		c.TurnThrust = float.Parse(data [14]);
		c.FuelConsumption = float.Parse(data [15]);
		if (addToList) {
			DesignedComponents.Add (c);
		}
		return c;
	}

	public static void LoadAllComponentsReflection(){
		string[] filePaths = Directory.GetFiles (System.IO.Path.Combine (Application.streamingAssetsPath, "Components/"), "*.txt");
		foreach (string s in filePaths) {
			LoadFields (s);
		}
	}



	public static void LoadFields(string name){
		List<FieldInfo> Fields = new List<FieldInfo> ();
		ShipComponents c = new ShipComponents();
		Fields.AddRange (c.GetType ().GetFields ());
		Debug.Log ("Fields found: " + Fields.Count);
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Components/" + name + "REFLECTED.txt"); 
		string[] data = File.ReadAllLines (path);
		foreach (string s in data) {

		}

	}

*/
}
