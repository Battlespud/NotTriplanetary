using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Ship : MonoBehaviour {

	//this only handles things specific to this entity, like movement.

	public static GameObject Debris; //spawns on death.

	public ShipClass shipClass;

	//combat
	public int faction;
	public List<SpaceGun> Guns = new List<SpaceGun> ();
	//main

	public string ShipName;

	public float mass; //kg
	public float enginePower; //newtons
	public Vector3 VelocityUI;
	public float DotProdUI;
	public GeneralDirection dir;

	public NavMeshAgent Agent;
	// Use this for initialization
	void Start () {
		Debris = Resources.Load<GameObject> ("Debris") as GameObject;
		Agent = GetComponent<NavMeshAgent> ();
		foreach (SpaceGun sg in GetComponentsInChildren<SpaceGun>()) {
			Guns.Add (sg);
		}
		shipClass = gameObject.AddComponent<ShipClass> ();
	}
	
	// Update is called once per frame
	void Update () {
		Agent.acceleration = mass / enginePower;
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		dir = ShipClass.GetDirection (transform.position, transform.position - new Vector3(mousePos.x,transform.position.y,mousePos.z));
	}

	public void SpawnDebris(Vector3 source){
		Vector3 dir = transform.position - source;
		dir = dir.normalized;
		GameObject deb = Instantiate (Debris);
		deb.transform.position = transform.position+dir*.1f;
		Rigidbody rb = deb.GetComponent<Rigidbody> ();
		rb.AddForce (dir*125f);
		Destroy (gameObject);
	}

	public void IssueMovementCommand(Vector2 vec){
		StopAllCoroutines ();
		StartCoroutine("Movement", (new Vector3(vec.x,transform.position.y, vec.y)));
	}

	public IEnumerator Movement(Vector3 dest){
		float dotProd = 0f;
		Agent.SetDestination (dest);
		while (dotProd < .85) {
			Agent.acceleration = .2f * Agent.acceleration;
			dotProd = Vector3.Dot (transform.forward, (dest - transform.position).normalized);
			DotProdUI = dotProd;
			yield return null;
		}
	}
}
