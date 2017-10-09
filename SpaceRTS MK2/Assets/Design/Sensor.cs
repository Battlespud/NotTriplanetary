using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sensor : MonoBehaviour {
	//Complete sensor package
	//Passive includes both EM and Thermal

	public bool ActiveEnabled;
	public float ActiveRange;

	public float PassiveSensitivity;
	public float PassiveRange;

	public bool PassiveCanDetect(float emission){
		if(emission < PassiveSensitivity)
		return true;
			return false;
	}



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
