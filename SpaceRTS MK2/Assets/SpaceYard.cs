using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceYard : MonoBehaviour {

	public int faction;
	public static List<GameObject> ShipPrefabs = new List<GameObject>();
	static bool ListBuilt = false;

	//UI
	public static GameObject SpaceYardUI;
	public static SpaceYard active;
	public static Dropdown drop;
	public static Button buildButton;

	public static void Build(){
		GameObject s = Instantiate (ShipPrefabs[drop.value]);
		s.transform.position = new Vector3 (active.transform.position.x, 1f, active.transform.position.z);
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
