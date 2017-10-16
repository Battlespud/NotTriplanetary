using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public enum CompCategory{
	DEFAULT,
	REQUIRED,
	ENGINE,
	WEAPON
};

public class ShipComponents {

	public CompCategory Category = CompCategory.DEFAULT;

	public static List<ShipComponents> DesignedComponents = new List<ShipComponents> ();

	//probably wont be used if we stick with aurora damage system

	public bool Obsolete = false;

	public string name;

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
		return string.Format ("{0}    {1}kt  {2}C", name, Mass, CrewRequired); //todo
	}

	int HTK = 1;
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

	public bool[,] Armor;

	public bool isEngine = false;


	public float Thrust;
	public float TurnThrust;

	public float FuelConsumption; //per tick

	public void Damage(){
		Debug.Log(name + " has been damaged.");
		Damaged = true;
	}



	public ShipComponents CloneProperties(){
		ShipComponents dest = new ShipComponents ();
		dest.Mass = Mass;
		dest.PassiveSig = PassiveSig;
		dest.CrewRequired = CrewRequired;
		dest.quarters = quarters;
		dest.lifeSupport = lifeSupport;
		dest.HTK = HTK;
		dest.name = name;
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
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Components/" + c.name + ".txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			writer.WriteLine (c.name ); //0
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

	public static ShipComponents LoadPath(string path, bool addToList){
		ShipComponents c = new ShipComponents ();
		string[] data = File.ReadAllLines (path);
		c.name = data [0];
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
		c.name = data [0];
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


}
