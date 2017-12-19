using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class PoliticalOffice{
	#region Static Collections
	public static Dictionary<Empire,List<PoliticalOffice>> AllOffices = new Dictionary<Empire, List<PoliticalOffice>>();
	public static Dictionary<Character,PoliticalOffice>CharacterToOffice = new Dictionary<Character, PoliticalOffice>();
	public static Dictionary<PoliticalOffice,Character>OfficeToCharacter = new Dictionary<PoliticalOffice,Character>();
	#endregion

	public Empire empire;

	public string OfficeName;
	public Character Director;

	public float YearlyBudget; //Wealth increases to this number each year
	public Wallet OfficeFunds;

	public PoliticalOffice Parent;
	public List<PoliticalOffice> Children = new List<PoliticalOffice>();

	public PoliticalOffice(Empire e, string name, PoliticalOffice parent = null){
		OfficeName = name;
		empire = e;

		if (AllOffices.ContainsKey (empire)) {
			AllOffices [empire].Add (this);
		} else {
			AllOffices.Add (empire, new List<PoliticalOffice> (){ this });
		}

		CharacterToOffice.Add (Director, this);
		OfficeToCharacter.Add (this, Director);

		if (parent != null) {
			Parent = parent;
			Parent.Children.Add (this);
		}
	}

	//Methods
	public void AppointDirector(Character c){
		if (c.Role == OfficerRoles.Politician) {
			if (Director != null)
				Director.AppointPoliticalOffice (this, false);
			Director = c;
			Director.AppointPoliticalOffice (this, true);
		}
	}







}

public class Government{




	/*
	public GovernmentTypes GovType;

	public Character Leader;
	public string LeaderTitle;
	static List<string> LeaderTitlesM = new List<string> () {
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
	static List<string> LeaderTitlesF = new List<string> () {
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
	static List<string> WarTitleM = new List<string> () {
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
	static List<string> EconomyTitleM = new List<string> () {
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
	static List<string> EconomyTitleF = new List<string> () {
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





*/


	}

