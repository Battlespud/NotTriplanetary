﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public string PlayerName = "Player";
	public bool human = true;
	Camera cam;
	public int faction = 0;

	public List<Ship> SelectedShips = new List<Ship>();

	// Use this for initialization
	void Start () {
		cam = Camera.main;

	}
	
	// Update is called once per frame
	void Update () {
		Vector3 mousePos = cam.ScreenToWorldPoint (Input.mousePosition);
		Ray clickRay = new Ray (mousePos,Vector3.down);

		if (Input.GetMouseButtonDown (0)) {
		//	Debug.Log("Trying to select");
			Debug.DrawRay (mousePos, Vector3.down, Color.white, 25f);
			RaycastHit hit;
			if (Physics.Raycast(clickRay, out hit,10000f)){
				Debug.Log (hit.collider.name);
				if (hit.collider.GetComponent<Ship> ()) {
					Ship hitShip = hit.collider.GetComponent<Ship>();
					if(hitShip.faction == faction)
					SelectShip (hitShip);
				}
				if (hit.collider.GetComponentInParent<SpaceYard> ()) {
					Debug.Log ("Spaceyard");
					SpaceYard s = hit.collider.GetComponentInParent<SpaceYard> ();
					s.Toggle ();
					SpaceYard.active = s;
				}
			}
		}
		if (Input.GetMouseButtonDown (1)) {
			MoveShips (new Vector2 (mousePos.x, mousePos.z));
		}
		if (Input.GetKeyDown (KeyCode.T)) {
			RaycastHit hit;
			if (Physics.Raycast(clickRay, out hit,10000f)){
				Debug.Log (hit.collider.name);
				if (hit.collider.GetComponent<Ship> ()) {
					Ship hitShip = hit.collider.GetComponent<Ship> ();
					if (hitShip.faction != faction)
						SelectedShips [0].shipClass.ActivateTractor (hitShip);
				} else {
					SelectedShips [0].shipClass.DeactivateTractor();
				}
			}
		}

		if (Input.GetKeyDown(KeyCode.B)) {
			foreach (Ship s in SelectedShips) {
				s.FireTorpedo ();
			}
		}

	}

	void SelectShip(Ship s){
		if (SelectedShips.Contains (s)) {
			SelectedShips.Remove (s);
			s.TogglePath ();
			s.GetComponentInChildren<Renderer> ().material.color = Color.green;
			Debug.Log ("Ship " + s.ShipName + " removed.");
		} else {
			SelectedShips.Add (s);
			s.TogglePath ();
			s.GetComponentInChildren<Renderer> ().material.color = Color.blue;
			Debug.Log ("Ship " + s.ShipName + " selected.");
		}
	}

	void MoveShips(Vector2 vec){
		foreach (Ship s in SelectedShips) {
			s.AddWaypoint (vec, Input.GetKey(KeyCode.LeftShift));
		}
	}
}
