﻿using System.Collections;
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

public class Empire : MonoBehaviour {

	public static List<Empire> AllEmpires = new List<Empire>();


	//Essentially our version of a faction.  Multiple empires of the same FAC can exist, representing
	//political groups within the whole.   Each will have its own officer core, but can share everything else.
	static System.Random rnd = new System.Random();
	public FAC Faction;
	public Eras Era;
	public string Name;
	public Government Gov;

	public bool Player = true;

	public TechTree EmpireTechTree;
	public List<Tech> AvailableTechs = new List<Tech> ();
	public List<string> DebugAvailableTechNames = new List<string>();

	public List<Character>Characters = new List<Character>();
	public List<Character> Unassigned = new List<Character>();
	public List<Character> Dead = new List<Character>();
	public List<StrategicShip> Ships = new List<StrategicShip> ();



	public void GenerateStartingOfficerCorps(int i){
		Debug.Log ("Generating starting officers: " + i);
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,GenerateCorps(i));
	}

	public void DistributeOfficers(){
		foreach (StrategicShip s in Ships) {
			if (s.Executive == null) {
				for (int i = 0; i < Unassigned.Count; i++) {
					if (Unassigned [i].Rank <= 1 ) {
						Unassigned [i].AppointXO (s);
					}
				}
			}
			if(s.Captain == null){
				for (int i = 0; i < Unassigned.Count; i++) {
					if (Unassigned [i].Rank >= 2 ) {
						Unassigned [i].AppointCaptain (s);
					}
				}
			}
		}
	}

	public List<Character>GetCharactersByType(OfficerRoles r){
		List<Character> ch = new List<Character> ();
		foreach (Character c in Characters) {
			if (c.Role == r)
				ch.Add (c);
		}
		return ch;
	}

	public static bool RandomTraits = true;
	IEnumerator GenerateCorps(int i){
		//Max rank starting  = 6
		int index = 0;
		List<float> Distribution = new List<float>(){.65f,.15f,.1f,.05f, .05f};
		foreach (float f in Distribution) {
			for (int d = (int)(i * f); d > 0; d--) {
				Character c = new Character(index);
				c.Age = (int)(rnd.Next (24, 29) + index*rnd.Next(1.45f,4f));
				Characters.Add (c);
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
				c.Output ();
				yield return Ninja.JumpBack;
			}
			index++;
		}
		Unassigned.AddRange (Characters);
		yield return Ninja.JumpToUnity;
	}

	public bool DistributeCaptains = false;

	// Use this for initialization
	void Start () {
		AllEmpires.Add (this);
		BuildTechTree ();
		Debug.Log (EmpireTechTree.TechByID.Count);
		AvailableTechs = EmpireTechTree.GetAvailableTech ();
		Debug.Log (AvailableTechs.Count);
		GenerateStartingOfficerCorps (100);
		foreach (Tech t in AvailableTechs) {
			DebugAvailableTechNames.Add (t.Name);
		}
	}

	void BuildTechTree(){
		EmpireTechTree = new TechTree ();
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
	}
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
		string.Format("Sword of the {0}", LeaderTitle),
		string.Format("Right hand of the {0}", LeaderTitle),
		string.Format("Sword of the {0}", LeaderTitle),
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
	public List<string> NobleRanks = new List<string>(){"Peer","Honor","Baron","Count","Duke";

}
