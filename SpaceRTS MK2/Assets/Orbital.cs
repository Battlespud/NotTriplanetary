using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbital : MonoBehaviour {


	Rigidbody rb;

	public float magnitude = 100f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.AddForce (transform.forward * magnitude);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
