using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionProxy : MonoBehaviour {

	public Player player;

	// Use this for initialization
	void Awake () {
		Contents = new List<Ship> ();
	}

	public List<Ship> Contents;

	public void OnTriggerEnter(Collider col){
		Ship s = col.GetComponent<Ship> ();
		if (s) {
			if (!player.SelectedShips.Contains (s) && s.faction == player.faction) {
				player.SelectShip (s);
				Contents.Add (s);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}
}
