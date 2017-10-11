using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class MissileBehaviourTest : MonoBehaviour {

	public GameObject Target;
	public NavMeshAgent Agent;

	public bool Proceed = false;

	// Use this for initialization
	void Start () {
		Proceed = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Proceed && Target)
		Agent.SetDestination (Target.transform.position);

	}
}
