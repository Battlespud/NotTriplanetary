using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class Ship : MonoBehaviour {

	//this only handles things specific to this entity, like movement.

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
		Agent = GetComponent<NavMeshAgent> ();
		foreach (SpaceGun sg in GetComponentsInChildren<SpaceGun>()) {
			Guns.Add (sg);
		}
	}
	
	// Update is called once per frame
	void Update () {
		Agent.acceleration = mass / enginePower;
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		dir = ShipClass.GetDirection (transform.position, new Vector3(mousePos.x,transform.position.y,mousePos.z));
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
