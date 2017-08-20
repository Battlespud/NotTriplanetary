using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour {

	public City city;

	public RawResources input;
	Products output;
	public bool active = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		output = (Products)input;
		float f = 5f*Time.deltaTime;
		if (active) {
			if(city.UseResources(input,f)){
				city.AddProduct (output, f);
			}

		}
	}
}
