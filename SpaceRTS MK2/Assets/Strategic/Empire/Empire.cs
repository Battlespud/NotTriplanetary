﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine.UI;


//TODO this will probably be removed later. Just leave for now.
public enum Eras{
	Primitive,
	Classical,
	Industrial,
	EarlySpace,
	Stellar,
	Interstellar
}



public class Empire : MonoBehaviour {

	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//STATICS 
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static Dictionary<OfficerRoles, KeyValuePair<int, List<float>>> StartingOfficersDict = new Dictionary<OfficerRoles, KeyValuePair<int, List<float>>>();
	public static List<Empire> AllEmpires = new List<Empire>();
	public static List<ILocation> AllLocations = new List<ILocation>(); //This is for ALL empires
	//public static ILocation DeadLocation = new ILocation ();

	static System.Random rnd = new System.Random();
	public static DeadLocation DeadLoc = new DeadLocation(); //TODO i forgot what this is


	////////////////////////////////////////////////////////////////////////////////////////////////////
	//COLLECTIONS 
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public List<Colony>Colonies = new List<Colony>();
	public List<Team> Teams = new List<Team>();
//	public List<Character>Characters = new List<Character>();
	public List<Character> Unassigned = new List<Character>();
//	public List<Character> Dead = new List<Character>();
	public List<StrategicShip> Ships = new List<StrategicShip> ();
	public List<StrategicShipyard> Yards = new List<StrategicShipyard> ();
	public List<GroundUnit>GroundUnits = new List<GroundUnit>();
	public TechTree EmpireTechTree;
	public DesignerToken Token;
	public List<Tech> AvailableTechs = new List<Tech> ();
	public List<string> DebugAvailableTechNames = new List<string>();

	////////////////////////////////////////////////////////////////////////////////////////////////////
	//REFERENCES 
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public FAC Faction;
	public Eras Era;
	public string EmpireName;
	public Government Gov = new Government();
	public EmpireStats Stats;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//SETTINGS 
	////////////////////////////////////////////////////////////////////////////////////////////////////
	public static bool RandomTraits = true;
	int GroundUnitCounter = 0; //Just used in naming ground units. Fluff
	public bool DistributeCaptains = false;
	public int StartingOfficers = 100;
	public int CurrentOfficers;
	public bool Player = true;
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//LOGS 
	////////////////////////////////////////////////////////////////////////////////////////////////////
	#region Logs
	public Dictionary<string,List<EmpireLogEntry>>Logbook = new Dictionary<string, List<EmpireLogEntry>>();

	public void AddLog(EmpireLogEntry Entry = null){
		try{
			if(Entry != null){
		if (Logbook.ContainsKey (Entry.Date)) {
			Logbook [Entry.Date].Add (Entry);
		} else {
			Logbook.Add(Entry.Date,new List<EmpireLogEntry>());
			Logbook [Entry.Date].Add (Entry);
		}
			}
			else{
				if (!Logbook.ContainsKey (StrategicClock.GetDate())){
					Logbook.Add(Entry.Date,new List<EmpireLogEntry>());
				}
			}
		}
		catch{
			Debug.Log (Entry.Date + "  Key: ");
		}
	}

	public List<EmpireLogEntry> GetLogs(string Date){
		List<EmpireLogEntry> Entries = new List<EmpireLogEntry>();
		if (Logbook.ContainsKey (Date))
			Entries = Logbook [Date];
		return Entries;
	}
	#endregion

	public Dictionary<Theme,float>EmpireThemes = new Dictionary<Theme, float>();

	////////////////////////////////////////////////////////////////////////////////////////////////////
	//COLONY 
	////////////////////////////////////////////////////////////////////////////////////////////////////
	#region Colony
	public void AddColony(Colony c){
		if(!Colonies.Contains(c))
			Colonies.Add (c);
	}

	public void RemoveColony(Colony c){
		if(Colonies.Contains(c))
			Colonies.Remove (c);
	}
	#endregion
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//UNITY 
	////////////////////////////////////////////////////////////////////////////////////////////////////

	void Awake(){
		BuildTechTree ();
		Stats = new EmpireStats();
		ResearchScreenManager.ActiveEmpire = this;
		OfficerManagerUI.ActiveEmpire = this;
		StrategicClock.PhaseChange.AddListener (PhaseManager);
	}

