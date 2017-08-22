using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType{
	LOAD,
	UNLOAD
}


public class ShipAction : MonoBehaviour {

	public RawResources targetResource;
	public int amount;

	City source;
	City target;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
