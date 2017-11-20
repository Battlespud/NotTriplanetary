using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public enum OfficerRoles{
	Research=3,
	Government=2,
	Army=1,
	Navy=0
}

public enum Sex{
	Female,
	Male
}

public struct Trait{
	public static List<Trait>Traits = new List<Trait>();
	static char delimiter = '+';
	public string Raw;
	public string Name;
	public string Description;
	public List<int> PersonalityModifiers;
	public string Summary;
	public Trait(string s, string d, string r, List<int> modifiers = null ){
		Name = s;
		Description = d;
		PersonalityModifiers = new List<int> ();
		PersonalityModifiers.AddRange (modifiers);
		string arraystring = "";
		foreach (int i in PersonalityModifiers) {
			arraystring += i.ToString() + ",";
		}
		Raw = r;
		Summary = string.Format ("{0}:({1}) {2}",Name,arraystring,Description);
		Traits.Add (this);

//		Debug.Log (Summary);
	}
	public static void Load(){
		//Load traits
		Traits.Clear();
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Traits/" ); 
		string[] traits = File.ReadAllLines(path+"Traits.txt");
		//string[] descr=File.ReadAllLines(path+"Descriptions.txt");
		int numTraits =0;

		for (int i = 0; i < traits.Length; i++) {
			List<int> mods = new List<int> ();
			List<string> cut = new List<string> ();
			cut.AddRange(traits [i].Split (delimiter));
			//Debug.Log (cut.Count);
			string traitName = cut [0];
			string modsString = cut [1];
			modsString = modsString.Replace ("(", "");
			modsString =modsString.Replace (")", "");
			modsString =modsString.Replace ("{", "");
			modsString =modsString.Replace ("}", "");
			modsString =modsString.Replace (" ", "");
			char modsDelim = ',';
			foreach (string s in modsString.Split (modsDelim)) {
				mods.Add (int.Parse (s));
			}
			string desc = cut [2];

			Trait t = new Trait (traitName, desc,traits[i], mods);
			numTraits = i + 1;
		}
//		Debug.Log("Loaded " + numTraits + " traits.");
	}
}

public class Medal{
	public static Dictionary<int, Sprite> MedalImages = new Dictionary<int, Sprite> ();
	public string Name;
	public string Description;
	public int Points = 50;
	public string DateDesigned;
	public int ImageIndex;
	public static List<Medal> DesignedMedals = new List<Medal> ();

	public Medal(string na,string desc,int p, int i ){
		Name = na;
		Description = desc;
		Points = p;
		DateDesigned = StrategicClock.GetDate ();
		ImageIndex = i;
		DesignedMedals.Add (this);
	}
	public Sprite GetImage(){
		return MedalImages [ImageIndex];
	}

	static Medal(){
		Medal Pioneer = new Medal ("Pioneer's Medallion", "An award commemorating this officer's role in early exploration efforts on the stellar frontier.", 50, 0);
	
	}
}

public enum NavalCommanderRole{
	XO=2,
	CMD=1,
	NONE=0
}

public class Character {
	public static string[] PersonalityAspectsStrings = new string[]{"Ambition","Courage","Extraversion","Intelligence","Discipline","Luck"};

	public static System.Random rnd = new System.Random();

	static StrategicClock clock = StrategicClock.strategicClock;

