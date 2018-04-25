using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum LogCategories{
	DEFAULT,
	MILITARY,
	ECONOMIC,
	TECH,
	EXPLORATION
}

public class EmpireLogEntry {

	Empire empire;

	static List<string> MeaninglessHeadlines = new List<string> () {"Nothing but the Rain", "See ya, Space Cowboy",
		"Second Star to the Right", "Wait for the Wheel", "Code Zero Zero Zero. Destruct. Zero.",
		"Authorization Alpha-Alpha 3-0-5.", "Situation Normal", "Protocol <3", "Fly, you fool.",
		"In case of doubt, attack.", "Wrong us, shall we not revenge?", "Annie, get your gun.",
		"Roll a hard six.", "Till all are one.","Boom, boom, boom."
	};
		

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
