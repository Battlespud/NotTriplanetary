using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Linq;

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
	//	c.GetShipPosting() = this;
		CharactersAboard.Add (c);
		UpdateCommand ();
	}
	public void MoveCharacterFromThis(Character c){
		CharactersAboard.Remove (c);
		UpdateCommand ();
		/*
		if (c.shipPosting == this)
			c.shipPosting = null;
		if (Captain == c) {
			Captain = null;
		}
		if (Executive == c) {
			Executive = null;
		}
		*/
	}
	#endregion

	void UpdateCommand(){
		List<Character> OnBoard = Emp.GetCharactersAtLocation (this, OfficerRoles.Navy);
		if(OnBoard.Count > 0){
			if (Captain != null && Captain != OnBoard [0]) {
				Captain.StepDownCaptain (this);
				AddHistory("Captain Stands Down",string.Format ("\n{0}: {1} stands down as Captain.", StrategicClock.GetDate (), Captain.GetNameString()));

			}
			OnBoard [0].AppointCaptain (this);
		}
		if(OnBoard.Count > 1){
			if (Executive != null && Executive != OnBoard [1]) {
				Executive.StepDownXO (this);
				AddHistory("Exec Stands Down", string.Format ("\n{0}: {1} stands down as Exec.", StrategicClock.GetDate (), Executive.GetNameString()));

			}
			OnBoard [1].AppointXO (this);
		}
	}


	public FAC Faction;
	public Empire Emp;

	public string ShipName;
	public Character Captain;
	public Character Executive;

	public List<Character> CharactersAboard = new List<Character>();
	public List<ICargo> Cargo = new List<ICargo> ();

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
	public float MaxSpeed;

	public int Quarters;
	public int Crew;
	public int mCrew;
	public string CrewString;

	public float MaxCargo;
	public float CurrentCargo;

	public float MaxMaintParts;
	public float CurrMaintParts;

	public int Mass;
	public bool isControllable;

	public Emissions emissions;

	//Fuel
	public float MaxFuel;
	public float CurrFuel;

	public string GetFuelString(){
		return string.Format ("{0}/{1}", CurrFuel, MaxFuel);
	}

	public float GetFuelNeeded(){
		return MaxFuel - CurrFuel;
	}



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

	//Combat
	public int Shields = 0;
	int MaxDAC = 0;
	public Dictionary<int,ShipComponents> DAC = new Dictionary<int, ShipComponents> ();
	public Dictionary<ShipComponents,Range>DACRanges = new Dictionary<ShipComponents, Range>();

	public void TransferToFleet(Fleet f){
		if (f == ParentFleet)
			return;
		if (ParentFleet != null) {
			ParentFleet.RemoveShip (this);
			ParentFleet = null;
		}
		f.AddShip (this);
		AddHistory("Transferred to Fleet", string.Format ("\n{0}: {1} is attached to {2}", StrategicClock.GetDate (), ShipName, f.FleetName));
	}

	public string GetCaptainName(){
		if (Captain != null)
			return Captain.GetNameString ();
		return "None Assigned";
	}
	public string GetExecName(){
		if (Executive != null)
			return Executive.GetNameString ();
		return "None Assigned";
	}

	public void ChangeStats(bool SuppressFuelCheck = false){
		Quarters = 0;
		isControllable = false;
		Thrust = 0;
		CurrentCargo = 0;
		MaxCargo = 0;
		Crew = 0;
		mCrew = 0;
		Mass = (int)DesignClass.mass;
		MaxFuel = 0f;
		Shields = 0;
		isDamaged = false;
		foreach (ShipComponents c in Components) {
			mCrew += c.CrewRequired;
			Crew += c.CrewPresent;
			if (!c.isDestroyed ()) {
				Quarters += (int)c.GetQuarters ();
				if (c.isControl ()) {
					isControllable = true;
				}
				MaxMaintParts += c.GetMaxSpareParts ();
				CurrMaintParts += c.GetCurrentSpareParts ();
				MaxCargo += c.GetCargo ();
				MaxFuel += c.GetFuelMax ();

				if (c.Enabled) {
					Thrust += c.GetThrust ();
					if(ParentFleet != null)
						Shields += (int)(c.GetMaxShield () * ParentFleet.ShieldStrength);
				}
			} else {
				isDamaged = true;
			}
		}


		MaxSpeed = Thrust / ((float)Mass / 50f);

		CrewString = string.Format ("Crew: {0}/{1}", Crew, mCrew);
		UpdateComponentStatusStrings ();
		CalculateCargo(); //TODO Test this and make sure it works
		BuildArmorString(); //TODO Test this and make sure it works
	}

	void CheckFuel(){
		if (MaxFuel < CurrFuel)
			CurrFuel = MaxFuel;
		if (CurrFuel <= 0f) {
			foreach (ShipComponents c in Components) {
				if (c.GetFuelUse () > 0f)
					c.Enabled = false;
			}
			ChangeStats (true);
		} else {
			foreach (ShipComponents c in Components) {
				if (c.Category == CompCategory.ENGINE)
					c.Enabled = true;
			}
		}
	}


	public void UseMovementFuel(float speed){
	//	Debug.LogError ("Checking fuel. This must be followed by a notification of how much fuel was used, or else no engines were found");
			List<ShipComponents> Sorted = Components.OrderBy (x => x.GetFuelUse ()).ToList();
			foreach (ShipComponents c in Sorted) {
				if (c.Category == CompCategory.ENGINE) {

					float original = CurrFuel;
					float multiplier = 1f; //used if we're only using part of an engines full power to reduce fuel cost.
					if (speed < SpeedFromThrust (c.GetThrust())) {
						multiplier = speed / SpeedFromThrust (c.GetThrust());
					}
				speed -= SpeedFromThrust(c.GetThrust())*multiplier;
		//		CurrFuel -= c.GetFuelUse ()*multiplier / StrategicClock.strategicClock.GoTurnLength;
				Debug.LogError(ShipName + " has used " + (original - CurrFuel) + " fuel units");
				if (speed <= 0)
					break;
				}
			}
	}

	float SpeedFromThrust(float thrust){
		return thrust / (Mass / 50f);
	}

	public void UseShieldFuel(float Percent){
		foreach (ShipComponents c in Components) {
			if (c.Category == CompCategory.SHIELDS) {
				//we can multiply by percent if we want to make shields cost fuel proportional to how charged they are.
				CurrFuel -= c.GetFuelUse ();
			}
		}
		CheckFuel ();
	}

	public void Refuel(float amount){
		CurrFuel += amount;
		if (CurrFuel > MaxFuel)
			CurrFuel = MaxFuel;
		ChangeStats ();
		CheckFuel ();
	}

	public float DistributeFuel(float Amount){
		CurrFuel = 0f;
		CurrFuel = (MaxFuel > Amount) ? Amount : MaxFuel;
		return Amount - CurrFuel; 

	}

	void CalculateCargo(){
		foreach (ICargo c in Cargo) {
			CurrentCargo += c.GetSize ();
		}
		while (CurrentCargo > MaxCargo) {
			int i = random.Next (0, Cargo.Count);
			ICargo z = Cargo [i];
			Cargo.Remove (z);
			z.DestroyCargo ();
			CurrentCargo = 0;
			foreach (ICargo c in Cargo) {
			CurrentCargo += c.GetSize ();
			}
		}
	}

	bool UseMaintParts(float amount){
		ChangeStats ();
		if (CurrMaintParts >= amount) {
			foreach (ShipComponents c in Components) {
				if (amount > 0) {
					if (c.GetCurrentSpareParts() >= amount) {
						c.UseSpareParts (amount);
					} else {
						amount -= c.GetCurrentSpareParts ();
						c.UseSpareParts (c.GetCurrentSpareParts ());
					}
				} else {
					break;
				}
			}
			return true;
		}
		return false;
	}

	void LoadCargo(ICargo c){
		if (CurrentCargo + c.GetSize () <= MaxCargo) {
			if (c.Load ()) {
				Cargo.Add (c);
				c.SetLocation (this);
			}
		}
	}

	void UpdateComponentStatusStrings(){
		ComponentStatus.Clear();
		List<ShipComponents> Sorted = Components.OrderBy (x => x.Category).ThenBy (x => x.Name).ThenBy(x=>x.isDamaged()).ThenBy(x=>x.isDestroyed()).ToList();
		foreach(ShipComponents c in Sorted){
			string stat = "";
			string color = "";
			string colorEnd = "</color>";
			if (c.isDestroyed ()) {
				stat = "XX";
				color = "<color=red>";
			} else if (c.isDamaged ()) {
				stat = "OX";
				color = "<color=orange>";
			}
			else {
				stat = "OK";
				color = "<color=green>";
			}
			string s = string.Format("{0}{1}{2} | {3}{4}{5}",color,stat,colorEnd,color,c.Name,colorEnd);
			ComponentStatus.Add(s);
		}
	}


	public float GetFunctionality(){
		int total = 0;
		int damaged = 0;
		foreach (ShipComponents c in Components) {
			total += c.Mass;
			if (c.isDestroyed())
				damaged += c.Mass;
		}
		return (total - damaged) / total;
	}

	public Fleet ParentFleet;


		

	public void Save(Ship s){
		//ShipName = s.shipClass.ShipName;
	//	Captain = s.shipClass.Captain;
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
		ChangeStats();
		ArmorType = template.ArmorType;
		ChangeStats ();
		UpdateMaint ();
		AddHistory ("Launched", string.Format ("{0}: {1} is launched.", StrategicClock.GetDate (), ShipName));
		CommissionDate = StrategicClock.GetDate ();
	}

	void UpdateEmissions(){
		foreach (ShipComponents c in Components) {
			emissions.Clear ();
			if (c.Enabled)
				emissions.Add (c.emissions);
		}
	}

	public string CommissionDate;

	public void UpdateMaint(){ //Call after damage
		MaintDamage = Mass / 2000f;
		if (MaintDamage < 2f)
			MaintDamage = 2f;
		MaxParts = 0;
		CurrParts = 0;
		float MaintMass = 0;
		foreach (ShipComponents c in Components) {
			if (!c.isDestroyed()) {
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
		if (!c.isDestroyed ()) {
			if (!UseMaintParts (c.MaintReq)) {
				c.Damage ();
				AddHistory("<color=yellow>Maintenance Failure</color>", string.Format ("\n{0}: {1} experiences a maintenance failure with the {2}, repairs proved impossible with current supplies.", StrategicClock.GetDate (), ShipName,c.Name));
				EmpireLogEntry E = new EmpireLogEntry(LogCategories.MILITARY,3,Emp,"MAINTENANCE FAILURE",string.Format("{0} has experienced a maintenance failure.",ShipName));
				ChangeStats ();
			} else {
				AddHistory("<color=red>Maintenance Failure</color>", string.Format ("\n{0}: {1} experiences a maintenance failure with the {2}, repairs were made with maintenance supplies.", StrategicClock.GetDate (), ShipName,c.Name));
				EmpireLogEntry E = new EmpireLogEntry(LogCategories.MILITARY,4,Emp,"MAINTENANCE FAILURE",string.Format("{0} has experienced a maintenance failure. No damage reported.",ShipName),CharactersAboard,new List<StrategicShip>{this});
			}
		} else {
			if (counter >= .85* Components.Count) {
				DestroyShip ();
				ShipLog += "\n<color=red>---Loss Resultant from Catastrophic Maintenance Failures---</color>";
			}
			TakeInternalHit (damage, counter++);
		}
	}

	public void CreateShip(GameObject g){
		Ship s = g.AddComponent<Ship> ();
		s.shipClass.ImportDesign (DesignClass);

	}

	public void AssignOfficer(Character c, NavalCommanderRole role ){
		if (role == NavalCommanderRole.XO) {
			Executive = c;
							AddHistory("Exec Appointed", string.Format ("\n{0}: {1} is appointed as Executive Officer", StrategicClock.GetDate (), Executive.GetNameString()));
		}

		else if(role == NavalCommanderRole.CMD) {
			Captain = c;
							AddHistory("Captain Appointed", string.Format ("\n{0}: {1} is appointed as Commanding Officer.", StrategicClock.GetDate (), Captain.GetNameString()));
		}
	}


	public StrategicShip(ShipDesign template, Empire emp){
		Setup (template);
		ShipName = emp.GetName (this);
		Emp = emp;
		Faction = emp.Faction;
		Empire.AllLocations.Add (this);
		Emp.Ships.Add (this);
	}

	public StrategicShip(ShipDesign template, string name, Empire emp){
		Setup (template);
		Emp = emp;
		Faction = emp.Faction;
		ShipName = name;	
		Empire.AllLocations.Add (this);

		Emp.Ships.Add (this);
	}

	public void DestroyShip(){
	//	NameManager.RecycleName (this);
		//TODO
		AddHistory("Contact Lost",string.Format("\n{0}: <color=red>---Contact Lost---</color> ",StrategicClock.GetDate()));
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.MILITARY,2,Emp,"SHIP DESTROYED",string.Format("Contact has been lost with {0}.\nThe ships logs may contain more detailed information.",ShipName));
		Cargo.ForEach (x => {
			x.DestroyCargo();
		});
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

	void BuildArmorString(){
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
			}
			a.AppendLine();
		}
		ArmorString = a.ToString();
	}






	//Combat
	public void SetupCombat(){
		ChangeStats ();
	}

	public float Sturdiness = 0f;

	public void TakeDamage(List<Int2> Pattern, float Damage){
		if (Shields > Damage)
			Shields -= Mathf.CeilToInt(Damage);
		else {
			Damage -= Shields;
			Shields = 0;
			DamageArmor (Pattern, Damage);
		}
	}


	IEnumerator DamageArmor(List<Int2> pattern, float dam){
			if (dam >= Mass / 1000) {
				if (random.NextDouble () < dam / (Mass / (2 + 15 * (1 - Sturdiness)))) {
					Debug.Log ("Shock Damage");
					TakeComponentDamage (dam * random.NextFloat (.35f, .6f));
			}
		}
		int startX = random.Next (0, Armor.GetLength (0) - 1);
		int startY = 0;
		for(startY = Armor.GetLength(1)-1; startY >= 0; startY--){
			if (Armor [startX, startY] > 0f) {
				break;
			}
		}
		int HullBound = 0;
		float counter = 0;
		//adjust penetration profile by damage
		for(int i = 0; dam > 0; i++) { //TODO REFACTOR PLS PLS PLS ITS SO BAD
			if (i >= pattern.Count)
				i = 0;
			Int2 v = pattern[i]; 
			if (startY + v.y < HullBound) {
				if (dam >= 1f) {
					counter++;
					dam -= 1;
				} else {
					counter += dam;
					dam = 0f;
				}
			}
			if (startX + v.x < 0) {
				v.x += Armor.GetLength (0)-1; 
			}
			if (startX + v.x > Armor.GetLength(0)-1) {
				v.x -= Armor.GetLength (0)-1; 
			}
			if (startX + v.x >= 0 && startX + v.x < Armor.GetLength (0) && startY + v.y >= 0 && startY + v.y < Armor.GetLength (1)) {
				try {
					if(dam > Armor [startX + v.x, startY + v.y]){
						dam -= Armor [startX + v.x, startY + v.y];
						Armor [startX + v.x, startY + v.y] = 0f;
					}
					else{
						Armor [startX + v.x, startY + v.y] -= dam;
						dam = 0f;
					}
				} catch {
					Debug.Log ("Invalid armor coords");
				}
			}
		}
		yield return Ninja.JumpToUnity;
		Debug.Log ("Penetrating hits: " + counter);
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(StrategicClock.strategicClock,TakeComponentDamage(counter));
	}

	IEnumerator TakeComponentDamage(float amount){
		Debug.Log ("Component Damage");
		int attempts = 0;
		ShipComponents target;
		while (amount > 0) {
			target = RollDAC ();
			while (target.isDestroyed ()) {
				target = RollDAC ();
				attempts++;
				if (attempts > Components.Count*.4f) {
					DestroyShip ();
					break;
				}
			}
			if (amount >= target.GetHTK ()) {
				target.Damage ();
				amount -= target.GetHTK ();
			} else if (amount < target.GetHTK ()) {
				float chance = amount / target.GetHTK ();
				if (random.Next (0, 100)/100 < chance) {
					target.Damage ();
				} else {
					target.DestroySubComponent ();
				}

				amount = 0;
			}
		}
		yield return Ninja.JumpToUnity;
		ChangeStats ();
	}

	public void SetupDAC(){
		int curr = 0;
		foreach (ShipComponents c in Components) {
			int pCounter = 0;
			int start = curr;
			for (int i = 0; c.Mass > i; i += ShipDesign.Slot) {
				DAC.Add (curr, c);
				pCounter++;
				curr++;
			}
			int end = curr-1;
			DACRanges.Add(c,new Range(start,end));
		}
		MaxDAC = curr; //exclusive
	}

	public ShipComponents RollDAC(){
		int i = random.Next (0, MaxDAC);
		Debug.Log ("Rolled #" + i + " " + DAC[i].Name + " HTK: " + DAC[i].GetHTK());
		return DAC [i];
	}

}
