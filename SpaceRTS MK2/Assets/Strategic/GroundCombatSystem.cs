using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battlefield{
	public static List<Battlefield> AllBattlefields = new List<Battlefield>();

	const int NumTurns = 5;

	public readonly ILocation Location; //The parent location. IE a planet or whatever
	public string BattlefieldName = "";

	public List<GroundUnit> Attackers = new List<GroundUnit>();
	public List<GroundUnit> Defenders = new List<GroundUnit>();

	public bool Active = false;

	public Battlefield(ILocation loc)
	{
		Location = loc;
		BattlefieldName = loc.GetLocationName ();
		StrategicClock.PhaseChange.AddListener(PhaseManager);
		AllBattlefields.Add(this);
	}


void PhaseManager(Phase p){
	switch (p) {
	case(Phase.ORDERS):
		{
			break;
		}
	case(Phase.GO):
		{
			CheckActivity ();
			ProgressCombat();
			break;
		}
	case (Phase.REVIEW):
		{
			break;
		}
	case (Phase.INTERRUPT):
		{
			break;
		}
	}
}

	void CheckActivity(){
		if (Attackers.Count > 0)
			Active = true;
	}

	void ProgressCombat()
		{
		if (!Active)
			return;
		bool isDefender = true;
		ApplyDamage(isDefender ? Attackers : Defenders, GetCombatStats(isDefender ? Defenders : Attackers));
		CheckResult ();
		}


	void CheckResult(){

	}

KeyValuePair<float,int> GetCombatStats(List<GroundUnit> force)
{
	float power = 0f;
	int count = 0;
	force.ForEach((unit) => 
		{
			if (unit.CombatEffective)
			{
				power += unit.GetCombatPower();
				count++;
			}
		});
	return new KeyValuePair<float, int>(power, count);
}

void ApplyDamage(List<GroundUnit> forceOne, KeyValuePair<float,int> forceTwo)
{
	KeyValuePair<float, int> fOneStats = GetCombatStats(forceOne);

	float fTwoDPT = forceTwo.Value / NumTurns;
	float dmg = fOneStats.Value <= 0 ? 
		fTwoDPT :
		fTwoDPT / fOneStats.Value;

	forceOne.ForEach((unit) => 
		{
			if (unit.CombatEffective)
			{
				unit.RecieveDamage(dmg);
			}
		});
	}
}

public class GroundUnit : ILocation, ICargo{
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
	#region Cargo
	public string GetCargoName(){
		return UnitName;
	}
	public float GetSize(){
		return (float)Size;
	}
	public object GetCargo(){
		return (object)this;
	}
	public System.Type GetCargoType(){
		return this.GetType();
	}
	public void SetLocation(ILocation Loc){
		Location = Loc;
	}
	public void DestroyCargo (){
		DestroyUnit ();
	}
	public bool Load (){
		return true;
	}
	#endregion

	//used for cargo
	public int Size = 1;

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
		UpdateStatus();
	}

	void UpdateStatus(){
		if (CombatEffective && NumberTroops < MaxNumberTroops / 2) {
			if (Commander != null) {
				if (rand.Next (-25, 25 + (1 - NumberTroops / MaxNumberTroops) * 100) > Commander.GetDiscipline () + Commander.GetCourage ()) {
			//		CombatEffective = false;
				}
			} else {
				if(rand.Next (-25, 25 + (1 - NumberTroops / MaxNumberTroops) * 100) > rand.Next(-35,40)){
				//	CombatEffective = false;
				}
			}
		}
	}

	public void DestroyUnit(){
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.MILITARY,3,empire,"Unit Lost",string.Format("<color=orange>{0}</color> has been <color=red>Destroyed</color at <color=blue>{1}</color>.",UnitName,Location.GetLocationName()),new List<Character>(){Commander});
		if (Commander != null) {
			Commander.AddHistory (string.Format("<color=orange>{0}</color> was <color=red>Destroyed</color at <color=blue>{1}</color> under the command of {2}.",UnitName,Location.GetLocationName(),Commander.GetNameString(true)));
			Commander.MoveTo (Location);
			CleanseReferences (this);

		}
	}

	static void CleanseReferences(GroundUnit G){
		G.empire.GroundUnits.Remove (G);
		foreach (Battlefield B in Battlefield.AllBattlefields) {
			if (B.Attackers.Contains (G))
				B.Attackers.Remove (G);
			if (B.Defenders.Contains(G))
				B.Defenders.Remove(G);
		}
	}

	public GroundUnit(Empire e, int num, float h, float cr, ILocation location){
		empire = e;
		Location = location;
		empire.GroundUnits.Add (this);
		UnitName = empire.GetName (this);
		MaxNumberTroops = num;
		NumberTroops = MaxNumberTroops;
		HealthPerTroop = h;
		CombatRating = cr;
		Log.Add(new HistoryEvent(string.Format("<Color=white>{0}</color> is brought into service",UnitName)));
	}

}