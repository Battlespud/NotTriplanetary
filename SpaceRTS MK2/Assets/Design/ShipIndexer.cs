using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipIndexer : MonoBehaviour {

	public GameObject Targeting;
	public GameObject Main;
	public ShipClass ShipClass;
	public Ship ship;

	// Use this for initialization
	void Start () {
		ShipClass = GetComponent<ShipClass> ();
		ship = GetComponent<Ship> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
