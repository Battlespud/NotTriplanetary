using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyTester : MonoBehaviour {


	public bool ApplyImpulse;

	public float ImpulseForce= 25f;

	Rigidbody rb;


	public Vector3 Velocity;
	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (ApplyImpulse || Input.GetKeyDown(KeyCode.Space)) {
			ApplyImpulse = false;
			rb.AddForce (transform.forward * ImpulseForce);
		}
		Velocity = rb.velocity;
	}
}
