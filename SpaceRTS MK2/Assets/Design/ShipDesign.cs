using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DesignTypes{


}



public class ShipDesign {

	public static List<ShipDesign> Designs = new List<ShipDesign>();

	public string DesignName;

	public float mass;
	public int ArmorLayers;
	public int mTorpedoes;

	public ShipPrefabTypes BaseType;
	public SpaceGun WeaponType;
	public TargetingSensors Sensor;

	public List<ShipComponents> Components;


	public ShipDesign(){
		Designs.Add (this);
	}


}
