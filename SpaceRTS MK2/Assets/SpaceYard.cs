using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceYard : MonoBehaviour {

	public int faction;
	public GameObject shipPrefab;

	public void Build(int fac){
		GameObject s = Instantiate (shipPrefab);
		s.transform.position = new Vector3 (transform.position.x, s.transform.position.y, transform.position.z);
		s.GetComponent<Ship>().faction = fac;
		if (fac == 0)
			s.GetComponentInChildren<Renderer> ().material.color = Color.green;
		else {
			s.GetComponentInChildren<Renderer> ().material.color = Color.red;
		}
		Debug.Log ("Ship built for #" + fac);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
