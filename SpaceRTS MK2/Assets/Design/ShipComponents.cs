using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;

public enum CompCategory{
	DEFAULT,
	REQUIRED,
	ENGINE,
	WEAPON
};

public class ShipComponents {

	public string Name;

	public CompCategory Category = CompCategory.DEFAULT;

	public static List<ShipComponents> DesignedComponents = new List<ShipComponents> ();
	public static List<int>UsedID = new List<int>();
	public static Dictionary<int,ShipComponents> IDComp = new Dictionary<int, ShipComponents> ();
	//probably wont be used if we stick with aurora damage system

	public bool Obsolete = false;


	public int Mass; //kilotons
	public bool Enabled = true;
	public bool toggleable = false;
	public float PassiveSig;

	public int CrewRequired = 0;

	public Dictionary<RawResources, float> Cost = new Dictionary<RawResources, float>();

	public int quarters; //space for crew/guests
	public int lifeSupport;

	public float powerReq;

	public bool control = false;
	public bool flagControl = false;

	public string GenerateDesignString(){
		if(isEngine)
			return string.Format ("{0} {1}kt  {2}Thrust", Name, Mass, Thrust); //todo
		

		return string.Format ("{0} {1}kt  {2}C", Name, Mass, CrewRequired); //todo
	}

	public int HTK = 1;
	public int GetHTK(){
		return HTK;
	}
	public void SetHTK(int i){
		HTK = i;
	}
	bool Damaged = false;
	public bool isDamaged(){
		return Damaged;
	}

	public bool isEngine = false;


	public float Thrust;
	public float TurnThrust;

	public float FuelConsumption; //per tick

	public void Damage(){
		Debug.Log(Name + " has been damaged.");
		Damaged = true;
	}

	public EngineTypes EngineType;
	float PowerModifier;

	static float BaseFuelConsumption = 1;

	public static ShipComponents DesignEngine(EngineTypes type, int m, float power = 1f){
		ShipComponents D = new ShipComponents ();
		D.isEngine = true;
		D.Category = CompCategory.ENGINE;
		D.EngineType = type;
		D.Mass = m;
		D.PowerModifier = power;
		D.HTK = m / 100;
		D.Thrust = (D.Mass / 50) * (power * (int)D.EngineType);
		return D;
	}

	public ShipComponents CloneProperties(){
		ShipComponents dest = new ShipComponents ();
		dest.Mass = Mass;
		dest.PassiveSig = PassiveSig;
		dest.CrewRequired = CrewRequired;
		dest.quarters = quarters;
		dest.lifeSupport = lifeSupport;
		dest.HTK = HTK;
		dest.Name = Name;
		dest.isEngine = isEngine;
		dest.Thrust = Thrust;
		dest.TurnThrust = TurnThrust;
		dest.FuelConsumption = FuelConsumption;
		dest.Obsolete = Obsolete;
		dest.Category = Category;
		dest.control = control;
		dest.flagControl = flagControl;
		return dest;
	}

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

	public void GetFields(){
		List<FieldInfo> Fields = new List<FieldInfo> ();
		Fields.AddRange (GetType ().GetFields ());
		Debug.Log ("Fields found: " + Fields.Count);
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Components/" + Name + "REFLECTED.txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			foreach (FieldInfo f in Fields) {
				Debug.Log (f.GetValue(this));
				writer.WriteLine(
					f.Name + ":\t" +
					f.GetValue (this).ToString());
			}
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
}
