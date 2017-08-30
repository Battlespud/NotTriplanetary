using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Berth : MonoBehaviour {

	public bool Full = false;

	public List<ShipAbstract> ShipList = new List<ShipAbstract>();
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
		if (col.gameObject.GetComponent<ShipAbstract> ()) {
			ShipList.Add (col.GetComponent<ShipAbstract> ());
		}
	}

	void OnTriggerExit(Collider col){
		if (col.gameObject.GetComponent<ShipAbstract> ()) {
			ShipList.Remove (col.GetComponent<ShipAbstract> ());
		}
	}

}
