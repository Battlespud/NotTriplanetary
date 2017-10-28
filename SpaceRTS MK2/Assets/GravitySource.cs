using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitySource : MonoBehaviour {

public	List<Rigidbody> rbs = new List<Rigidbody>();
	const double G = 0.00000000006674;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		foreach (Rigidbody rb in rbs) {
			float r = Vector3.Distance (rb.transform.position, transform.position);
			float total = -5f;
			Vector3 force = (rb.transform.position - transform.position).normalized * total;
			rb.AddForce (force);


		}

	}




	void OnTriggerEnter(Collider col){
		if(col.GetComponent<Rigidbody>())
			rbs.Add(col.GetComponent<Rigidbody>());

	}

	void OnTriggerExit(Collider col){
		try{
			rbs.Remove(col.GetComponent<Rigidbody>());
		}
		catch{
		}
	}
}
