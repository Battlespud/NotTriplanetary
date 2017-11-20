using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine.UI;


public enum Eras{
	Primitive,
	Classical,
	Industrial,
	EarlySpace,
	Stellar,
	Interstellar
}

public enum LogCategories{
	DEFAULT,
	MILITARY,
	ECONOMIC,
	TECH,
	EXPLORATION
}

public class EmpireLogEntry{
	Empire empire;

	public int Priority = 5; //1 is highest

	public List<StrategicShip> InvolvedShips = new List<StrategicShip> ();
	public List<GroundUnit> InvolvedGroundUnits = new List<GroundUnit> ();
	public List<Character> InvolvedCharacters = new List<Character> ();
	public LogCategories Category;
	public string Headline;
	public string Date;
	public string Description;

	public bool Seen = false;

	public EmpireLogEntry(LogCategories l, int i,Empire e, string Head, string Descr = "", List<Character> chars = null, List<StrategicShip> ships = null, List<GroundUnit> units = null){
		Priority = i;
		Category = l;
		if (Priority < 1)
			Priority = 1;
		if (Priority > 6)
			Priority = 6;
		Headline = Head;
		Description = Descr;
		InvolvedShips = ships;
		InvolvedGroundUnits = units;
		InvolvedCharacters = chars;
		Date = StrategicClock.GetDate ();
//		Debug.Log (Date);
		empire = e;
		e.AddLog (this);
	}
}

public class Empire : MonoBehaviour {

	public static List<Empire> AllEmpires = new List<Empire>();


	//Essentially our version of a faction.  Multiple empires of the same FAC can exist, representing
	//political groups within the whole.   Each will have its own officer core, but can share everything else.
	static System.Random rnd = new System.Random();
	public FAC Faction;
	public Eras Era;
	public string EmpireName;
	public Government Gov = new Government();

	public  int StartingOfficers = 100;

	public int CurrentOfficers;

	public bool Player = true;

	public TechTree EmpireTechTree;
	public DesignerToken Token;
	public List<Tech> AvailableTechs = new List<Tech> ();
	public List<string> DebugAvailableTechNames = new List<string>();

	public List<Character>Characters = new List<Character>();
	public List<Character> Unassigned = new List<Character>();
	public List<Character> Dead = new List<Character>();
	public List<StrategicShip> Ships = new List<StrategicShip> ();
	public List<StrategicShipyard> Yards = new List<StrategicShipyard> ();
	public List<GroundUnit>GroundUnits = new List<GroundUnit>();
	int GroundUnitIndex = 0;

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

