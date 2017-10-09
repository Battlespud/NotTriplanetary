using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum DesignTypes{


}



public class ShipDesign {

	public static List<ShipDesign> Designs = new List<ShipDesign>();
	public static List<string> DesignNames = new List<string> ();
	public static Dictionary<string, ShipDesign> DesignDictionary = new Dictionary<string, ShipDesign>();


	public string DesignName;
	public HullDes HullDesignation;
	public float DeploymentTime;


	public float mass;
	public int ArmorLayers;
	public int mTorpedoes;

	public ShipPrefabTypes BaseType;//visual
//	public SpaceGun WeaponType;
//	public TargetingSensors Sensor;

	public List<ShipComponents> Components;
	public List<Engine> Engines;

	public ShipDesign(string d){
		Designs.Add (this);
		DesignName = d;
		DesignNames.Add (this.DesignName);
		DesignDictionary.Add (this.DesignName, this);
	}


	public void Output(){
		Debug.Log ("Output blueprint of " + DesignName + " to text..");
		string path="Assets/Resources/Designs/" + DesignName + ".txt";
		using(StreamWriter writer = new StreamWriter(path)){
		writer.WriteLine ("Design Date: " + ClockStatic.clock.GetDate());
		writer.WriteLine ( "\n" + HullDesignation.Prefix + " " + DesignName + "-Class "+  HullDesignation.HullType );
		writer.WriteLine ("\nMass: " + mass + "KT");
		writer.WriteLine ("\nArmor Thickness: " + ArmorLayers);
		writer.Close ();
		}
		Debug.Log ("Done. Check " + path);
	}

}
