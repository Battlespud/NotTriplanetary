using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colony : MonoBehaviour {

	static float growth = .025f; //per minute

	public Planet planet;
	public int population;
	public string ColonyName;
	public int faction;

	public float morale = 0f;
	public float taxRate = .08f;

	public int manpower;

	public string Description;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
