using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
using System.Linq;

public enum OfficerRoles{
	//Imperial, Imperial characters are those that will always fall solely under the governments, and thus usually the players, control. They work for the good of the empire.
	Navy=0,
	Army,
	Government,
	Research,
	Intelligence,
	Police,


	//NonImperial, Nonimperial Characters do not work directly for the government, but are also generally beneficial or at least neutral. Civilians basically
	Child=100,
	Corporate, //works for a corporation from this faction
	Social, //socialite
	Merchant, //Independent or small time merchants, not from a corporation
	Celebrity,
	Scientist, //Independent scientist
	Politician,
	Media,
	Engineer,
	Noble,
	Retired,

	//Criminal, Never under player control and generally always bad.
	Terrorist = 200,
	Hacker,
	Rebel,
	Spy,
	Criminal, //major nonpolitical crime
	Cartel
}

public enum Sex{
	Female,
	Male
}
	
//Teams are groups of characters with a type.  Survey teams are deployed to uncharted worlds to find stuff for example.
public enum TeamTypes{
	Survey,
	Research,
	Diplomatic,
	Intelligence,
	Expedition //for dealing with primitives
}




//XO is second in command. CMD is first in command.  This decides what the XP they gain goes towards. This system will probably be removed in favor of an iterator later. As its a nightmare to manage.
public enum NavalCommanderRole{
	XO=2,
	CMD=1,
	NONE=0
}

public class MonthYear{
	public int Month;
	public int Year;

	public MonthYear(int m, int y){
		Month = m;
		Year = y;
	}
}

public class Character : ISearchable {
	#region statics
	public static string[] PersonalityAspectsStrings = new string[]{"Ambition","Courage","Extraversion","Intelligence","Discipline","Luck"};

	public static System.Random rnd = new System.Random();

	static StrategicClock clock = StrategicClock.strategicClock;

	static int NextID = -1;
	public static int GetNextID(){
		NextID++;
		return NextID;

	}