	static int NextID = -1;
	public int GetNextID(){
		NextID++;
		return NextID;

	}
	public int ID;
	public OfficerRoles Role = OfficerRoles.Navy;
	public NavalCommanderRole NavalRole;
	public int[] TimeInRole = new int[]{0,0,0};																						//Why doesnt this initialization work?
	public static Dictionary<OfficerRoles,List<string>> JobTitlesDictLong = new Dictionary<OfficerRoles, List<string>> (); //{ {OfficerRoles.Navy,NavalRankNames}, {OfficerRoles.Army, ArmyRankNames}, {OfficerRoles.Gov, GovRankNames}, {OfficerRoles.Research, ResearchRankNames }};
	public static Dictionary<OfficerRoles,List<string>> JobTitlesDictShort = new Dictionary<OfficerRoles, List<string>> ();
	public static List<string>NavalRankNames = new List<string>(){"Ensign", "Lieutenant","Lt. Commander","Commander","Captain","List Captain","Commodore", "Rear Admiral", "Vice Admiral", "Admiral", "Admiral of the Fleet"}; //add more
	public static List<string>ResearchRankNames = new List<string>(){"Research Assistant","Associate Researcher","Researcher","Senior Researcher","Associate Director","Director", "Secretary of Science"};
	public static List<string>GovRankNames = new List<string>(){"Junior Clerk","Clerk","Senior Clerk","Manager","Senior Manager","Junior Executive", "Executive", "Undersecretary", "Secretary of Government"};
	public static List<string>ArmyRankNames = new List<string>(){"Second Lieutenant","Lieutenant","Captain","Major","Lt. Colonel","Colonel", "Brigadier", "Major General", "General"};

	public static List<string>NavalRankNamesS = new List<string>(){"En.", "Lt.","Lt. Cmdr.","Cmdr.","Cpt.","L. Cptn.","Como.", "R. Adm.", "V. Adm.", "Adm.", "F. Adm."}; //add more
	public static List<string>ResearchRankNamesS = new List<string>(){"R.A.","Assoc.","R.","S.R.","A.Dir","Dir.", "Sec. Sci."};
	public static List<string>GovRankNamesS = new List<string>(){"J. Clk.","Clk.","S. Clk.","Man.","S. Man.","J. Exec.", "Exec.", "USec.", "SecGov."};
	public static List<string>ArmyRankNamesS = new List<string>(){"2nd. LT.","LT.","CPT.","MJR.","LT. COL.","COL.", "BRIG.", "MJR. GEN.", "GEN."};

	public static List<string>RolesAbbrev = new List<string>(){"SCI","GOV","MA","FL"};

	public static List<string>NavalComanderRoleNames = new List<string>(){"Executive Officer", "Commanding Officer"};
	public int PromotionPoints;
	public List<Medal> Medals = new List<Medal>();
	public List<Trait> Traits = new List<Trait> ();

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

	//im so sorry u exist
	 static Character(){
		JobTitlesDictLong.Add (OfficerRoles.Navy, NavalRankNames);
		JobTitlesDictLong.Add (OfficerRoles.Army, ArmyRankNames);
		JobTitlesDictLong.Add (OfficerRoles.Research, ResearchRankNames);
		JobTitlesDictLong.Add (OfficerRoles.Government, GovRankNames);

		JobTitlesDictShort.Add (OfficerRoles.Navy, NavalRankNamesS);
		JobTitlesDictShort.Add (OfficerRoles.Army, ArmyRankNamesS);
		JobTitlesDictShort.Add (OfficerRoles.Research, ResearchRankNamesS);
		JobTitlesDictShort.Add (OfficerRoles.Government, GovRankNamesS);
	}

	public string CharName;
	public int Age;
	public Sex sex;
	public StrategicShip shipPosting;
	public Empire empire;
	public int Rank;
	public int HP = 100;
	public ILocation Location;

	public Sprite Portrait;

	public List<int> PersonalityAspects = new List<int>(){0,0,0,0,0,0};


	//extra floof
	public string CommissionDate;

	#region PersonalityAspectGetters
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
	#endregion

	public bool Noble;
	public int NobleRank = 0;

	StringBuilder sb;

	public string History;

	public string GetNameString( bool isShortened = false, bool NobleOnly = false){
		if(NobleOnly)
			return string.Format ("{0} {1}", GetNobleTitle(), CharName);
		
		if (isShortened) 
			return string.Format ("{0} {1}", GetJobTitle(false), CharName);
		
		return string.Format ("{0} {1}", GetJobTitle(), CharName);
	}

