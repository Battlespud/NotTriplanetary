using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum DesignTypes{


}

public struct Range{
	public int start;
	public int end;
	public bool Contains(int a){
		return (start <= a && end >= a);
	}
	public string ToString(){
		return string.Format ("[{0}-{1}]",start,end);
	}
	public Range(int x, int y){
		start = x;
		end = y;
	}
}

public class ShipDesign {

	//1 space = 50KT

	public const int Slot = 50;
	public int MaxRoll;


	public static List<ShipDesign> Designs = new List<ShipDesign>();
	public static List<string> DesignNames = new List<string> ();
	public static Dictionary<string, ShipDesign> DesignDictionary = new Dictionary<string, ShipDesign>();


	public string DesignName;
	public HullDes HullDesignation;
	public float DeploymentTime;
	public int CrewMin;
	public int CrewBerths;

	public float mass;
	public int ArmorLayers;
	public int ArmorLength;
	public int mTorpedoes;

	public ShipPrefabTypes BaseType;//visual



	public List<ShipComponents> Components = new List<ShipComponents>();
	public Dictionary<int,ShipComponents> DAC = new Dictionary<int, ShipComponents> ();
	public Dictionary<ShipComponents,Range>DACRanges = new Dictionary<ShipComponents, Range>();


	public void SetupDAC(){
		int curr = 0;
		foreach (ShipComponents c in Components) {
			int pCounter = 0;
			int start = curr;
			for (int i = 0; c.Mass > i; i += Slot) {
				DAC.Add (curr, c);
				pCounter++;
				curr++;
			}
			int end = curr-1;
			DACRanges.Add(c,new Range(start,end));
			Debug.Log (c.Name + " given " + pCounter + " slots. " + DACRanges[c].ToString());
		}
		MaxRoll = curr; //exclusive
	}


	public ShipDesign(string d){
		Designs.Add (this);
		DesignName = d;
		DesignNames.Add (this.DesignName);
		DesignDictionary.Add (this.DesignName, this);
	}


	public void Output(){
		Debug.Log ("Printing blueprint of " + DesignName + " to text file..");
		string path="Assets/Output/Designs/" + DesignName + ".txt";
		using(StreamWriter writer = new StreamWriter(path)){
		writer.WriteLine ("Design Date: " + ClockStatic.clock.GetDate());
			writer.WriteLine ( "\n" + HullDesignation.Prefix + " " + DesignName + "-Class "+  HullDesignation.HullType + "\nMass: " + mass + "KT\nArmor Thickness: " + ArmorLayers+"\n" + "Crew: " + CrewMin 
				+ "\nSpare Berths: " + (CrewBerths - CrewMin) + "\n");
			foreach (ShipComponents c in Components) {
				writer.WriteLine (DACRanges[c].ToString() + " " + c.Name + ": ");
			}
		writer.Close ();
		}
		Debug.Log ("Done. Check " + path);
	}

}
