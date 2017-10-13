using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipComponents {

	public static List<ShipComponents> DesignedComponents = new List<ShipComponents> ();

	public const float HealthCutoff = .4f; //Below this health components will not function

	public bool Obsolete = false;

	public string name;

	public int Mass; //kilotons
	public bool Enabled = true;
	public bool toggleable = false;
	public float PassiveSig;

	public int CrewRequired =25;

	public Dictionary<RawResources, float> Cost = new Dictionary<RawResources, float>();

	public int quarters; //space for crew/guests
	public int lifeSupport;

	public float powerReq;

	public bool control = false;
	public bool flagControl = false;

	public string GenerateDesignString(){
		return string.Format ("{0}    {1}kt  {2} crew", name, Mass, CrewRequired); //todo
	}

	public float Health = 1f;

	public bool isFunctional(){
		return Health >= HealthCutoff;
	}

	public static void CloneProperties(ShipComponents source, ShipComponents dest){
		dest.Mass = source.Mass;
		dest.PassiveSig = source.PassiveSig;
		dest.CrewRequired = source.CrewRequired;
		dest.Health = 1f;
		dest.name = source.name;
	}
}