	public Dictionary<Theme,float>EmpireThemes = new Dictionary<Theme, float>();

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
		foreach (StrategicShip ship in Ships) {
			if (ship.IsDeployed) {
				ship.MaintClock += .1f;
				ship.RollMaint ();
			}
			if (ship.InDrydock) {
				ship.MaintClock -= ship.OverhaulMulti * .1f;
			}
			if (ship.MaintClock <= 0f) {
				ship.MaintClock = 0f;
			}
		}
	}


	public IEnumerator GenerateStartingOfficerCorps(int i){
		Debug.Log ("Generating starting officers: " + i);
		int t = 0;
		while (ThemeManager.Initialized != true) {
			t++;
			Debug.Log (t + " Officers waiting for theme manager..." );
			yield return null;
		}
			StartCoroutine(GenerateCorps (StartingOfficers));
			StartCoroutine(GenerateScientists (15));

	}
		

	public void DistributeOfficers(){
		foreach (StrategicShip s in Ships) {
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
			CharSet.AddRange (Characters);
		foreach (Character c in CharSet) {
			if (c != null) {
				try {
					if (c.Role == r)
						ch.Add (c);
				} catch {
					if (string.IsNullOrEmpty (c.CharName))
						c.CharName = "Null";
					Debug.LogError (c.CharName + " has failed to process");
				}
			} else {
				imthenulls.Add (c);
				Debug.LogError ("A null character was removed from the list");
			}
		}
		foreach (Character c in imthenulls) {
			Characters.Remove (c);
		}
		ch = ch.OrderByDescending (x => x.Rank).ThenByDescending(x => x.Noble).ThenByDescending(x => x.NobleRank).ToList ();
		return ch;
	}

	public List<Character>GetCharactersByType(OfficerRoles r){
		List<Character> ch = new List<Character> ();
		List<Character> imthenulls = new List<Character> ();
		foreach (Character c in Characters) {
			if (c != null) {
				try {
					if (c.Role == r)
						ch.Add (c);
				} catch {
					if (string.IsNullOrEmpty (c.CharName))
						c.CharName = "Null";
					Debug.LogError (c.CharName + " has failed to process");
				}
			} else {
				imthenulls.Add (c);
				Debug.LogError ("A null character was removed from the list");
			}
		}
		foreach (Character c in imthenulls) {
			Characters.Remove (c);
		}
		ch = ch.OrderByDescending (x => x.Rank).ThenByDescending(x => x.Noble).ThenByDescending(x => x.NobleRank).ToList ();
		return ch;
	}

	public List<Character>GetCharactersByTypeAndRank(OfficerRoles r, int rank){
		List<Character> ch = new List<Character> ();
		List<Character> imthenulls = new List<Character> ();

		foreach (Character c in Characters) {
			if (c != null) {
				try {
					if (c.Role == r && c.Rank == rank)
						ch.Add (c);
				} catch {
					if (c.CharName == null)
						c.CharName = "Null";
					Debug.Log (c.CharName + " has failed to process");
				}
			}
			else {
				imthenulls.Add (c);
				Debug.LogError ("A null character was removed from the list");
			}
		}
		foreach (Character c in imthenulls) {
			Characters.Remove (c);
		}
		ch = ch.OrderByDescending (x => x.Rank).ThenByDescending(x => x.Noble).ThenByDescending(x => x.NobleRank).ToList ();
		return ch;
	}

	public List<Character> GetCharactersAtLocation(ILocation loc, OfficerRoles? rNullable = null){
		List<Character> Output = new List<Character> ();
		foreach (Character c in Characters) {
			if (c.Location == loc)
				Output.Add (c);
		}
		if (rNullable != null && Output.Count > 0) {
			OfficerRoles r = rNullable.Value;
			Output = GetCharactersByType (r, Output);
			if (Output.Count < 1) {
				Debug.LogError ("No valid characters found, finding all other characters in order to prevent exception!");
				foreach (Character c in Characters) {
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

	public static bool RandomTraits = true;


	IEnumerator GenerateCorps(int i){
		//Max rank starting  = 6
		int index = 0;
		List<float> Distribution = new List<float>(){.40f,.2f,.15f,.1f,.075f,.05f,.035f,.025f};
		float NobilityChance = .2f;
		foreach (float f in Distribution) {
			for (int d = (int)(i * f); d > 0; d--) {
				Character c = new Character(index,OfficerRoles.Navy);
				c.Age = (int)(rnd.Next (24, 29) + index*rnd.Next(1.25f,3f));
				Characters.Add (c);
				c.Noble = MakeNobleChance (NobilityChance + index/100*5f);
				c.empire = this;
				c.AwardMedal(Medal.DesignedMedals[0]); //All starting characters recieve a pioneer medal to show their seniority.
				if (RandomTraits) {
					int b = rnd.Next (0, 3);
					for(b=b;b >= 0; b--){
						int ind = rnd.Next(0,Trait.Traits.Count);
						try{
						c.AddTrait (Trait.Traits [ind]);
						}
						catch{
						}
					}
				}
				yield return Ninja.JumpToUnity;
			//	c.Output ();
				yield return Ninja.JumpBack;
			}
			index++;
		}
		index = 0;
		foreach (float f in Distribution) {
			for (int d = (int)(i * f); d > 0; d--) {
				Character c = new Character(index,OfficerRoles.Army);
				c.Age = (int)(rnd.Next (24, 29) + index*rnd.Next(1.45f,4f));
				Characters.Add (c);
				c.Noble = MakeNobleChance (NobilityChance/2 + (index-2f)/100*5f);
				c.empire = this;
				c.AwardMedal(Medal.DesignedMedals[0]); //All starting characters recieve a pioneer medal to show their seniority.
				if (RandomTraits) {
					int b = rnd.Next (0, 3);
					for(b=b;b >= 0; b--){
						int ind = rnd.Next(0,Trait.Traits.Count);
						try{
							c.AddTrait (Trait.Traits [ind]);
						}
						catch{
						}
					}
				}
				yield return Ninja.JumpToUnity;
			//	c.Output ();
				yield return Ninja.JumpBack;
			}
			index++;
		}
		Unassigned.AddRange (Characters);
		yield return Ninja.JumpToUnity;
	}

	IEnumerator GenerateScientists(int i){
		//Max rank starting  = 6
		int index = 0;
		List<float> Distribution = new List<float>(){.55f,.2f,.15f,.1f,.05f,.025f};
		float NobilityChance = .1f;
		foreach (float f in Distribution) {
			for (int d = (int)(i * f); d > 0; d--) {
				Character c = new Character(index, OfficerRoles.Research);
				c.Age = (int)(rnd.Next (26, 32) + index/100*rnd.Next(1.85f,7.75f));
				Characters.Add (c);
				c.Noble = MakeNobleChance (NobilityChance + index/100f*5f);
				c.empire = this;
				c.AwardMedal(Medal.DesignedMedals[0]); //All starting characters recieve a pioneer medal to show their seniority.
				if (RandomTraits) {
					int b = rnd.Next (0, 3);
					for(b=b;b >= 0; b--){
						int ind = rnd.Next(0,Trait.Traits.Count);
						try{
							c.AddTrait (Trait.Traits [ind]);
						}
						catch{
						}
					}
				}
				yield return Ninja.JumpToUnity;
			//	c.Output ();
				yield return Ninja.JumpBack;
			}
			index++;
		}
		index = 0;
		Distribution = new List<float>(){.3f,.25f,.2f,.1f,.1f,.05f};
		foreach (float f in Distribution) {
			for (int d = (int)(i*1.5f * f); d > 0; d--) {
				Character c = new Character(index, OfficerRoles.Government);
				c.Age = (int)(rnd.Next (22, 32) + index*rnd.Next(2.85f,3.75f));
				Characters.Add (c);
				c.Noble = MakeNobleChance (NobilityChance*2 + (index - 2.5f)/100*15f);
				c.empire = this;
				c.AwardMedal(Medal.DesignedMedals[0]); //All starting characters recieve a pioneer medal to show their seniority.
				if (RandomTraits) {
					int b = rnd.Next (0, 3);
					for(b=b;b >= 0; b--){
						int ind = rnd.Next(0,Trait.Traits.Count);
						try{
							c.AddTrait (Trait.Traits [ind]);
						}
						catch{
						}
					}
				}
				yield return Ninja.JumpToUnity;
			//	c.Output ();
				yield return Ninja.JumpBack;
			}
			index++;
		}
		Unassigned.AddRange (Characters);
		yield return Ninja.JumpToUnity;
	}
	public bool DistributeCaptains = false;

	bool MakeNobleChance(float f){
		if (rnd.NextFloat (0, 1f) < f)
			return true;
		return false;
	}

	void Awake(){
		BuildTechTree ();
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
		StartCoroutine( GenerateStartingOfficerCorps (StartingOfficers));
		foreach (Tech t in AvailableTechs) {
			DebugAvailableTechNames.Add (t.Name);
		}
		EmpireLogEntry E = new EmpireLogEntry(LogCategories.MILITARY,1,this,"NOTHING BUT THE RAIN",string.Format("**//nothing but the rain-"));

	}

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

	string SpaceTrim(string s){
		string o = s.Trim (new char[] { ' ' });
		return o;
	}



	// Update is called once per frame
	void Update () {
		if (DistributeCaptains) {
			DistributeOfficers ();
			DistributeCaptains = false;
		}
		CurrentOfficers = Characters.Count;
	}



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
		name = GroundUnitIndex + ". Mobile Infanterie";
		return name;
	}

	public string GetName(StrategicShip unit, Theme theme = null){
		string name = "NoValidThemeSet";
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
		return name;
	}
	#endregion
}

public enum GovernmentTypes{
	Theocracy,
	Republic,
	Communist,
	Corporatocracy,
	Dictatorship,
	Monarchy,
	Empire,
	StarKingdom,
	Islamic
}

public class MilitaryStaff{


}


public class Government{

	public GovernmentTypes GovType;

	public Character Leader;
	public string LeaderTitle;
	 List<string> LeaderTitlesM = new List<string> () {
		"High Priest",
		"President",
		"Chairman",
		"Chief Executive",
		"Generalissimo",
		"King",
		"Emperor",
		"King",
		"Caliph"
	};
	 List<string> LeaderTitlesF = new List<string> () {
		"High Priestess",
		"President",
		"Chairwoman",
		"Chief Executive",
		"Generalissa",
		"Queen",
		"Empress",
		"Queen",
		"Caliphess"
	};

	public Character War;
	public string WarTitle;
	 List<string> WarTitleM = new List<string> () {
		"Sword of the Faithful",
		"Secretary of Defense",
		"Defender of the People",
		"Chief Security Officer",
		"Warlord",
		"Sword of the ",
		"Right hand of the {0}",
		"Sword of the {0}",
		"Sword of Allah"
	};
	
	static List<string> WarTitleF = new List<string> () {
		"High Priestess",
		"President",
		"Chairwoman",
		"Chief Executive",
		"Generalissa",
		"Queen",
		"Empress",
		"Queen",
		"Sword of Allah"
	};

	public Character Economy;
	public string EconomyTitle;
	List<string> EconomyTitleM = new List<string> () {
		"Sword of the Faithful",
		"Secretary of Defense",
		"Secretary of Production",
		"Chief Economics Officer",
		"Head Jew",
		string.Format("Lord of the Exeqchuer"),
		string.Format("Lord of the Exeqchuer"),
		string.Format("Lord of the Exeqchuer"),
		"Keeper of the Gold"
	};
	List<string> EconomyTitleF = new List<string> () {
		"Sword of the Faithful",
		"Secretary of Defense",
		"Secretary of Production",
		"Chief Economics Officer",
		"Head Jew",
		string.Format("Lord of the Exeqchuer"),
		string.Format("Lord of the Exeqchuer"),
		string.Format("Lord of the Exeqchuer"),
		"Keeper of the Gold"
	};

	public bool HasNobleClass;
	 List<string> NobleRanksM = new List<string> (){ "Peer", "Honor", "Baron", "Count", "Duke" };
	 List<string> NobleRanksF = new List<string> (){ "Peer", "Honor", "Baron", "Count", "Duke" };
	public Dictionary<Sex,List<string>>NobleRanks = new Dictionary<Sex, List<string>>();

	public Government(){
		NobleRanks.Add (Sex.Female, NobleRanksF);
		NobleRanks.Add (Sex.Male, NobleRanksM);
	}


}
