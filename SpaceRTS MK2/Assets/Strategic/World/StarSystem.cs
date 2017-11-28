using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StarSystem : MonoBehaviour {

	public string SystemName;

	static List<string> AvailableNames = new List<string> ();
	public Dictionary<string,StarSystem> SystemByName = new Dictionary<string, StarSystem>();

	public List<Planet> Planets = new List<Planet>();
	public List<Fleet> Fleets = new List<Fleet>();

	void Awake(){
		if (AvailableNames.Count <= 0) {
			LoadNames ();
		}
	}

	public void LoadNames(){
		AvailableNames.AddRange(File.ReadAllLines((Directory.GetDirectories (System.IO.Path.Combine (Application.streamingAssetsPath, "Planets/SystemNames.txt")))[0]));
		Debug.Log (AvailableNames.Count + " System Names Loaded.");
	}

	// Use this for initialization
	void Start () {
		SystemName = AvailableNames [Random.Range (0, AvailableNames.Count)];
		AvailableNames.Remove (SystemName);
		SystemByName.Add (SystemName, this);
		StrategicClock.PhaseChange.AddListener (PhaseManager);
	}


	void OnTriggerEnter(Collider col){
		if (col.GetComponent<Fleet> ()) {
			Fleets.Add (col.GetComponent<Fleet> ());
		}
		if (col.GetComponent<Planet> ()) {
			Planets.Add (col.GetComponent<Planet> ());
		}
	}
	void OnTriggerExit(Collider col){
		if (col.GetComponent<Fleet> ()) {
			Fleets.Remove (col.GetComponent<Fleet> ());
		}
		if (col.GetComponent<Planet> ()) {
			Planets.Remove (col.GetComponent<Planet> ());
		}
	}


	void PhaseManager(Phase p){
		switch (p) {
		case(Phase.ORDERS):
			{
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

	// Update is called once per frame
	void Update () {

	}
}
