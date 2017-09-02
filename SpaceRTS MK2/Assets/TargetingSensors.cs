using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingSensors : MonoBehaviour {

	public float MaxRange; // unity units
	public SphereCollider targetingCollider;

	// Use this for initialization
	void Start () {
		targetingCollider = GetComponent<SphereCollider> ();
		targetingCollider.radius = MaxRange;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
