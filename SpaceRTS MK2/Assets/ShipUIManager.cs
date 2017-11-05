using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipUIManager : MonoBehaviour {

	public StrategicShip S;


	//Armor
	public Text ArmorText;




	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AssignShip(StrategicShip s){
		S = s;
		UpdateUI ();
	}

	void UpdateUI(){
		ArmorText.text = S.ArmorString;

	}


}