	public string GetNobleTitle(){
		if(Noble)
			return  empire.Gov.NobleRanks[sex][NobleRank];
		if (empire.Gov.HasNobleClass)
			return "Commoner";
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

	public void UpdateTimeInRole(){
		if (NavalRole != NavalCommanderRole.NONE) {
			TimeInRole [(int)NavalRole]++;
			PromotionPoints += TimeInRole [(int)NavalRole] * (int)NavalRole / 4;
		}
	}

	public void AddHistory(string s){
		sb = new StringBuilder ();
		sb.Append(History);
		sb.AppendLine (s);
		History = sb.ToString ();
	}

	public void ModHP(int i){
		HP += i;
		if (HP <= 0) {
			Die ();
			HP = 0;
		}
	}

	public void MoveTo(ILocation loc){
		if (Location == loc)
			return;
		string FormerLoc = "NULL";
		if (Location != null) {
			Location.MoveCharacterFromThis (this);
			FormerLoc = Location.GetLocationName ();
		}
		Location = loc;
		string st = string.Format("{0}: {1} <color=yellow>transfers</color> from <color=white>{2}</color> to <color=white>{3}</color>.",StrategicClock.GetDate(), GetNameString(true), FormerLoc, Location.GetLocationName());
		AddHistory (st);
		Location.MoveCharacterToThis (this);

	}

	public void JoinsUp(){
		string st = string.Format("{0}: {1} enlists at the rank of <color=yellow>{2}</color>.",StrategicClock.GetDate(), CharName, GetJobTitle());
		CommissionDate = StrategicClock.GetDate ();
		AddHistory (st);
		if(GetValidPortraits(this).Count > 1)
			Portrait = GetValidPortraits (this) [rnd.Next(0,GetValidPortraits (this).Count)];
		for (int d = 0; d < PersonalityAspects.Count; d++) {
			PersonalityAspects[d] += rnd.Next (-50, 50);
		}
	}

	public void AppointCaptain(StrategicShip s){
		if (s.Captain == this)
			return;
		empire.Unassigned.Remove (this);
		MoveTo (s);
		shipPosting = s;
		s.AssignOfficer (this, NavalCommanderRole.CMD);
		NavalRole = NavalCommanderRole.CMD;
		string st = string.Format("{0}: {1} is placed in <color=green>appointed</color> <color=cyan>Captain</color> of <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ShipName);
		AddHistory (st);
	}

	public void AppointXO(StrategicShip s){
		if (s.Executive == this)
			return;
		empire.Unassigned.Remove (this);
		MoveTo (s);
		shipPosting = s;
		s.AssignOfficer (this, NavalCommanderRole.XO);
		NavalRole = NavalCommanderRole.XO;
		string st = string.Format("{0}: {1} is <color=green>appointed</color> <color=cyan>Executive Officer</color> of <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ShipName);
		AddHistory (st);
	}
	public void StepDownCaptain(StrategicShip s){
		empire.Unassigned.Add (this);
		MoveTo (s);
	//	s.AssignOfficer (this, NavalCommanderRole.CMD);
		NavalRole = NavalCommanderRole.NONE;
		string st = string.Format("{0}: {1} <color=orange>stands down</color> as <color=cyan>Captain</color> of <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ShipName);
		AddHistory (st);
	}

	public void StepDownXO(StrategicShip s){
		empire.Unassigned.Remove (this);
		MoveTo (s);
	//	s.AssignOfficer (this, NavalCommanderRole.XO);
		NavalRole = NavalCommanderRole.NONE;
		string st = string.Format("{0}: {1} <color=orange>stands down</color> as <color=cyan>Executive Officer</color> of <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ShipName);
		AddHistory (st);
	}
	public void AppointCommander(GroundUnit s){
		if(Location != s)
			MoveTo (s.Location);
		NavalRole = NavalCommanderRole.CMD;
		empire.Unassigned.Remove (this);
		shipPosting = null;
		Location = s.Location;
		string st = string.Format("{0}: {1} is <color=green>appointed</color> the <color=cyan>Commander</color> of <color=grey>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.UnitName);
		AddHistory (st);
	}

	public void StepDownCommander(GroundUnit s){
		NavalRole = NavalCommanderRole.NONE;
		empire.Unassigned.Add (this);
		shipPosting = null;
		Location = s.Location;
		string st = string.Format("{0}: {1} <color=orange>steps down</color> as the <color=cyan>Commander</color> of <color=grey>{2}</color?.",StrategicClock.GetDate(),GetNameString(), s.UnitName);
		AddHistory (st);
	}

	public void AppointSeniorOfficer(StrategicShipyard s){
		if(Location != s)
			MoveTo (s);
		s.SeniorOfficer = this;
		NavalRole = NavalCommanderRole.CMD;
		empire.Unassigned.Remove (this);
		shipPosting = null;
		Location = s;
		string st = string.Format("{0}: {1} is <color=green>appointed</color> the <color=cyan>Senior Officer</color> aboard <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), s.ShipYardName);
		AddHistory (st);
	}

	public void StepDownSeniorOfficer(StrategicShipyard s){
		if(Location != s)
			MoveTo (s);
		NavalRole = NavalCommanderRole.NONE;
		empire.Unassigned.Add (this);
		shipPosting = null;
		Location = s;
		string st = string.Format("{0}: {1} <color=orange>steps down</color> as the <color=cyan>Senior Officer</color> aboard {2}.",StrategicClock.GetDate(),GetNameString(), s.ShipYardName);
		AddHistory (st);
	}

	public void Unassign(){
		NavalRole = NavalCommanderRole.NONE;
		empire.Unassigned.Add (this);
		shipPosting = null;
		string st = string.Format("{0}: {1} has <color=red>no current assignment</color>.",StrategicClock.GetDate(),GetNameString(true));
		AddHistory (st);
	}

	public void AwardMedal(Medal m){
		Medals.Add (m);
		PromotionPoints += m.Points;
		string st = string.Format("{0}: {1} is <color=green>awarded</color> the <color=#D4AF37>{2}</color>.",StrategicClock.GetDate(),GetNameString(true), m.Name);
		AddHistory (st);
	}

	public void AwardMedal(Medal m, string reason){
		Medals.Add (m);
		PromotionPoints += m.Points;
		string st = string.Format("{0}: {1} is <color=green>awarded</color> the <color=#D4AF37>{2}</color> for '{3}'.",StrategicClock.GetDate(),GetNameString(true), m.Name, reason);
		AddHistory (st);
	}

	public void StartResearch(string TechName){
		string st = string.Format("{0}: {1} begins <color=blue>research</color> into {2}.",StrategicClock.GetDate(),GetNameString(true), TechName);
		AddHistory (st);
	}

	public void DidResearch(string TechName){
		string st = string.Format("{0}: {1} <color=green>completes</color> <color=blue>research</color> into {2}.",StrategicClock.GetDate(),GetNameString(true), TechName);
		AddHistory (st);
	}

	public void EntersGroundCombat(ILocation loc, GroundUnit gu){
		string st = string.Format("{0}: {1} enters ground combat in command of <color=magenta>{2}</color> on <color=white>{3}</color>.",StrategicClock.GetDate(),GetNameString(true), gu.UnitName,loc.GetLocationName());
		AddHistory (st);
	}
	public void InjureGroundCombat(ILocation loc, GroundUnit gu, float dam){
		string st = string.Format("{0}: {1} is <color=red>injured</color> {2}  while in command of <color=magenta>{3}</color> on <color=white>{4}</color>.",StrategicClock.GetDate(),GetNameString(true),HP +  "HP - " + (int)dam , gu.UnitName,loc.GetLocationName());
		ModHP ((int)dam * -1);
		AddHistory (st);
	}

	public void Promote(){
		if (Rank >= JobTitlesDictLong[Role].Count - 1)
			return;
		Rank++;
		string st = string.Format("{0}: {1} is <color=green>promoted</color> to the rank of <color=yellow>{2}</color>.",StrategicClock.GetDate(),CharName, GetJobTitle());
		AddHistory (st);
	}

	public void Demote(){
		if (Rank == 0)
			return;
		PromotionPoints -= 500;
		Rank--;
		string st = string.Format("{0}: {1} is <color=red>demoted</color> to the rank of <color=yellow>{2}</color>.",StrategicClock.GetDate(), CharName, GetJobTitle());
		AddHistory (st);
	}


	public void MakeNoble(int rank = 0){
		if (Noble)
			return;
		Noble = true;
		NobleRank = rank;
		string st = string.Format("{0}: {1} is made a <color=#C9BE62>{2}</color> of the realm.", StrategicClock.GetDate(),  GetNameString(true),GetNobleTitle());
		AddHistory (st);
	}

	public void PromoteNoble(){
		if (NobleRank == empire.Gov.NobleRanks.Count - 1)
			return;
		Noble = true;
		NobleRank++;
		string st = string.Format("{0}: {1} is raised to the station of <color=#C9BE62>{2}</color>.",StrategicClock.GetDate(),GetNameString(),GetNobleTitle());
		AddHistory (st);
	}

	public void Retire(bool Forced){
		string st = "";
		if (Forced) {
			 st = string.Format ("{0}: {1} was <color=red>dishonorably discharged</color> from the service.", StrategicClock.GetDate (), GetNameString ());
		
		} else {
			st = string.Format ("{0}: {1} {2} has <color=green>retired honorably</color> from the service.", StrategicClock.GetDate (),GetNobleTitle(), GetNameString ());
		}
		AddHistory (st);
		shipPosting = null;
		empire.Characters.Remove (this);
		empire.Unassigned.Remove (this);
		OutputDeath ();
	}

	public void AddTrait(Trait t){
		Traits.Add (t);
		for(int i = 0; i < 6; i++){
			PersonalityAspects [i] += t.PersonalityModifiers [i];
		}
	}

	public string GetPersonalitySummary(bool inline = true){
		StringBuilder sb = new StringBuilder ();
		if (inline) {
			for (int i = 0; i < 6; i++) {
				string c="";
				if (PersonalityAspects [i] > 74)
					c = "<color=green>";
				if (PersonalityAspects [i] > 24 && PersonalityAspects [i] < 75)
					c = "<color=cyan>";
				else if (PersonalityAspects [i] > -25 && PersonalityAspects [i] < 25)
					c = "<color=white>";
				else if (PersonalityAspects [i] > -75 && PersonalityAspects [i] <= -25)
					c = "<color=yellow>";
				else if (PersonalityAspects [i] <= -75)
					c = "<color=red>";
				sb.Append (c + PersonalityAspectsStrings [i]+"</color>     ");
			//	sb.Append (c + PersonalityAspectsStrings [i] + ": " + PersonalityAspects [i]+"</color>\t");
			}
		} else {
			for (int i = 0; i < 6; i++) {
				sb.AppendLine (PersonalityAspectsStrings [i] + ": " + PersonalityAspects [i]);
			}
		}
		return sb.ToString ().Trim();
	}

	public void Die(){
		string st = string.Format("{0}: {1} was <color=red>killed</color> in the destruction of the <color=white>{2}</color>.",StrategicClock.GetDate(),GetNameString(), shipPosting.ShipName);
		AddHistory (st);
		shipPosting = null;
		empire.Characters.Remove (this);
		empire.Unassigned.Remove (this);
		empire.Dead.Add (this);
		OutputDeath ();
	}
		
	public void Output(){
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Characters/" + CharName + ".txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			writer.WriteLine ("Name: " + CharName + "\t Age: " + Age);
			if(shipPosting != null)
			writer.WriteLine ("Posting: " + shipPosting.ShipName);
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
			if(shipPosting != null)
				writer.WriteLine ("Posting: " + shipPosting.ShipName);
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

	// Use this for initialization
	public Character(){

		Role = OfficerRoles.Navy;
		sex = (Sex)rnd.Next (0, 2);
		CharName = ThemeManager.GenerateCharName (sex); //todo
		Rank = 0;
		ID = GetNextID();
		JoinsUp ();
	}

	public Character(int i, OfficerRoles r){


		Role = r;

		Rank = i;
		ID = GetNextID();
		sex = (Sex)rnd.Next (0, 2);
		CharName = ThemeManager.GenerateCharName (sex); //todo
		JoinsUp ();
	}

	public Character(int i, Theme t, OfficerRoles r){

		Role = r;
		Rank = i;
		ID = GetNextID();
		sex = (Sex)rnd.Next (0, 2);

		CharName = ThemeManager.GenerateCharName(t,sex); //todo
		JoinsUp ();
	}
}
