using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class HistoryEvent{
	public string Header;
	public string Date;
	public string Description;

	public HistoryEvent(string h){
		Header = h;
		Date = StrategicClock.GetDate ();
		Description = "";
	}

	public HistoryEvent(string h, string d){
		Header = h;
		Date = StrategicClock.GetDate ();
		Description = d;
	}
}

//Data container for ships, stored in fleet and used to instantiate the actual monos and gameobjects.
public class StrategicShip {

	public FAC Faction;
	public Empire Emp;

	public string ShipName;
	public Character Captain;
	public Character Executive;

	public List<Character> CharactersAboard = new List<Character>();

	public List<HistoryEvent> HistoryEvents = new List<HistoryEvent>();
	public void AddHistory(string head, string desc = ""){
		HistoryEvents.Add (new HistoryEvent (head, desc));
	}


	public ShipDesign DesignClass;

	public List<ShipComponents> Components = new List<ShipComponents> ();
	public List<string>ComponentStatus = new List<string>();

	ShipPrefabTypes VisualPrefab;


	public float[,] Armor;
	public ArmorTypes ArmorType;


	public bool isDamaged = false;
	public float Thrust;
	public int Quarters;
	public int Crew;
	public int mCrew;
	public string CrewString;

	public int Mass;
	public bool isControllable;

	public Emissions emissions;

	void ChangeStats(){
		Quarters = 0;
		isControllable = false;
		Thrust = 0;
		Mass = (int)DesignClass.mass;
		isDamaged = false;
		foreach (ShipComponents c in Components) {
			if (!c.isDamaged ()) {
				Quarters += (int)c.GetQuarters ();
				if (c.isControl ()) {
					isControllable = true;
				}
				Thrust += c.GetThrust ();
			} else {
				isDamaged = true;
			}
		}
		CrewString = string.Format ("Crew: {0}/{1}", Crew, mCrew);
		UpdateComponentStatusStrings ();
	}

	void UpdateComponentStatusStrings(){
		ComponentStatus.Clear();
		foreach(ShipComponents c in Components){
			string s = string.Format("{0} Functional: {1}",c.Name, c.isDamaged());
			ComponentStatus.Add(s);
		}
	}


	public Fleet ParentFleet;


	public void Save(Ship s){
		ShipName = s.shipClass.ShipName;
		Captain = s.shipClass.Captain;
	}

	void Setup(ShipDesign template){
		DesignClass = template;
		foreach (ShipComponents c in template.Components) {
			ShipComponents copy = c.CloneProperties ();
			Components.Add (copy);
		}
		UpdateComponentStatusStrings ();
		SetupArmor (template.ArmorLength, template.ArmorLayers, (float)template.ArmorType);
		mCrew = template.CrewMin;
		Crew = mCrew;
		ChangeStats();
		emissions = new Emissions();
		ArmorType = template.ArmorType;
		ChangeStats ();
	}

	public void CreateShip(GameObject g){
		Ship s = g.AddComponent<Ship> ();
		s.shipClass.ImportDesign (DesignClass);

	}


	public StrategicShip(ShipDesign template, Empire emp){
		Setup (template);
		ShipName = NameManager.AssignName (this);
		Emp = emp;
		Emp.Ships.Add (this);
	}

	public StrategicShip(ShipDesign template, string name, Empire emp){
		Setup (template);
		Emp = emp;
		ShipName = name;	
		Emp.Ships.Add (this);
	}

	public void DestroyShip(){
		NameManager.RecycleName (this);
	}

	void SetupArmor(int c, int r, float armorStrength){
		Armor = new float[c, r];
		for (int x = 0; x < Armor.GetLength(0); x += 1) {
			for (int y = 0; y < Armor.GetLength(1); y += 1) {
				Armor [x, y] = armorStrength;
			}
		}
	}


}
