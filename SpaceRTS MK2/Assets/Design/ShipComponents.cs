using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
	public float TurnRate;

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
		dest.TurnRate = TurnRate;
		dest.FuelConsumption = FuelConsumption;
		dest.Obsolete = Obsolete;
		dest.Category = Category;
		dest.control = control;
		dest.flagControl = flagControl;
		return dest;
	}
}
