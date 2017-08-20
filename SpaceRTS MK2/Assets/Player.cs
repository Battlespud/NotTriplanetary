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

	public bool InMenu = true;

	// Use this for initialization
	void Start () {
		Debug.Log (NameManager.names.Count + " names have been loaded");
		SpaceYard.player = this;
		Screens.ScreenPrefab = Resources.Load <GameObject>("ScreenPrefab") as GameObject;
		cam = Camera.main;
		SelectionUI = GameObject.FindGameObjectWithTag ("SelectionUI");
		selectText = SelectionUI.GetComponentInChildren<Text> ();
		Ship.OnDeath.AddListener (RemoveShip);
	}
	
	// Update is called once per frame
	void Update () {
		mousePos = cam.ScreenToWorldPoint (Input.mousePosition);
		Ray clickRay = new Ray (mousePos, Vector3.down);
		RaycastHit hit;
		Ship hitS = null;
		if(Physics.Raycast (clickRay, out hit, 10000f))
			hitS = hit.collider.GetComponent<Ship> ();
		if (!InMenu) {
			if (Input.GetMouseButtonDown (0)) {
				StartCoroutine ("SelectionBox");
				if (hit.collider != null) {
					if (hit.collider.GetComponentInParent<SpaceYard> ()) {
						Debug.Log ("Spaceyard");
						SpaceYard s = hit.collider.GetComponentInParent<SpaceYard> ();
						s.Toggle ();
						SpaceYard.active = s;
					}
				}
			}
			if (Input.GetMouseButtonDown (1)) {
				if (hitS) {
					if (hitS.faction != faction) {
						foreach (Ship s in SelectedShips) {
							foreach (SpaceGun a in s.Guns) {
								a.AssignTarget (hitS);
							}
						}
					}
				} else {
					MoveShips (new Vector2 (mousePos.x, mousePos.z));
				}
			}
			if (Input.GetKeyDown (KeyCode.T)) {
				if (hitS) {
					SelectedShips [0].shipClass.ActivateTractor (hitS);
				} else {
					SelectedShips [0].shipClass.DeactivateTractor ();
				}
			}
			if (Input.GetKeyDown (KeyCode.B)) {
				foreach (Ship s in SelectedShips) {
					s.StartCoroutine("TorpedoArm");
				}
			}
			if (Input.GetKeyDown (KeyCode.Backspace)) {
				foreach (Ship s in SelectedShips) {
					s.Agent.Stop ();
					s.Waypoints.Clear ();
				}
			}
		}
	}

	IEnumerator SelectionBox(){
		bool additive = false;
		if(Input.GetKey(KeyCode.LeftShift)){
			additive = true;
		}
		if (!additive)
			ClearSelection ();
		Vector3 origin = mousePos;
		Vector3 end = new Vector3 ();
		GameObject selectionOutline = new GameObject ();
		LineRenderer s = selectionOutline.AddComponent<LineRenderer> ();
		s.material = new Material(Shader.Find("Particles/Additive"));
		s.SetWidth (.2f, .2f);
		s.positionCount = 5;
		s.SetColors (Color.green, Color.green);
		Vector3[] pos = new Vector3[5];
		float height =2f;
		while (Input.GetMouseButton (0)) {
			end = mousePos;
			pos = new Vector3[5]{new Vector3(origin.x,height,origin.z), new Vector3 (end.x, height, origin.z), new Vector3(end.x,height,end.z), new Vector3 (origin.x, height, end.z), new Vector3(origin.x,height,origin.z) };  
			s.SetPositions (pos);
			yield return null;
		}
		GameObject b = new GameObject ();
		SelectionProxy proxy = b.AddComponent<SelectionProxy> ();
		proxy.player = this;
		Collider box = b.AddComponent<BoxCollider> ();
		box.isTrigger = true;
		foreach (Vector3 v in pos) {
	//		box.bounds.Encapsulate (v);  //Why doesnt this work tho. seems to have no effect.
		}
		box.transform.position = new Vector3 (origin.x + ((end.x - origin.x) * .5f), 1f, origin.z + ((end.z - origin.z) * .5f));
		box.transform.localScale = new Vector3 (Mathf.Abs (end.x - origin.x), 5f, Mathf.Abs (end.z - origin.z));


		Destroy (selectionOutline);

		float timer = .05f;
		while (timer > 0f) { //Why is this needed?
			timer -= Time.deltaTime;
			yield return null;
		}
		if (proxy.Contents.Count < 1 && !additive)
			ClearSelection ();
		Destroy (b);


	}

	public void SelectShip(Ship s){
		if (SelectedShips.Contains (s)) {
			SelectedShips.Remove (s);
			s.TogglePath ();
			foreach (Renderer r in s.rens) {
				r.material.SetColor ("_EmissionColor", Color.blue);
			}
			s.render.material.color = Color.blue;
		//	Debug.Log ("Ship " + s.ShipName + " removed.");
		} else {
			SelectedShips.Add (s);
			s.TogglePath ();
			if(s.render)
				s.render.material.color = Color.green;
			foreach (Renderer r in s.rens) {
				if(r)
				r.material.SetColor ("_EmissionColor", Color.green);
			}
		//	Debug.Log ("Ship " + s.ShipName + " selected.");
		}
		selectText.text = "Selected: ";
		foreach (Ship c in SelectedShips) {
			selectText.text += "| " + c.name + " ";
		}
	}

	public void ClearSelection(){
		List<Ship> mirror = new List<Ship> ();
		mirror.AddRange (SelectedShips);
		foreach (Ship s in mirror) {
			SelectShip (s);
		}
	}

	public void RemoveShip(Ship s){
		if(SelectedShips.Contains(s)){
			SelectShip (s);
		}
	}

	void MoveShips(Vector2 vec){
		float offset = .75f;
		int row = 0;
		int mMow = 3;
		int counter = 0;
		float invert = 1;
		foreach (Ship s in SelectedShips) {
			float sizeAdjust = 0f;
			if (s.BaseType == ShipPrefabTypes.CV || s.BaseType == ShipPrefabTypes.DN) {
				sizeAdjust += 1f;
			}
			Vector3 local = new Vector2 (vec.x+(counter*invert*(offset+sizeAdjust)),  vec.y+(row*-1f*(offset+sizeAdjust)));
			s.AddWaypoint (local, Input.GetKey(KeyCode.LeftShift));
			counter++;
			if (counter > mMow) {
				row++;
				counter = 0;
			}
			invert *= -1;
		}
	}
}
