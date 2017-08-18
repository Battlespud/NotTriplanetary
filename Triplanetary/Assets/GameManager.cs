using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


	public GameObject PlayerPrefab;

	public Transform SpawnPoint;


	void Awake(){
		PlayerPrefab = Resources.Load<GameObject> ("Player") as GameObject;

	}

	// Use this for initialization
	void Start () {
		GameObject g = Instantiate (PlayerPrefab);
		g.transform.position = SpawnPoint.position;
		g.transform.rotation = SpawnPoint.rotation;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
