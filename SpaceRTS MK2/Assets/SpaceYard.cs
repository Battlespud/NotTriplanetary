using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.Events;


public class SpaceYard : MonoBehaviour, IContext {



	public List<UnityAction> ContextActions()
	{
		List<UnityAction> actions = new List<UnityAction>(){ new UnityAction (BuildDEF), new UnityAction(BuildDD), new UnityAction(BuildCS), new UnityAction(BuildCV), new UnityAction(BuildDN), new UnityAction(BuildFR), new UnityAction(BuildCT)};
		return actions;
	}

	public GameObject getGameObject(){
		return gameObject;
	}

	public static Player player;
	public int faction;


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
		GameObject s = Instantiate (ShipAbstract.ShipPrefabs[drop.value]);
		ShipAbstract sb = s.GetComponent < ShipAbstract> ();
		s.GetComponent<NavMeshAgent>().Warp(new Vector3 (open.transform.position.x, .59f, open.transform.position.z));
		s.transform.rotation = Quaternion.Inverse(open.transform.rotation);
		sb.faction = FAC.PLAYER;
	//	sb.Start ();
		Debug.Log ("Ship built");
	}



	public void Build(ShipPrefabTypes typ){
		Berth open = Berths[0];
		bool openslot = false;
		foreach (Berth b in Berths) {
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
			open = Berths [i];
		}
		Debug.Log ("Open Berth is #" + open.Designation);
		GameObject s = Instantiate (ShipAbstract.ShipTypeDict[typ]);
		s.GetComponent<NavMeshAgent>().Warp(new Vector3 (open.transform.position.x, .59f, open.transform.position.z));
		s.transform.rotation = Quaternion.Inverse(open.transform.rotation);
		s.GetComponent<ShipAbstract>().faction = FAC.PLAYER;
		s.GetComponent<ShipAbstract> ().RegenColors ();

		Debug.Log ("Ship built local");
	}

	void BuildDD(){
		Build (ShipPrefabTypes.DD);
	}
	void BuildDEF(){
		Build (ShipPrefabTypes.DEF);
	}
	void BuildCS(){
		Build (ShipPrefabTypes.CS);
	}
	void BuildCV(){
		Build (ShipPrefabTypes.CV);
	}
	void BuildDN(){
		Build (ShipPrefabTypes.DN);
	}
	void BuildFR(){
		Build (ShipPrefabTypes.FR);
	}
	void BuildCT(){
		Build (ShipPrefabTypes.CT);
	}

	public void Toggle(){
		SpaceYardUI.SetActive (!SpaceYardUI.activeSelf);
		player.InMenu = !player.InMenu;
	}


	// Use this for initialization
	void Start () {
			SpaceYardUI = GameObject.FindGameObjectWithTag ("SpaceYardUI");
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





	
	// Update is called once per frame
	void Update () {
		
	}
}