	// Use this for initialization
	void Start () {
		AllEmpires.Add (this);
		for(int i = 0; i < 12; i++){
			if (Logbook.ContainsKey (StrategicClock.strategicClock.GetFutureDate (i))) {
				Logbook.Add (StrategicClock.strategicClock.GetFutureDate (i), new List<EmpireLogEntry> ());
			}
		}
		Token = new DesignerToken (EmpireName);
//		Debug.Log (EmpireTechTree.TechByID.Count);
		AvailableTechs = EmpireTechTree.GetAvailableTech ();
		//	Debug.Log (AvailableTechs.Count);
		foreach (Tech t in AvailableTechs) {
			DebugAvailableTechNames.Add (t.Name);
		}
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.MILITARY,1,this,"NOTHING BUT THE RAIN",string.Format("**//nothing but the rain-"));
		StartCoroutine(GenerateStartingOfficerCorps());
	}
	
	// Update is called once per frame
	void Update () {
		if (DistributeCaptains) {
			DistributeOfficers ();
			DistributeCaptains = false;
		}
		//	CurrentOfficers = Character.CharactersByEmpire[this].Count;
	}
	
	void PhaseManager(Phase p){
		switch (p) {
		case(Phase.ORDERS):
			{
				EmpireTechTree.DoResearch ();
				AvailableTechs = EmpireTechTree.GetAvailableTech ();
				ProgressMaint();
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

	void ProgressMaint(){
		foreach (StrategicShip ship in Ships)
		{
			ship.Turn();

		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////////
	//CHARACTERS 
	////////////////////////////////////////////////////////////////////////////////////////////////////
	#region Characters
	
	//How many of each type of character an empire starts with.
	static KeyValuePair<int,List<float>> StartingByRole(OfficerRoles r)
	{
		int num = 100;
		//Default Rank distribution.
		List<float> dist = new List<float>(){.40f,.2f,.15f,.1f,.075f,.05f,.035f,.025f};

		switch (r)
		{
			case OfficerRoles.Navy:
				num = 50;
				break;
			case OfficerRoles.Army:
				num = 30;
				break;
			case OfficerRoles.Government:
				num = 20;
				break;
			case OfficerRoles.Research:
				num = 15;
				break;
			case OfficerRoles.Intelligence:
				dist= new List<float>(){.50f,4f,.1f};
				num = 5;
				break;
			case OfficerRoles.Police:
				dist= new List<float>(){.50f,4f,.1f};
				num = 12;
				break;
			case OfficerRoles.Child:
				num = 0;
				break;
			case OfficerRoles.Corporate:
				dist= new List<float>(){.50f,.3f,.2f};
				num = 5;
				break;
			case OfficerRoles.Social:
				num = 40;
				break;
			case OfficerRoles.Merchant:
				num = 40;
				break;
			case OfficerRoles.Scientist:
				num = 15;
				break;
			case OfficerRoles.Politician:
				num = 30;
				break;
			case OfficerRoles.Media:
				num = 7;
				break;
			case OfficerRoles.Engineer:
				num = 10;
				break;
			case OfficerRoles.Noble:
				dist= new List<float>(){1f};
				num = 30;
				break;
			case OfficerRoles.Retired:
				num = 15;
				break;
			case OfficerRoles.Terrorist:
				num = 5;
				break;
			case OfficerRoles.Rebel:
				num = 10;
				break;
			case OfficerRoles.Spy:
				dist= new List<float>(){.50f,.3f,.2f,.1f};
				num = 3;
				break;
			case OfficerRoles.Criminal:
				num = 40;
				break;
			default:
				throw new ArgumentOutOfRangeException("r", r, null);
		}
		KeyValuePair<int, List<float>> result =  new KeyValuePair<int, List<float>>(num,dist);
		return result;
	}
	
	public IEnumerator GenerateStartingOfficerCorps()
	{
		Dictionary<OfficerRoles, KeyValuePair<int, List<float>>> all = new Dictionary<OfficerRoles, KeyValuePair<int, List<float>>>();
		foreach (OfficerRoles v in Enum.GetValues(typeof(OfficerRoles)))
		{
			all.Add(v, StartingByRole(v));
		}
		int t = 0;
		while (ThemeManager.Initialized != true)
		{
			t++;
			yield return null;
		}
		Character.GenerateCharacters(this,all);

	}

	//Sends unassigned officers to unmanned ships, with highest rank officers going to largest ships.
	public void DistributeOfficers()
	{
		
		List<StrategicShip> ShipsBySize = new List<StrategicShip>();
		ShipsBySize = Ships.OrderByDescending(x => x.Hull.Size).ToList();
		foreach (StrategicShip s in ShipsBySize) {
			if (s.Executive == null || s.Captain == null) {
				if (s.Captain == null) {
					Character prospect = Unassigned.OrderByDescending (x => x.Rank).ToList() [0];
					prospect.MoveTo (s);
				}
				if (s.Executive == null) {
					Character prospect = Unassigned.OrderBy (x => x.Rank).ToList() [0];
					prospect.MoveTo (s);
				}
			}
		}
	}

	public List<Character>GetCharactersByType(OfficerRoles r, List<Character> CharSet){
		List<Character> ch = new List<Character> ();
		List<Character> imthenulls = new List<Character> ();
		if (CharSet.Count < 1)
			CharSet.AddRange (Character.CharactersByEmpire[this]);
		foreach (Character c in CharSet) {
			if (c != null) {
				try {
					if (c.Role == r)
						ch.Add (c);
				} catch {
					if (string.IsNullOrEmpty (c.CharName))
						//c.CharName = "Null";
					Debug.LogError ("Name has failed to process");
				}
			} else {
				imthenulls.Add (c);
				Debug.LogError ("A null character was removed from the list");
			}
		}
		foreach (Character c in imthenulls) {
			Character.CharactersByEmpire[this].Remove (c);
		}
		ch = ch.OrderByDescending (x => x.Rank).ThenByDescending(x => x.Noble).ThenByDescending(x => x.NobleRank).ToList ();
		return ch;
	}

	public List<Character>GetCharactersByType(OfficerRoles r){
		List<Character> ch = new List<Character> ();
		List<Character> imthenulls = new List<Character> ();
		foreach (Character c in Character.CharactersByEmpire[this]) {
			if (c != null) {
				try {
					if (c.Role == r)
						ch.Add (c);
				} catch {
					if (string.IsNullOrEmpty (c.CharName))
					Debug.LogError ("Name has failed to process");
				}
			} else {
				imthenulls.Add (c);
				Debug.LogError ("A null character was removed from the list");
			}
		}
		foreach (Character c in imthenulls) {
			Character.CharactersByEmpire[this].Remove (c);
		}
		ch = ch.OrderByDescending (x => x.Rank).ThenByDescending(x => x.Noble).ThenByDescending(x => x.NobleRank).ToList ();
		return ch;
	}

	public List<Character>GetCharactersByTypeAndRank(OfficerRoles r, int rank){
		List<Character> ch = new List<Character> ();
		List<Character> imthenulls = new List<Character> ();

		foreach (Character c in Character.CharactersByEmpire[this]) {
			if (c != null) {
				try {
					if (c.Role == r && c.Rank == rank)
						ch.Add (c);
				} catch {
					if (c.CharName == null)
					Debug.LogError ("Name has failed to process");
				}
			}
			else {
				imthenulls.Add (c);
				Debug.LogError ("A null character was removed from the list");
			}
		}
		foreach (Character c in imthenulls) {
			Character.CharactersByEmpire[this].Remove (c);
		}
		ch = ch.OrderByDescending (x => x.Rank).ThenByDescending(x => x.Noble).ThenByDescending(x => x.NobleRank).ToList ();
		return ch;
	}

	public List<Character> GetCharactersAtLocation(ILocation loc, OfficerRoles? rNullable = null, bool softReq = true){
		List<Character> Output = new List<Character> ();
		foreach (Character c in Character.CharactersByEmpire[this]) {
			if (c.Location == loc)
				Output.Add (c);
		}
		if (rNullable != null && Output.Count > 0) {
			OfficerRoles r = rNullable.Value;
			Output = GetCharactersByType (r, Output);
			if (Output.Count < 1 && softReq) {
				foreach (Character c in Character.CharactersByEmpire[this]) {
					if (c.Location == loc)
						Output.Add (c);
				}
			}
		}
		if (rNullable == null) {
			Output = Output.OrderByDescending (x => x.Rank).ThenByDescending (x => x.Noble).ThenByDescending (x => x.NobleRank).ToList ();
		} else {
			Output = Output.OrderBy (x => x.Role).ThenByDescending (x => x.Rank).ThenByDescending (x => x.Noble).ThenByDescending (x => x.NobleRank).ToList ();
		}
		return Output;
	}
	#endregion

	////////////////////////////////////////////////////////////////////////////////////////////////////
	//TECH 
	////////////////////////////////////////////////////////////////////////////////////////////////////

//Called at star tto setup the tech tree.
	void BuildTechTree(){
		EmpireTechTree = new TechTree (this);
		string[] TechTreeText = File.ReadAllLines (System.IO.Path.Combine (Application.streamingAssetsPath, "Tech/TechTree.txt"));
		for (int i = 0; i < TechTreeText.Count(); i++) {
			if (TechTreeText [i].Contains ("Tech Name:")) {
				Tech t = new Tech ();
				t.Name = SpaceTrim (TechTreeText [i].Remove (0, 10));
				t.ID = SpaceTrim (TechTreeText [i + 1].Remove(0,8));
				t.Description = SpaceTrim (TechTreeText [i + 2].Remove(0,12));
				try{
				t.Section = (TechSection)System.Enum.Parse (typeof(TechSection), SpaceTrim (TechTreeText [i + 3].Remove(0,8)), true);
				}
				catch{
					Debug.LogError ("Invalid TechSection: " + SpaceTrim (TechTreeText [i + 3].Remove(0,8)));
				}
				try{
				t.SubSection = (TechSubSection)System.Enum.Parse (typeof(TechSubSection), SpaceTrim (TechTreeText [i + 4].Remove(0,11)), true);
				}
				catch{
					Debug.LogError ("Invalid TechSubSection: " + SpaceTrim (TechTreeText [i + 4].Remove (0, 11)));
				}
				t.Cost = int.Parse (SpaceTrim( TechTreeText[i + 5].Remove(0,5)));
				string[] requirements = SpaceTrim (TechTreeText [i + 6]).Remove(0,13).Split (',');
				t.Requirements = new List<string> ();
					foreach (string s in requirements) {
					if(s != "")
						t.Requirements.Add (SpaceTrim (s));
				}
				EmpireTechTree.AddTech (t);
				i += 5;
			}
		}
	}
	//Just a little utility to take care of any missed spaces in the techtree files.  
	string SpaceTrim(string s){
		string o = s.Trim (new char[] { ' ' });
		return o;
	}





	////////////////////////////////////////////////////////////////////////////////////////////////////
	//NAMING 
	////////////////////////////////////////////////////////////////////////////////////////////////////

	#region Naming

	struct Range{
		float bottom;
		float top;
		public Theme theme;
		public Range(float a, float b, Theme d){
			bottom = a;
			top = b;
			theme = d;
		}
		public bool InRange(float c){
			if (c >= bottom && c < top)
				return true;
			return false;
		}
	}

	public string GetName(GroundUnit unit, Theme theme = null){
		string name = "NoValidThemeSet";
		List<Range> Ranges = new List<Range> ();
		if (theme == null) {
			float last=0;
			Theme lastT = null;
			foreach (Theme t in EmpireThemes.Keys) {
				if (lastT == null)
					last = 0;
				Ranges.Add(new Range(last,last+EmpireThemes[t],t));
				lastT = t;
			}
			float roll = rnd.NextFloat (0, last);
			foreach (Range r in Ranges) {
				if (r.InRange (roll)) {
					lastT = r.theme;
				}
			//	name = ThemeManager.GetName (unit);
			}
		} else {

		}
		name = GroundUnitCounter + ". Mobile Infanterie";
		return name;
	}

	public string GetName(StrategicShip unit, Theme theme = null){
		string name = "null";
		List<Range> Ranges = new List<Range> ();
		if (theme == null) {
			float last=0f;
			Theme lastT=null;
			foreach (Theme t in EmpireThemes.Keys) {
				if (lastT == null)
					last = 0;
				Ranges.Add(new Range(last,last+EmpireThemes[t],t));
				lastT = t;
			}
			float roll = rnd.NextFloat (0, last);
			foreach (Range r in Ranges) {
				if (r.InRange (roll)) {
					theme = r.theme;
				}
				name = ThemeManager.Manager.GetName (unit, theme);
			}
		} else {
			name = ThemeManager.Manager.GetName (unit, theme);
		}
		if (name == "null")
			name = ThemeManager.Manager.GetName (unit, ThemeManager.ThemesDict ["Alliance"]);
		return name;
	}
	#endregion


	
	
}





