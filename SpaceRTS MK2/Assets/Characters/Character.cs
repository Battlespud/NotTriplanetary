using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public enum OfficerRoles{
	Research,
	Government,
	Army,
	Navy
}

public enum Sex{
	Female,
	Male
}

public struct Trait{
	public static List<Trait>Traits = new List<Trait>();
	static char delimiter = '+';

	public string Name;
	public string Description;
	public List<int> PersonalityModifiers;
	public string Summary;
	public Trait(string s, string d, List<int> modifiers = null){
		Name = s;
		Description = d;
		PersonalityModifiers = new List<int> ();
		PersonalityModifiers.AddRange (modifiers);
		string arraystring = "";
		foreach (int i in PersonalityModifiers) {
			arraystring += i.ToString() + ",";
		}
		Summary = string.Format ("{0}:({1}) {2}",Name,arraystring,Description);
		Traits.Add (this);

//		Debug.Log (Summary);
	}
	public static void Load(){
		//Load traits
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

			Trait t = new Trait (traitName, desc, mods);
			numTraits = i + 1;
		}
		Debug.Log("Loaded " + numTraits + " traits.");
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
		if (HP <= 0)
			Die ();
	}


	public void JoinsUp(){
		string st = string.Format("{0}: {1} enlists at the rank of {2}.",StrategicClock.GetDate(), CharName, GetJobTitle());
		CommissionDate = StrategicClock.GetDate ();
		AddHistory (st);
	}

	public void AppointCaptain(StrategicShip s){
		s.AssignOfficer (this, NavalCommanderRole.CMD);
		NavalRole = NavalCommanderRole.CMD;
		empire.Unassigned.Remove (this);
		shipPosting = s;
		string st = string.Format("{0}: {1} is placed in command of {2}.",StrategicClock.GetDate(),GetNameString(), s.ShipName);
		AddHistory (st);
	}

	public void AppointXO(StrategicShip s){
		s.AssignOfficer (this, NavalCommanderRole.XO);
		NavalRole = NavalCommanderRole.XO;
		empire.Unassigned.Remove (this);
		shipPosting = s;
		string st = string.Format("{0}: {1} is appointed Executive Officer onboard {2}.",StrategicClock.GetDate(),GetNameString(), s.ShipName);
		AddHistory (st);
	}

	public void Unassign(){
		NavalRole = NavalCommanderRole.NONE;
		empire.Unassigned.Add (this);
		shipPosting = null;
		string st = string.Format("{0}: {1} has no current assignment.",StrategicClock.GetDate(),GetNameString(true));
		AddHistory (st);
	}

	public void AwardMedal(Medal m){
		Medals.Add (m);
		PromotionPoints += m.Points;
		string st = string.Format("{0}: {1} is awarded the {2}.",StrategicClock.GetDate(),GetNameString(true), m.Name);
		AddHistory (st);
	}

	public void AwardMedal(Medal m, string reason){
		Medals.Add (m);
		PromotionPoints += m.Points;
		string st = string.Format("{0}: {1} is awarded the {2} for '{3}'.",StrategicClock.GetDate(),GetNameString(true), m.Name, reason);
		AddHistory (st);
	}

	public void StartResearch(string TechName){
		string st = string.Format("{0}: {1} begins research into {2}.",StrategicClock.GetDate(),GetNameString(true), TechName);
		AddHistory (st);
	}

	public void DidResearch(string TechName){
		string st = string.Format("{0}: {1} completes research into {2}.",StrategicClock.GetDate(),GetNameString(true), TechName);
		AddHistory (st);
	}

	public void Promote(){
		if (Rank >= JobTitlesDictLong[Role].Count - 1)
			return;
		Rank++;
		string st = string.Format("{0}: {1} is promoted to the rank of {2}.",StrategicClock.GetDate(),CharName, GetJobTitle());
		AddHistory (st);
	}

	public void Demote(){
		if (Rank == 0)
			return;
		PromotionPoints -= 500;
		Rank--;
		string st = string.Format("{0}: {1} is demoted to the rank of {2}.",StrategicClock.GetDate(), CharName, GetJobTitle());
		AddHistory (st);
	}

	public void MakeNoble(){
		if (Noble)
			return;
		NobleRank = 0;
		string st = string.Format("{0}: {1} is made a peer of the realm.", StrategicClock.GetDate(), GetNameString(true));
		AddHistory (st);
		Noble = true;
	}

	public void MakeNoble(int rank){
		if (Noble)
			return;
		Noble = true;
		NobleRank = rank;
		string st = string.Format("{0}: {1} is made a {2} of the realm.", StrategicClock.GetDate(),  GetNameString(true),GetNobleTitle());
		AddHistory (st);
	}

	public void PromoteNoble(){
		if (NobleRank == empire.Gov.NobleRanks.Count - 1)
			return;
		Noble = true;
		NobleRank++;
		string st = string.Format("{0}: {1} is raised to the station of {2}.",StrategicClock.GetDate(),GetNameString(),GetNobleTitle());
		AddHistory (st);
	}

	public void Retire(bool Forced){
		string st = "";
		if (Forced) {
			 st = string.Format ("{0}: {1} was dishonorably discharged from the service.", StrategicClock.GetDate (), GetNameString ());
		
		} else {
			st = string.Format ("{0}: {1} {2} has retired honorably from the service.", StrategicClock.GetDate (),GetNobleTitle(), GetNameString ());
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
				sb.Append (PersonalityAspectsStrings [i] + ": " + PersonalityAspects [i]+"\t");
			}
		} else {
			for (int i = 0; i < 6; i++) {
				sb.AppendLine (PersonalityAspectsStrings [i] + ": " + PersonalityAspects [i]);
			}
		}
		return sb.ToString ();
	}

	public void Die(){
		string st = string.Format("{0}: {1} was killed in the destruction of the {2}.",StrategicClock.GetDate(),GetNameString(), shipPosting.ShipName);
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
