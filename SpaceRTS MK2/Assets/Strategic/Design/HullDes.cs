using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullDes {

	public static List<string> HullTypes = new List<string> ();
	public static Dictionary<string, HullDes> DesDictionary = new Dictionary<string, HullDes> ();
	// USS Enterprise (CV-01)

	public string HullType; //ie,Heavy Carrier, Missile Cruiser etc
	public string Prefix;  // CV, CG etc.  Will be appended to ship nam
	public int Number; 

	public HullDes(string hType, string pre){
		HullType = hType;
		Prefix = pre;
		Number = 01;
		HullTypes.Add (HullType);
		DesDictionary.Add (HullType, this);
	}

	static HullDes(){
		//setup some test ones
		HullDes Carrier = new HullDes("Carrier", "CV");
		HullDes Battleship = new HullDes ("Battleship", "BB");
		Debug.Log ("Made some hull designations. " + HullTypes.Count);
	}

}
