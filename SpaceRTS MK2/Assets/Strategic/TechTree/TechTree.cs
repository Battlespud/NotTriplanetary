using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Tech{
	public string Name;
	public string ID;
	public string Description;
	public TechSection Section;
	public TechSubSection SubSection;
	public int Cost;
	public List<string> Requirements;
	public bool Researched;

}

public enum TechSection{
	C3,
	Sensors,
	Power,
	Propulsion, 
	Society,
	Defensive,
	Offensive,
	EnergyWeapons,
	ProjectileWeapons,
	Missiles,
	SpecialWeapons, 
	Ground, 
	Cargo,
	Fighter,
	ResourceExtraction

};

public enum TechSubSection{
	Drive

};


public class TechTree{

	public List<Tech> AllTechnologies = new List<Tech>();
	public Dictionary<string,Tech>TechByID = new Dictionary<string, Tech>();

	public void AddTech(Tech t){
		AllTechnologies.Add (t);
		TechByID.Add (t.ID, t);
		Debug.Log ("Importing tech: " + t.Name);
		Debug.Log (string.Format ("Name: {0}, Requirements: {1}", t.Name, t.Requirements.Count));
	}

	public bool IsAvailable(Tech t){
		bool b = true;
		if (t.Requirements.Count < 1)
			return true;
		foreach (string req in t.Requirements) {
			try{
			if (!TechByID [req].Researched) {
				b = false;
			}
				}
			catch{
				Debug.LogError (req + " is an invalid key.");
			}
		}
		return b;
	}

	public List<Tech> GetAvailableTech(){
		List<Tech> Available = new List<Tech> ();
		foreach (Tech t in AllTechnologies) {
			if (!t.Researched && IsAvailable (t)) {
				Available.Add (t);
			}
		}
		return Available;
	}

}
