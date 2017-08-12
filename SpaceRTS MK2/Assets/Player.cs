using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public string PlayerName = "Player";
	public bool human = true;
	Camera cam;
	public int faction = 0;
	GameObject SelectionUI;
	Text selectText;
	public List<Ship> SelectedShips = new List<Ship>();
	Vector3 mousePos;

	// Use this for initialization
	void Start () {
		Screens.ScreenPrefab = Resources.Load <GameObject>("ScreenPrefab") as GameObject;
		cam = Camera.main;
		SelectionUI = GameObject.FindGameObjectWithTag ("SelectionUI");
		selectText = SelectionUI.GetComponentInChildren<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
		mousePos = cam.ScreenToWorldPoint (Input.mousePosition);
		Ray clickRay = new Ray (mousePos,Vector3.down);
		if (Input.GetMouseButtonDown (0)) {
			StartCoroutine ("SelectionBox");
		}
		if (Input.GetMouseButtonDown (0)) {
		//	Debug.Log("Trying to select");
			Debug.DrawRay (mousePos, Vector3.down, Color.white, 25f);
			RaycastHit hit;
			if (Physics.Raycast (clickRay, out hit, 10000f)) {
				Debug.Log (hit.collider.name);
				if (hit.collider.GetComponent<Ship> ()) {
					Ship hitShip = hit.collider.GetComponent<Ship> ();
					if (hitShip.faction == faction)
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

	IEnumerator SelectionBox(){
		Vector3 origin = mousePos;
		Vector3 end;
		GameObject selectionOutline = new GameObject ();
		LineRenderer s = selectionOutline.AddComponent<LineRenderer> ();
		s.material = new Material(Shader.Find("Particles/Additive"));
		s.SetWidth (.05f, .05f);
		s.positionCount = 5;
		s.SetColors (Color.green, Color.green);
		while (Input.GetMouseButton (0)) {
			end = mousePos;
			Vector3[] pos = new Vector3[5]{new Vector3(origin.x,1f,origin.z), new Vector3 (end.x, 1f, origin.z), new Vector3(end.x,1f,end.z), new Vector3 (origin.x, 1f, end.z), new Vector3(origin.x,1f,origin.z) };  
			s.SetPositions (pos);
			yield return null;
		}
		Destroy (selectionOutline);
	}

	void SelectShip(Ship s){

		if (SelectedShips.Contains (s)) {
			SelectedShips.Remove (s);
			s.TogglePath ();
			s.render.material.color = Color.green;
			Debug.Log ("Ship " + s.ShipName + " removed.");
		} else {
			SelectedShips.Add (s);
			s.TogglePath ();
			s.render.material.color = Color.blue;
			Debug.Log ("Ship " + s.ShipName + " selected.");
		}
		selectText.text = "Selected: ";
		foreach (Ship c in SelectedShips) {
			selectText.text += "| " + c.name + " ";
		}
	}

	void MoveShips(Vector2 vec){
		foreach (Ship s in SelectedShips) {
			s.AddWaypoint (vec, Input.GetKey(KeyCode.LeftShift));
		}
	}
}