	static void PhaseManager(Phase p){
		switch (p) {
		case(Phase.ORDERS):
			{
				ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(StrategicClock.strategicClock,Turn());
				break;
			}
		case(Phase.GO):
			{
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



	public static Dictionary<int,Character> AllCharacters = new Dictionary<int, Character>(); // by ID
	public static Dictionary<Empire,List<Character>> CharactersByEmpire = new Dictionary<Empire, List<Character>>();

	public static Dictionary<OfficerRoles,List<string>> JobTitlesDictLong = new Dictionary<OfficerRoles, List<string>> (); 
	public static Dictionary<OfficerRoles,List<string>> JobTitlesDictShort = new Dictionary<OfficerRoles, List<string>> ();
	public static List<string>NavalRankNames = new List<string>(){"Ensign", "Lieutenant","Lt. Commander","Commander","Captain","List Captain","Commodore", "Rear Admiral", "Vice Admiral", "Admiral", "Admiral of the Fleet"}; //add more
	public static List<string>ResearchRankNames = new List<string>(){"Research Assistant","Associate Researcher","Researcher","Senior Researcher","Associate Director","Director", "Secretary of Science"};
	public static List<string>GovRankNames = new List<string>(){"Junior Clerk","Clerk","Senior Clerk","Manager","Senior Manager","Junior Executive", "Executive", "Undersecretary", "Secretary of Government"};
	public static List<string>ArmyRankNames = new List<string>(){"Second Lieutenant","Lieutenant","Captain","Major","Lt. Colonel","Colonel", "Brigadier", "Major General", "General"};

	public static List<string>NavalRankNamesS = new List<string>(){"En.", "Lt.","Lt. Cmdr.","Cmdr.","Cpt.","L. Cptn.","Como.", "R. Adm.", "V. Adm.", "Adm.", "F. Adm."}; //add more
	public static List<string>ResearchRankNamesS = new List<string>(){"R.A.","Assoc.","R.","S.R.","A.Dir","Dir.", "Sec. Sci."};
	public static List<string>GovRankNamesS = new List<string>(){"J. Clk.","Clk.","S. Clk.","Man.","S. Man.","J. Exec.", "Exec.", "USec.", "SecGov."};
	public static List<string>ArmyRankNamesS = new List<string>(){"2nd. LT.","LT.","CPT.","MJR.","LT. COL.","COL.", "BRIG.", "MGEN.", "GEN."};

	public static Dictionary<OfficerRoles,string>RolesAbbrev = new Dictionary<OfficerRoles, string>();

	public static List<string>NavalComanderRoleNames = new List<string>(){"Executive Officer", "Commanding Officer"};

	static List<string> NobleRanksM = new List<string> (){ "Esquire", "Viscount", "Count", "Marquess", "Prince", "Duke" };
	static List<string> NobleRanksF = new List<string> (){ "Dame", "Viscountess", "Countess", "Margrave", "Princess", "Duchess" };
	public static Dictionary<Sex,List<string>>NobleRanks = new Dictionary<Sex, List<string>>();


	//Images
	public static	List<Sprite>MNavy = new List<Sprite>();
	public static	List<Sprite>FNavy = new List<Sprite>();

	public static	List<Sprite>MGovernment = new List<Sprite>();
	public	static List<Sprite>FGovernment = new List<Sprite>();

	public static	List<Sprite>MArmy = new List<Sprite>();
	public static	List<Sprite>FArmy = new List<Sprite>();

	public static	List<Sprite>MResearch = new List<Sprite>();
	public static	List<Sprite>FResearch = new List<Sprite>();


	public static List<Sprite>GetValidPortraits(Character c){
		string FieldName = string.Format ("{0}{1}", c.sex.ToString () [0], c.Role.ToString ());
		return (List<Sprite>)typeof(Character).GetField(FieldName).GetValue(null);
	}
	#endregion

	
	
	
	#region Fields
	//Who this character is a member of
	public Empire empire;

	public string CharName {get {
			 return MiddleName != null ? FirstName + " " + MiddleName[0] + " " + LastName : FirstName + " " + LastName;}}

	public string FirstName;
	public string MiddleName;
	public string LastName;
	public int Age;
	public MonthYear Birthday; //Month, Year
	public Sex sex;

	public bool Alive = true;

	public int ID;
	public OfficerRoles Role = OfficerRoles.Navy;
	public NavalCommanderRole NavalRole;
	public int[] TimeInRole = new int[]{0,0,0};		//Number of turns spent in each role. 

	public int Rank;
	public int HP = 100;
	public ILocation Location;

	public string CommissionDate;

	//Full log of everything that has been done to or by this character
	public string History;

	public bool Noble;
	public int NobleRank = 0;

	public int PromotionPoints;

	public List<Medal> Medals = new List<Medal>();
	public List<Trait> Traits = new List<Trait> ();


	public string GetSearchableString()
	{
		return GetNameString() + Role.ToString() + dynasty.DynastyName;
	}
	
	#region Family
	public Dynasty dynasty;

	public Character Mother;
	public Character Father;

	public Character Spouse;

	public List<Character> Children = new List<Character>();

	public List<Character> GetChildrenBy(Character other){
		List<Character> relevant = new List<Character> ();
		Children.ForEach (x => {
			if(x.Mother == other || x.Father == other)
				relevant.Add(x);
		});
		return relevant;
	}

	public static void HaveChild(Character a, Character b){
		if (a.sex == b.sex)
			return;
		Character c;
		if (a.sex == Sex.Female) {
			c = new Character (a.empire);
			c.Father = b;
			c.dynasty = a.dynasty;
			c.LastName = b.LastName;
			c.Mother = a;
		} else {
			c = new Character (b.empire);
			c.Father = a;
			c.dynasty = b.dynasty;
			c.LastName = a.LastName;
			c.Mother = b;
		}
		a.Children.Add (c);	b.Children.Add (c);

		c.empire = c.Mother.empire;
		c.Role = OfficerRoles.Child;
		c.Birthday = new MonthYear (StrategicClock.month, StrategicClock.year);
		c.Rank = 0;
		c.ID = GetNextID ();


		c.sex = (Sex)rnd.Next (0, 2);
		if (rnd.NextFloat (0f, 1f) < .075f) {
			if (c.sex == Sex.Male)
				c.FirstName = c.Father.FirstName;
			else
				c.FirstName = c.Mother.FirstName;
		}


		c.AddRelationship( new RelationshipModifier(c.Mother,100,"Mother"));
		c.AddRelationship( new RelationshipModifier(c.Father,100,"Father"));

		string s ="Son";
		if (c.sex == Sex.Female)
			s = "Daughter";

		c.Mother.AddRelationship( new RelationshipModifier(c.Mother,100,s));
		c.Father.AddRelationship( new RelationshipModifier(c.Father,100,s));

	}

	#endregion



	#region Story
	public bool SwarmAgent;
	public Empire Loyalty;
	#endregion

	#endregion

	#region Job Specific Fields
		#region  Navy
	public StrategicShip GetShipPosting(){
		if(IsOnType<StrategicShip>())
			return (StrategicShip)Location.GetLocation ();
		return null;
	}
		#endregion
	#region  Navy

	#endregion

	#endregion

	#region Relationships
	Dictionary<Character,Relationship> Relationships = new Dictionary<Character, Relationship> ();

	public T GetRelationship<T>(Character Other){
		if (!Relationships.ContainsKey (Other)) {
			Relationships.Add (Other, new Relationship (Other));
		}
		T t = default(T);
		if (typeof(T) == typeof(int)) {
			return  (T)(object)Relationships [Other].GetRelationshipValue ();
		} else if (typeof(T) == typeof(Relationship)) {
			return (T)(object)Relationships [Other];
		} else if (typeof(T) == typeof(List<RelationshipModifier>)) {
			return (T)(object)Relationships [Other].GetList ();
		}
		Debug.Log (typeof(T).ToString() + " is unsupported.");
		return t;
	}

	public void AddRelationship(RelationshipModifier mod){
		if (!Relationships.ContainsKey (mod.Other)) {
			Relationships.Add (mod.Other, new Relationship (mod.Other));
		}
		Relationships [mod.Other].AddMod (mod);
	}
	#endregion

		#region Sprite
	Sprite portrait;
	public Sprite GetPortrait(){
		if(portrait == null && GetValidPortraits(this).Count > 1)
			portrait = GetValidPortraits (this) [rnd.Next(0,GetValidPortraits (this).Count)];
		return portrait;
	}
	#endregion

	#region Personality
	public List<int> PersonalityAspects = new List<int>(){0,0,0,0,0,0};

	public int GetAmbition(){
		return PersonalityAspects [0];
	}
	public int GetCourage(){
		return PersonalityAspects [1];
	}
	public int GetExtraversion(){
		return PersonalityAspects [2];
	}
	public int GetIntelligence(){
		return PersonalityAspects [3];
	}
	public int GetDiscipline(){
		return PersonalityAspects [4];
	}
	public int GetLuck(){
		return PersonalityAspects [5];
	}

	public string GetPersonalitySummary(bool inline = true){
		StringBuilder sb = new StringBuilder ();
		if (inline) {
			for (int i = 0; i < 6; i++) {
				string c="";
				if (PersonalityAspects [i] > 74)
					c = "<color=green>";
				if (PersonalityAspects [i] > 24 && PersonalityAspects [i] < 75)
					c = "<color=teal>";
				else if (PersonalityAspects [i] > -25 && PersonalityAspects [i] < 25)
					c = "<color=white>";
				else if (PersonalityAspects [i] > -75 && PersonalityAspects [i] <= -25)
					c = "<color=orange>";
				else if (PersonalityAspects [i] <= -75)
					c = "<color=red>";
				sb.Append (c + PersonalityAspectsStrings [i]+"</color>     ");
			}
		} else {
			for (int i = 0; i < 6; i++) {
				sb.AppendLine (PersonalityAspectsStrings [i] + ": " + PersonalityAspects [i]);
			}
		}
		return sb.ToString ().Trim();
	}
	#endregion

	#region Utilities
	StringBuilder sb;

	//Probably deprecated
	public void SetAssigned(bool MakeAssigned = true){
		switch (MakeAssigned) {
		case (true):{
				if (empire.Unassigned.Contains (this))
					empire.Unassigned.Remove (this);
				break;
			}
		case (false):{
				if (!empire.Unassigned.Contains (this))
					empire.Unassigned.Add (this);
				break;
			}
		}
	}
	#region NameGetters
	public string GetNameString( bool isShortened = false, bool NobleOnly = false){
		if(NobleOnly)
			return string.Format ("{0} {1}", GetNobleTitle(), CharName);
		
		if (isShortened) 
			return string.Format ("{0} {1}", GetJobTitle(false), CharName);
		
		return string.Format ("{0} {1}", GetJobTitle(), CharName);
	}

	public string GetNobleTitle(){
		if(Noble)
			return  NobleRanks[sex][NobleRank];
		return "Citizen";
	}

	public string GetJobTitle(bool Full = true){
		if (Full) {
			try{
			return JobTitlesDictLong [Role] [Rank];
			}
			catch{
				Debug.LogError ("Integer: " + Rank + " is invalid with Role: " + Role.ToString ());
			}
		}
		return JobTitlesDictShort [Role] [Rank];
	}
	#endregion

	public void UpdateTimeInRole(){
		if (NavalRole != NavalCommanderRole.NONE) {
			TimeInRole [(int)NavalRole]++;
			PromotionPoints += TimeInRole [(int)NavalRole] * (int)NavalRole / 4;
		}
	}


	public void ModHP(int i){
		HP += i;
		if (HP <= 0) {
			Die ();
			HP = 0;
		}
	}

	public bool IsOnType<T>(){
		return	Location.GetType () == typeof(T);
	}
	#endregion


	#region Logging
	public void AddHistory(string s){
		sb = new StringBuilder ();
		sb.Append(History);
		sb.AppendLine (s);
		sb.AppendLine("-----------------------------------------------"); 	

		History = sb.ToString ();
	}

	public void MoveTo(ILocation loc){
		if (Location == loc)
			return;
		string FormerLoc = "NULL";
		if (Location != null) {
			Location.MoveCharacterFromThis (this);
			FormerLoc = Location.GetLocationName ();
		}
		SetAssigned (false);
		Location = loc;
		string st = string.Format("{0}: <color=navy>{1}</color> <color=yellow>transfers</color> from <color=white>{2}</color> to <color=white>{3}</color>.",StrategicClock.GetDate(), GetNameString(true), FormerLoc, Location.GetLocationName());
		AddHistory (st);
		Location.MoveCharacterToThis (this);
	}
		
	public void JoinsUp(){
		string st = string.Format("{0}: <color=navy><color=navy>{1}</color></color> enlists at the rank of <color=yellow>{2}</color>.",StrategicClock.GetDate(), CharName, GetJobTitle());
		CommissionDate = StrategicClock.GetDate ();
		AddHistory (st);
		SetAssigned (false);
		for (int d = 0; d < PersonalityAspects.Count; d++) {
			PersonalityAspects[d] += rnd.Next (-50, 50);
		}
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.MILITARY,5,empire,"OFFICER RECIEVES COMMISSION",st,new List<Character>{this});

	}

	public void JoinsTeam(Team t){
		SetAssigned (true);
		string st = string.Format("{0}: <color=navy>{1}</color> join <color=magenta>{2}</color>.",StrategicClock.GetDate(), CharName, t.TeamName);
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.DEFAULT,5,empire,"OFFICER JOINS TEAM",st,new List<Character>{this});
		CommissionDate = StrategicClock.GetDate ();
		AddHistory (st);
	}

	public void AppointCaptain(StrategicShip s){
		if (s.Captain == this)
			return;
		SetAssigned (true);
		MoveTo (s);
	//	shipPosting = s;
		s.AssignOfficer (this, NavalCommanderRole.CMD);
		NavalRole = NavalCommanderRole.CMD;
		string st = string.Format("{0}: <color=navy>{1}</color> is <color=green>made</color> <color=cyan>Captain</color> of <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ShipName);
		AddHistory (st);
	}

	public void AppointXO(StrategicShip s){
		if (s.Executive == this)
			return;
		SetAssigned (true);
		MoveTo (s);
	//	shipPosting = s;
		s.AssignOfficer (this, NavalCommanderRole.XO);
		NavalRole = NavalCommanderRole.XO;
		string st = string.Format("{0}: <color=navy>{1}</color> is <color=green>appointed</color> <color=cyan>Executive Officer</color> of <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ShipName);
		AddHistory (st);
	}
	public void StepDownCaptain(StrategicShip s){
		SetAssigned (false);
		MoveTo (s);
	//	s.AssignOfficer (this, NavalCommanderRole.CMD);
		NavalRole = NavalCommanderRole.NONE;
		string st = string.Format("{0}: <color=navy>{1}</color> <color=orange>stands down</color> as <color=cyan>Captain</color> of <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ShipName);
		AddHistory (st);
	}

	public void StepDownXO(StrategicShip s){
		SetAssigned (false);
		MoveTo (s);
	//	s.AssignOfficer (this, NavalCommanderRole.XO);
		NavalRole = NavalCommanderRole.NONE;
		string st = string.Format("{0}: <color=navy>{1}</color> <color=orange>stands down</color> as <color=cyan>Executive Officer</color> of <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ShipName);
		AddHistory (st);
	}
	public void AppointCommander(GroundUnit s){
		if(Location != s)
			MoveTo (s.Location);
		NavalRole = NavalCommanderRole.CMD;
		SetAssigned (true);
	//	shipPosting = null;
		Location = s.Location;
		string st = string.Format("{0}: <color=navy>{1}</color> is <color=green>appointed</color> the <color=cyan>Commander</color> of <color=grey>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.UnitName);
		AddHistory (st);
	}

	public void StepDownCommander(GroundUnit s){
		NavalRole = NavalCommanderRole.NONE;
		SetAssigned (false);
	//	shipPosting = null;
		Location = s.Location;
		string st = string.Format("{0}: <color=navy>{1}</color> <color=orange>steps down</color> as the <color=cyan>Commander</color> of <color=grey>{2}</color?.",StrategicClock.GetDate(),GetNameString(), s.UnitName);
		AddHistory (st);
	}

	public void AppointSeniorOfficer(StrategicShipyard s){
		if(Location != s)
			MoveTo (s);
		s.SeniorOfficer = this;
		NavalRole = NavalCommanderRole.CMD;
		SetAssigned (true);
	//	shipPosting = null;
		Location = s;
		string st = string.Format("{0}: <color=navy>{1}</color> is <color=green>appointed</color> the <color=cyan>Senior Officer</color> aboard <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ShipYardName);
		AddHistory (st);
	}

	public void StepDownSeniorOfficer(StrategicShipyard s){
		if(Location != s)
			MoveTo (s);
		NavalRole = NavalCommanderRole.NONE;
		SetAssigned (false);
	//	shipPosting = null;
		Location = s;
		string st = string.Format("{0}: <color=navy>{1}</color> <color=orange>steps down</color> as the <color=cyan>Senior Officer</color> aboard <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ShipYardName);
		AddHistory (st);
	}

	public void AppointGovernor(Colony s){
		if(Location != s)
			MoveTo (s);
		s.Governor = this;
		NavalRole = NavalCommanderRole.CMD;
		SetAssigned (true);
//		shipPosting = null;
		Location = s;
		string st = string.Format("{0}: <color=navy>{1}</color> is <color=green>appointed</color> the <color=cyan>Governor</color> of <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ColonyName);
		AddHistory (st);
	}

	public void AppointPoliticalOffice(PoliticalOffice pol, bool appointedNotRemoved = true){
		SetAssigned (true);
		string st = appointedNotRemoved ? string.Format("{0}: <color=navy>{1}</color> is <color=green>appointed</color> the <color=cyan>Director</color> of the <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), pol.OfficeName)
			: string.Format("{0}: <color=navy>{1}</color> is <color=red>removed</color> as the <color=cyan>Director</color> of the <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), pol.OfficeName);
		AddHistory (st);
	}

	public void StepDownGovernor(Colony s){
		if(Location != s)
			MoveTo (s);
		NavalRole = NavalCommanderRole.NONE;
		SetAssigned (false);
	//	shipPosting = null;
		Location = s;
		string st = string.Format("{0}: <color=navy>{1}</color> <color=orange>steps down</color> as the <color=cyan>Governor</color> of <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ColonyName);
		AddHistory (st);
	}

	public void Unassign(){
		NavalRole = NavalCommanderRole.NONE;
		SetAssigned (false);
//		shipPosting = null;
		string st = string.Format("{0}: <color=navy>{1}</color> has <color=red>no current assignment</color>.",StrategicClock.GetDate(),GetNameString(true));
		AddHistory (st);
	}

	public void AwardMedal(Medal m){
		Medals.Add (m);
		PromotionPoints += m.Points;
		string st = string.Format("{0}: <color=navy>{1}</color> is <color=green>awarded</color> the <color=#D4AF37>{2}</color>.",StrategicClock.GetDate(),GetNameString(true), m.Name);
		AddHistory (st);
	}

	public void AwardMedal(Medal m, string reason){
		Medals.Add (m);
		PromotionPoints += m.Points;
		string st = string.Format("{0}: <color=navy>{1}</color> is <color=green>awarded</color> the <color=#D4AF37>{2}</color> for '{3}'.",StrategicClock.GetDate(),GetNameString(true), m.Name, reason);
		AddHistory (st);
	}

	public void StartResearch(string TechName){
		SetAssigned (true);
		string st = string.Format("{0}: <color=navy>{1}</color> begins <color=blue>research</color> into {2}.",StrategicClock.GetDate(),GetNameString(true), TechName);
		AddHistory (st);
	}

	public void DidResearch(string TechName){
		SetAssigned (false);
		string st = string.Format("{0}: <color=navy>{1}</color> <color=green>completes</color> <color=blue>research</color> into {2}.",StrategicClock.GetDate(),GetNameString(true), TechName);
		AddHistory (st);
	}

	public void EntersGroundCombat(ILocation loc, GroundUnit gu){
		string st = string.Format("{0}: <color=navy>{1}</color> enters ground combat in command of <color=magenta>{2}</color> on <color=white>{3}</color>.",StrategicClock.GetDate(),GetNameString(true), gu.UnitName,loc.GetLocationName());
		AddHistory (st);
	}
	public void InjureGroundCombat(ILocation loc, GroundUnit gu, float dam){
		string st = string.Format("{0}: <color=navy>{1}</color> is <color=red>injured</color> {2}  while in command of <color=magenta>{3}</color> on <color=white>{4}</color>.",StrategicClock.GetDate(),GetNameString(true),HP +  "HP - " + (int)dam , gu.UnitName,loc.GetLocationName());
		ModHP ((int)dam * -1);
		AddHistory (st);
	}

	public void Promote(){
		if (Rank >= JobTitlesDictLong[Role].Count - 1)
			return;
		Rank++;
		string st = string.Format("{0}: <color=navy>{1}</color> is <color=green>promoted</color> to the rank of <color=yellow>{2}</color>.",StrategicClock.GetDate(),CharName, GetJobTitle());
		AddHistory (st);
	}

	public void Demote(){
		if (Rank == 0)
			return;
		PromotionPoints -= 500;
		Rank--;
		string st = string.Format("{0}: <color=navy>{1}</color> is <color=red>demoted</color> to the rank of <color=yellow>{2}</color>.",StrategicClock.GetDate(), CharName, GetJobTitle());
		AddHistory (st);
	}


	public void MakeNoble(int rank = 0){
		if (Noble)
			return;
		Noble = true;
		NobleRank = rank;
		string st = string.Format("{0}: <color=navy>{1}</color> is made a <color=#C9BE62>{2}</color> of the realm.", StrategicClock.GetDate(),  GetNameString(true),GetNobleTitle());
		AddHistory (st);
	}

	public void PromoteNoble(){
		if (NobleRank == NobleRanks.Count - 1)
			return;
		Noble = true;
		NobleRank++;
		string st = string.Format("{0}: <color=navy>{1}</color> is raised to the station of <color=#C9BE62>{2}</color>.",StrategicClock.GetDate(),GetNameString(),GetNobleTitle());
		AddHistory (st);
	}

	public void Retire(bool Forced){
		string st = "";
		SetAssigned (true);
		if (Forced) {
			 st = string.Format ("{0}: <color=navy>{1}</color> was <color=red>dishonorably discharged</color> from the service.", StrategicClock.GetDate (), GetNameString ());
		
		} else {
			st = string.Format ("{0}: <color=navy>{1}</color> {2} has <color=green>retired honorably</color> from the service.", StrategicClock.GetDate (),GetNobleTitle(), GetNameString ());
		}
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.MILITARY,3,empire,"OFFICER RETIRES",st,new List<Character>{this});
		AddHistory (st);
	//	shipPosting = null
		Role = OfficerRoles.Retired;
	}

	public void AddTrait(Trait t){
		if (Traits.Contains (t))
			return;
		Traits.Add (t);
		for(int i = 0; i < 6; i++){
			PersonalityAspects [i] += t.PersonalityModifiers [i];
		}
	}

	#endregion

	#region Static Processing

	static IEnumerator Turn(){
		foreach (Character c in AllCharacters.Values) {
			CheckBirthday(c);
			foreach (ILocation Loc in Empire.AllLocations) {
				ProcessLocationEvents (Loc);
			}

		}
		yield return null;
	}

	static void CheckBirthday(Character c){
		if(c.Birthday == null)
			c.Birthday = new MonthYear(rnd.Next(0,StrategicClock.Months.Count), StrategicClock.year-c.Age);
		if (c.Birthday.Month == StrategicClock.month) {
				c.Age++;
				if (rnd.Next (0, 15) < 2)
					c.AddTrait (Trait.Traits[rnd.Next(0,c.Traits.Count)]);
				if (c.Age == 18) {
					c.Role = SystemRandomExtensions.RandomEnum<OfficerRoles>();
					if (c.Role == OfficerRoles.Child)
						c.Role = OfficerRoles.Navy;
					if ((int)c.Role < 100)
						c.JoinsUp ();
				}
				else if (c.Age > 65)
					c.Retire (false);
			}
	}



	static void ProcessLocationEvents(ILocation L){
		Dictionary<OfficerRoles,List<Character>> CharactersByRole = new Dictionary<OfficerRoles, List<Character>>();
		List<Character> CharactersAtLocation = GetCharactersAtLocation (L,CharactersByRole);
		
		CharactersAtLocation.ForEach (x => {
			switch (x.Role)
			{
				case OfficerRoles.Navy:
					break;
				case OfficerRoles.Army:
					break;
				case OfficerRoles.Government:
					break;
				case OfficerRoles.Research:
					break;
				case OfficerRoles.Intelligence:
					break;
				case OfficerRoles.Police:
				 if (CharactersByRole.ContainsKey(OfficerRoles.Criminal))
					{
						Character target = CharactersByRole[OfficerRoles.Criminal][rnd.Next(0, CharactersByRole[OfficerRoles.Criminal].Count)];
						if (rnd.Next(0, 100) < x.Rank + 6 - target.Rank)
						{
							Debug.Log(string.Format("{0} successfully apprehends {1} on {2}", x.GetNameString(), target.GetNameString(),
								L.GetLocationName()));
							x.PromotionPoints += (int) (.25 * target.PromotionPoints + 25);
						}
						else
						{
							Debug.Log(string.Format("{0} fails to apprehend {1} on {2}", x.GetNameString(), target.GetNameString(),L.GetLocationName()));
						}
					}
					break;
				case OfficerRoles.Child:
					if (x.Mother.Alive)
						x.MoveTo (x.Mother.Location);
					else if (x.Father.Alive)
						x.MoveTo (x.Father.Location);
					else
						x.Die ();
					break;
				case OfficerRoles.Corporate:
					break;
				case OfficerRoles.Social:
					break;
				case OfficerRoles.Merchant:
					break;
				case OfficerRoles.Celebrity:
					break;
				case OfficerRoles.Scientist:
					break;
				case OfficerRoles.Politician:
					break;
				case OfficerRoles.Media:
					break;
				case OfficerRoles.Engineer:
					break;
				case OfficerRoles.Noble:
					break;
				case OfficerRoles.Terrorist:
					break;
				case OfficerRoles.Hacker:
					break;
				case OfficerRoles.Rebel:
					break;
				case OfficerRoles.Spy:
					break;
				case OfficerRoles.Criminal:
					break;
				case OfficerRoles.Cartel:
					break;
				default:
					Debug.LogError("Unsupported Character Role.");
					throw new ArgumentOutOfRangeException();
			}
			});
	}

	static List<Character> GetCharactersAtLocation(ILocation loc, Dictionary<OfficerRoles,List<Character>> dict, OfficerRoles? rNullable = null, bool softReq = false){
		List<Character> Output = new List<Character> ();
		foreach (Character c in AllCharacters.Values) {
			if (c.Location == loc)
			{
				if (dict.ContainsKey(c.Role))
				{
					dict[c.Role].Add(c);
				}
				else
				{
					dict.Add(c.Role, new List<Character>{c});
				}
				Output.Add(c);
			}
		}
		if (rNullable != null && Output.Count > 0) {
			OfficerRoles r = rNullable.Value;
			Output = GetCharactersByTypeAndRank (r, Output);
		}
		if (rNullable == null) {
			Output = Output.OrderByDescending (x => x.Rank).ThenByDescending (x => x.Noble).ThenByDescending (x => x.NobleRank).ToList ();
		} else {
			Output = Output.OrderBy (x => x.Role).ThenByDescending (x => x.Rank).ThenByDescending (x => x.Noble).ThenByDescending (x => x.NobleRank).ToList ();
		}
		return Output;
	}

	public static List<Character>GetCharactersByTypeAndRank(OfficerRoles r, List<Character> Set, int rank = -1){
		List<Character> ch = new List<Character> ();
		List<Character> imthenulls = new List<Character> ();

		foreach (Character c in Set) {
			if (c != null) {
				if (c.Role == r && (c.Rank == rank || rank == -1))
						ch.Add (c);
			}
			else {
				imthenulls.Add (c);
			}
		}
		foreach (Character c in imthenulls) {
			//Do Nothing
		}
		ch = ch.OrderByDescending (x => x.Rank).ThenByDescending(x => x.Noble).ThenByDescending(x => x.NobleRank).ToList ();
		return ch;
	}
	#endregion


	public void Die(){
		string st = "";
		SetAssigned (true);
		if(GetShipPosting() != null)
			st = string.Format("{0}: <color=navy>{1}</color> was <color=red>killed</color> in the destruction of the <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), GetShipPosting().ShipName);
		else
			st = string.Format("{0}: <color=navy>{1}</color> was <color=red>killed</color> at.",StrategicClock.GetDate(),GetNameString(),Location.GetLocationName());
		AddHistory (st);
	//	shipPosting = null;
		Alive = false;
		OutputDeath ();
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.MILITARY,3,empire,"OFFICER DEATH",st,new List<Character>{this});

	}

	#region Output (Deprecated)

	public void Output(){
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Characters/" + CharName + ".txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			writer.WriteLine ("Name: " + CharName + "\t Age: " + Age);
			if(GetShipPosting() != null)
				writer.WriteLine ("Posting: " + GetShipPosting().ShipName);
			writer.Write (History + "\n\nAwards:\n");
			foreach (Medal m in Medals) {
				writer.WriteLine (m.Name);
			}
			writer.Write ("\n\nTraits:\n");
			foreach (Trait m in Traits) {
				writer.WriteLine (m.Name);
			}
				}
	}
	public void OutputDeath(){
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Characters/Fallen" + CharName + ".txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			writer.WriteLine ("Name: " + CharName + "\t Age: " + Age);
			if(GetShipPosting() != null)
				writer.WriteLine ("Posting: " + GetShipPosting().ShipName);
			writer.Write (History + "\n\nAwards:\n");
			foreach (Medal m in Medals) {
				writer.WriteLine (m.Name);
			}
			writer.Write ("\n\nTraits:\n");
			foreach (Trait m in Traits) {
				writer.WriteLine (m.Name);
			}
		}
	}
	#endregion

	#region Constructors
	// TODO Check which ones are up to date and which arent, then cut it down to a single one.

	//Only for use at game start
	public Character(int i, OfficerRoles r, Empire e, Theme t = null){
		empire = e;
		Role = r;
		Rank = i;
		ID = GetNextID();
		sex = (Sex)rnd.Next (0, 2);
		if(t!= null)
			ThemeManager.GenerateCharName(this,t); //todo
		else
			ThemeManager.GenerateCharName(this); //todo
		Register(this);
		JoinsUp ();
	}

	public Character (Empire e){
		empire = e;
		ID = GetNextID();
		Register(this);

	}
	#endregion

	public static void Register(Character c)
	{
		if(CharactersByEmpire.ContainsKey(c.empire))
			CharactersByEmpire[c.empire].Add(c);
		else
		{
			CharactersByEmpire.Add(c.empire,new List<Character>(){c});
		}
		AllCharacters.Add(c.ID,c);
	}
	
	//im so sorry u exist
	//Sets up our dictionaries
	static Character(){
		StrategicClock.PhaseChange.AddListener (PhaseManager);


		JobTitlesDictLong.Add (OfficerRoles.Navy, NavalRankNames);
		JobTitlesDictLong.Add (OfficerRoles.Army, ArmyRankNames);
		JobTitlesDictLong.Add (OfficerRoles.Research, ResearchRankNames);
		JobTitlesDictLong.Add (OfficerRoles.Government, GovRankNames);

		JobTitlesDictShort.Add (OfficerRoles.Navy, NavalRankNamesS);
		JobTitlesDictShort.Add (OfficerRoles.Army, ArmyRankNamesS);
		JobTitlesDictShort.Add (OfficerRoles.Research, ResearchRankNamesS);
		JobTitlesDictShort.Add (OfficerRoles.Government, GovRankNamesS);

		RolesAbbrev.Add (OfficerRoles.Navy, "OF");
		RolesAbbrev.Add (OfficerRoles.Army, "MN");
		RolesAbbrev.Add (OfficerRoles.Government, "GV");
		RolesAbbrev.Add (OfficerRoles.Research, "RD");

		NobleRanks.Add (Sex.Female, NobleRanksF);
		NobleRanks.Add (Sex.Male, NobleRanksM);
	}
}
