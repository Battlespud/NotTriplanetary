using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

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
public class StrategicShip : ILocation{
	
	public static System.Random random = new System.Random ();

	#region ILocation
	public string GetLocationName(){
		return ShipName;
	}
	public object GetLocation(){
		return (object)this;
	}
	public System.Type GetLocType(){
		return this.GetType ();
	}
	public void MoveCharacterToThis(Character c){
		c.shipPosting = this;
	}
	public void MoveCharacterFromThis(Character c){
		if (c.shipPosting == this)
			c.shipPosting = null;
		if (Captain == c) {
			Captain = null;
		}
		if (Executive == c) {
			Executive = null;
		}
	}


	#endregion

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

	public string ShipLog = "";

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

	//Maint
	public int MaxParts;
	public int CurrParts;
	public const int TurnsPerMaintClockTick = 5;
	float MaintDamage = 2f;
	public float BaseFailRate;
	public float EffectiveFailRate;
	public float MaintClock = 0f;
	public float OverhaulMulti = 3f; //1 turn in dock = 3 turns deployed
	public float MaintLife;
	public bool InDrydock = true;
	public bool IsDeployed = false;

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
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(StrategicClock.strategicClock,BuildArmorString()); //TODO Test this and make sure it works
	}

	void UpdateComponentStatusStrings(){
		ComponentStatus.Clear();
		foreach(ShipComponents c in Components){
			string s = string.Format("{0} Functional: {1}",c.Name, c.isDamaged());
			ComponentStatus.Add(s);
		}
	}


	public Fleet ParentFleet;

	public void AssignFleet(Fleet f){
		ShipLog += string.Format ("{0}: {1} is attached to {2}", StrategicClock.GetDate (), ShipName, f.FleetName);
		ParentFleet = f;
	}
		

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
		UpdateMaint ();
		ShipLog += string.Format ("{0}: {1} is launched.", StrategicClock.GetDate (), ShipName);
	}



	public void UpdateMaint(){ //Call after damage
		MaintDamage = Mass / 2000f;
		if (MaintDamage < 2f)
			MaintDamage = 2f;
		MaxParts = 0;
		CurrParts = 0;
		float MaintMass = 0;
		foreach (ShipComponents c in Components) {
			if (!c.isDamaged()) {
				MaintMass += c.getMaintMass ();
				MaxParts += (int) c.GetMaxSpareParts();
				CurrParts += (int) c.GetCurrentSpareParts();
			}
		}
		float percent = (MaintMass / Mass)*100f;
		BaseFailRate = (Mass / 175  * (2 / (percent))) + MaintClock*(Mass / 145  * (4 / (percent))); //Chance of breakdown during 20 turn deployment
		if (percent == 0f) 
			BaseFailRate = Mass / 5 + (Mass/5)*MaintClock;
		EffectiveFailRate = BaseFailRate * (TimeBetweenRolls / 20);
	}

	public bool UseMaintParts(float amount){
		if (CurrParts >= amount) {
			foreach (ShipComponents c in Components) {
				if (amount > c.GetCurrentSpareParts ()) {
					amount -= (int)c.GetCurrentSpareParts ();
					c.SetSpareParts(0f);
				} else {
					c.SetSpareParts(c.GetCurrentSpareParts() - amount);
					amount = 0;
				}
				if (amount <= 0)
					break;
			}
			return true;
		}
		return false;
	}

	float TimeSinceLastRoll = 0f;
	public const float TimeBetweenRolls = 5; //in turns
	public void RollMaint(){
		TimeSinceLastRoll += 1;
		if (TimeSinceLastRoll > TimeBetweenRolls - .1*MaintClock) {
			TimeSinceLastRoll = 0f;
			if (random.NextFloat (0f, 1f) < EffectiveFailRate)
				TakeInternalHit (MaintDamage);
		}
	}

	public void TakeInternalHit(float damage, int counter = 0){ //skips armor
		ShipComponents c = Components [random.Next (0, Components.Count)];
		if (!c.isDamaged ()) {
			if (!UseMaintParts (c.MaintReq)) {
				c.Damage ();
				ShipLog += string.Format ("{0}: {1} experiences a maintenance failure with the {2}, repairs proved impossible with current supplies.", StrategicClock.GetDate (), ShipName,c.Name);
				ChangeStats ();
			} else {
				ShipLog += string.Format ("{0}: {1} experiences a maintenance failure with the {2}, repairs were made with maintenance supplies.", StrategicClock.GetDate (), ShipName,c.Name);
			}
		} else {
			if (counter > 6)
				DestroyShip ();
			TakeInternalHit (damage, counter++);
		}
	}

	public void CreateShip(GameObject g){
		Ship s = g.AddComponent<Ship> ();
		s.shipClass.ImportDesign (DesignClass);

	}

	public void AssignOfficer(Character c, NavalCommanderRole role ){
		if (role == NavalCommanderRole.XO) {
			if (Executive != null)
				Executive.Unassign ();
			Executive = c;
			ShipLog += string.Format ("{0}: {1} is appointed as Executive Officer", StrategicClock.GetDate (), Executive.GetNameString());
		}

		else if(role == NavalCommanderRole.CMD) {
			if (Captain != null)
				Captain.Unassign ();
			Captain = c;
			ShipLog += string.Format ("{0}: {1} is appointed as Commanding Officer.", StrategicClock.GetDate (), Executive.GetNameString());
		}
	}


	public StrategicShip(ShipDesign template, Empire emp){
		Setup (template);
		ShipName = NameManager.AssignName (this);
		Emp = emp;
		Faction = emp.Faction;

		Emp.Ships.Add (this);
	}

	public StrategicShip(ShipDesign template, string name, Empire emp){
		Setup (template);
		Emp = emp;
		Faction = emp.Faction;
		ShipName = name;	
		Emp.Ships.Add (this);
	}

	public void DestroyShip(){
	//	NameManager.RecycleName (this);
		//TODO
		ShipLog += string.Format("\n{0}: {1} is lost.",StrategicClock.GetDate(),ShipName);
		Emp.Ships.Remove(this);
		if(ParentFleet)
			ParentFleet.Ships.Remove (this);
		if (Captain != null)
			Captain.Die ();
		if (Executive != null)
			Executive.Die ();
		foreach(Character c in CharactersAboard){
			c.Die ();
		}
	}



	void SetupArmor(int c, int r, float armorStrength){
		Armor = new float[c, r];
		for (int x = 0; x < Armor.GetLength(0); x += 1) {
			for (int y = 0; y < Armor.GetLength(1); y += 1) {
				Armor [x, y] = armorStrength;
			}
		}
	}

	public string ArmorString;

	IEnumerator BuildArmorString(){
		StringBuilder a = new StringBuilder();
		for (int y = 0; y < Armor.GetLength (1); y++) {
			for (int x = 0; x < Armor.GetLength (0); x++) {
				if (Armor [x, y] == (float)ArmorType) {
					a.Append( "<color=green>□</color>");
				}
				else if (Armor[x,y] > 0f && Armor[x,y] < (float)ArmorType) {
					a.Append( "<color=yellow>□</color>");
				}
				else if (Armor[x,y] <= 0f) {
					a.Append( "<color=red>□</color>");
				}
				//	float l =   Armor [x, y]/MaxArmor  ;
				//	Color c = new Color (2.0f * l, 2.0f * (1 - l), 0);
				//	a.Append( "<color=" + ColorUtility.ToHtmlStringRGBA(c) + ">□</color>");


			}
			a.AppendLine();
		}
		ArmorString = a.ToString();
		yield return Ninja.JumpToUnity;
	}

}
