using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;


public class SpaceYard : MonoBehaviour {
	public static Player player;
	public int faction;
	public static List<GameObject> ShipPrefabs = new List<GameObject>();
	static bool ListBuilt = false;


	//UI
	public static GameObject SpaceYardUI;
	public static SpaceYard active;
	public static Dropdown drop;
	public static Button buildButton;


	public GameObject BerthsParent;
	public List<Berth> Berths = new List<Berth>();

	public static void Build(){
		Berth open = active.Berths[0];
		bool openslot = false;
		foreach (Berth b in active.Berths) {
			if (!b.Full)
				openslot = true;
		}
		if (!openslot) {
			Debug.Log ("No open slots");
			return;
		}
		int i = 0;
		while (open.Full) {
			i++;
			open = active.Berths [i];
		}
		Debug.Log ("Open Berth is #" + open.Designation);
		GameObject s = Instantiate (ShipPrefabs[drop.value]);
		s.GetComponent<NavMeshAgent>().Warp(new Vector3 (open.transform.position.x, .59f, open.transform.position.z));
		s.GetComponent<Ship>().faction = 0;
		if (s.GetComponent<Ship>().faction == 0)
			s.GetComponentInChildren<Renderer> ().material.color = Color.green;
		else {
			s.GetComponentInChildren<Renderer> ().material.color = Color.red;
		}
		Debug.Log ("Ship built");
	}

	public void Toggle(){
		SpaceYardUI.SetActive (!SpaceYardUI.activeSelf);
		player.InMenu = !player.InMenu;
	}


	// Use this for initialization
	void Start () {
		if (!ListBuilt) {
			ListBuilt = true;
			SpaceYardUI = GameObject.FindGameObjectWithTag ("SpaceYardUI");
			foreach (var val in System.Enum.GetValues(typeof(ShipPrefabTypes))) {
				ShipPrefabs.Add (Resources.Load<GameObject> (val.ToString()) as GameObject);
				Debug.Log ("Loaded " + val.ToString ());
			}
			drop = SpaceYardUI.GetComponentInChildren<Dropdown> ();
			List<string> shipStringTypes = new List<string> ();
			foreach (var val in System.Enum.GetValues(typeof(ShipPrefabTypes)))	{
				shipStringTypes.Add (val.ToString ());
			}
			int i = 0;
			foreach (Berth b in BerthsParent.GetComponentsInChildren<Berth>()) {
				Berths.Add (b);
				b.Designation = i;
				i++;
			}
			drop.ClearOptions ();
			drop.AddOptions (shipStringTypes);
			buildButton = SpaceYardUI.GetComponentInChildren<Button> ();
			buildButton.onClick.AddListener (Build);
			Toggle ();
		}


	}



	
	// Update is called once per frame
	void Update () {
		
	}
}
