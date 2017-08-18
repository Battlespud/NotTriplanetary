using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntiGravityGenerator : MonoBehaviour {

	public bool Active = true;
	PowerEndpoint power;


	// Use this for initialization
	void Start () {
		power = GetComponent<PowerEndpoint> ();	
	}
	
	// Update is called once per frame
	void Update () {
		Active = power.Recieving;
	}
}
