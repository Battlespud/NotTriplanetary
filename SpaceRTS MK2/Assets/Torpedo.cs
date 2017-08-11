using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour {


	public List<Ship> InBlastZone = new List<Ship>();

	public float Force = 125f;

	// Use this for initialization
	void Start () {
		
	}

	void OnTriggerEnter(Collider col){
		if(col.GetComponent<Ship>())
			InBlastZone.Add(col.GetComponent<Ship>());
	}

	void OnTriggerExit(Collider col){
		if(col.GetComponent<Ship>())
			InBlastZone.Remove(col.GetComponent<Ship>());
	}

	void OnCollisionEnter(Collision col){
		if (col.collider.GetComponent<Ship> ())
			Detonate ();
	}

	public void Detonate(){
		foreach (Ship s in InBlastZone) {
			s.shipClass.Damage (5f, transform.position);
			s.shipClass.Damage (5f, transform.position);
			s.shipClass.Damage (5f, transform.position);

		}
		Collider[] col = Physics.OverlapSphere (transform.position, 15f);
		foreach (Collider hit in col)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();

			if (rb)
				rb.AddExplosionForce(Force, transform.position, 15f, 0f);
		}
		Destroy (gameObject);

	}

	// Update is called once per frame
	void Update () {
		
	}
}
