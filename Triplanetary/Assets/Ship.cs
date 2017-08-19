using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour {

	public List<Player> Crew = new List<Player>();
	public bool Gravity;
	public AntiGravityGenerator GravGen;
	public GameObject Floor;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

	void FixedUpdate(){
		if (Gravity && GravGen.Active) {
			foreach (Player p in Crew) {
				p.rb.AddForce (Floor.transform.up * -12f);
				p.ship = this;
			}
		}
	}

	void LateUpdate(){
		foreach (Player p in Crew) {
			//nope
			Quaternion old = p.transform.rotation;
		//	p.transform.rotation = transform.rotation;
		//	p.transform.rotation = Quaternion.
		//	p.transform.localEulerAngles = new Vector3 (gameObject.transform.eulerAngles.x, p.transform.localEulerAngles.y, gameObject.transform.eulerAngles.z);
		}
	}

	void OnTriggerEnter(Collider col){
		if (col.GetComponent<Player> ()) {
			Crew.Add (col.GetComponent<Player> ());
	//		col.gameObject.transform.parent = Floor.transform;
		}
	}

	void OnTriggerExit(Collider col){
		if (col.GetComponent<Player> ()) {
			Crew.Remove (col.GetComponent<Player> ());
		//	col.gameObject.transform.parent = null;
		}
	}
}
