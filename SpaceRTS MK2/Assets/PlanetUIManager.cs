using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlanetUIManager : MonoBehaviour
{

	public static GameObject RegionButton;

	public static Vector3 StartingPosition = new Vector3(55f,-860f,0f); //topleft
	
	public Planet p;
	public Empire viewer;

	public Text Description;


	void Awake()
	{
		if (!RegionButton)
		RegionButton = Resources.Load<GameObject>("RegionButton") as GameObject;
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
