using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PassengerLiner : MonoBehaviour {

	public City target;

	public int mPassengers;
	public int passengers;
	bool full = false;
	bool hasMission = false;
	public GameObject Controller;

	public int faction;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (!hasMission) {
			GenerateMission ();
		}
	}

	void GenerateMission(){
		List<City> Source = new List<City> ();
		Source.AddRange (Collections.PopSources);
		Source.OrderBy(
			targ => Vector3.Distance(this.transform.position,targ.transform.position)).ToList();
		try{
		target = Source [0];

		}
		catch{
			Debug.Log ("No valid sources.");
		}
	}
}
