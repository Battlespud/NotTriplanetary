using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class ElectricalComponent : MonoBehaviour {

	public float MaxHeat = 100f;
	public float heat = 0f;

	public float SpecificHeat;
	//we assume mass is always equal to 1.

	public bool Damaged = false;
	public CurrentType currentType;
	public CurrentType reqCurrentType;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
