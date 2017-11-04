using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tech{
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
	DesignedSystems,  //research for unlocking designed components
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

public struct ResearchProject{
	public Tech tech;
	public int startCost;
	public int cost;
	public Character Scientist;
	public int LabCount;

	public bool DoResearch(int Rate){
		cost -= LabCount * Rate;
		if (cost <= 0)
			return true;
		return false;
	}

	public ResearchProject(Tech t, Character s, int labs){
		tech = t;
		startCost = t.Cost;
		cost = startCost;
		Scientist = s;
		LabCount = labs;
	}
}


public class TechTree{

	public List<Tech> AllTechnologies = new List<Tech>();
	public Dictionary<string,Tech>TechByID = new Dictionary<string, Tech>();
	public List<ResearchProject>ResearchProjects = new List<ResearchProject>();

	public int ResearchRate = 12;

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

	public void CreateResearch(Character c, Tech t, int labs){
		bool NoPriorProject = true;
		foreach (ResearchProject p in ResearchProjects) {
			if (p.tech == t || c == p.Scientist)
				NoPriorProject = false;
		}
		if (NoPriorProject) {
			ResearchProjects.Add (new ResearchProject (t, c, labs));
		} else {
			Debug.Log ("Project already exists or Scientist is already working on a project.");
		}
	}

	public void DoResearch(){
		List<ResearchProject> Completed = new List<ResearchProject> ();
		foreach (ResearchProject r in ResearchProjects) {
			if (r.DoResearch (ResearchRate)) {
				Completed.Add (r);
				AddTech (r.tech);
				r.Scientist.DidResearch (r.tech.Name);
			}
		}
			foreach (ResearchProject rd in Completed) {
				ResearchProjects.Remove (rd);
		}
	}

}
