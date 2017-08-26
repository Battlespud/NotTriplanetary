using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EscapePod : MonoBehaviour {


	public int Survivors;

	float lifespan = 240f;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter(Collider col){
		if (col.gameObject.GetComponent<Ship> ()) {
			col.GetComponent<Ship> ().RescueSurvivors (this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		lifespan -= Time.deltaTime;
		if (lifespan <= 0f)
			Destroy (gameObject);
	}
}
