using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour {

	public City city;

	public RawResources input;
	Products output;
	public bool active = true;
	GameObject controller;

	// Use this for initialization
	void Start () {
	controller = GameObject.FindGameObjectWithTag ("Controller");
	controller.GetComponent<Clock> ().TurnEvent.AddListener (Manufacture);
	active = true;
	}
	
	// Update is called once per frame
	void Update () {

	}

	public void Manufacture(){
		output = (Products)input;
		float f = 5f;
		if (active) {
			if(city.TakeResource(input,f)){
				city.AddProduct (output, f);
			}
		}
	}

}
