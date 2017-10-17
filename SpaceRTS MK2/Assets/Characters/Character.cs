using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Roles{
	Scientist,
	Leader,
	General,
	Captain,
	Admiral
}

public class Character : MonoBehaviour {


	public static List<string>NavalRankNames = new List<string>(){"Lieutenant Commander","Commander","Captain","List Captain","Commodore", "Rear Admiral", "Admiral", "Admiral of the Fleet"};

	public string CharName;

	ShipAbstract shipPosting;

	public int Rank;

	public string GetNameString(){
		return string.Format ("{0} {1}", NavalRankNames [Rank], CharName);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
