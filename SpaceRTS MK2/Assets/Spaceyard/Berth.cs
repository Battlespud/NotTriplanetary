using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berth : MonoBehaviour {

	public bool Full = false;

	public List<Ship> ShipList = new List<Ship>();
	public int Designation;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Full = false;
		if (ShipList.Count != 0)
			Full = true;
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.GetComponent<Ship> ()) {
			ShipList.Add (col.GetComponent<Ship> ());
		}
	}

	void OnTriggerExit(Collider col){
		if (col.gameObject.GetComponent<Ship> ()) {
			ShipList.Remove (col.GetComponent<Ship> ());
		}
	}
}
