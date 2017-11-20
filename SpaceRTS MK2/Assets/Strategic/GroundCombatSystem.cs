using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCombat{
	public static List<GroundCombat>AllGroundCombats = new List<GroundCombat>();

	public GroundCombat(){
		AllGroundCombats.Add (this);
	}

	void End(){
		AllGroundCombats.Remove (this);
	}
}

public class GroundUnit:ILocation{
	static System.Random rand = new System.Random();

	#region Location
	public string GetLocationName(){
		return UnitName;
	}
	public object GetLocation(){
		return (object)this;
	}
	public System.Type GetLocType(){
		return this.GetType ();
	}
	public void MoveCharacterToThis(Character c){
		Commander = empire.GetCharactersAtLocation (this, OfficerRoles.Army) [0];
	}
	public void MoveCharacterFromThis(Character c){
		if (c == Commander) {
			c.StepDownCommander (this);
		}
	}
	#endregion

	public bool InCombat = false;
	public Empire empire;

	public string UnitName = ""; //makes testing alot easier to just have this initialized

	public Character Commander;
	public ILocation Location;

	public bool CombatEffective = true;

	public int NumberTroops;
	public readonly int MaxNumberTroops;

	public readonly float HealthPerTroop;
	public readonly float CombatRating;

	public void AppointCommander(Character c){
		if (Commander != null) {
			Commander.StepDownCommander (this);
			Commander.MoveTo (Location);
		}
		c.AppointCommander (this);
		Commander = c;
	}

	//fluff
	public List<HistoryEvent> Log = new List<HistoryEvent>();

	public float GetCombatPower(){
		return (NumberTroops / MaxNumberTroops) * CombatRating;
	}

	public void RecieveDamage(float f){
		if (rand.NextFloat (0, 1) > (NumberTroops*1.25 + 1) / MaxNumberTroops) {
			Commander.InjureGroundCombat(Location,this,f/(NumberTroops+1));
		}
		NumberTroops -= (int)System.Math.Floor ((f / HealthPerTroop));
		if (NumberTroops <= 0) {
			float leftovers = (0 - NumberTroops) * HealthPerTroop;
			Commander.InjureGroundCombat (Location, this, leftovers);
			NumberTroops = 0;
		} 

	}

	void UpdateStatus(){
		if (CombatEffective && NumberTroops < MaxNumberTroops / 2) {
			if (Commander != null) {
				if (rand.Next (-25, 25 + (1 - NumberTroops / MaxNumberTroops) * 100) > Commander.GetDiscipline () + Commander.GetCourage ()) {
					CombatEffective = false;
				}
			} else {
				if(rand.Next (-25, 25 + (1 - NumberTroops / MaxNumberTroops) * 100) > rand.Next(-35,40)){
					CombatEffective = false;
				}
			}
		}
	}

	public GroundUnit(Empire e, int num, float h, float cr){
		empire = e;
			empire.GroundUnits.Add (this);
		UnitName = empire.GetName (this);
		MaxNumberTroops = num;
		NumberTroops = MaxNumberTroops;
		HealthPerTroop = h;
		CombatRating = cr;
		Log.Add(new HistoryEvent(string.Format("<Color=white>{0}</color> is brought into service",UnitName)));
	}



}