using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SlipwayButtonUIManager : MonoBehaviour {

	public Slipway Slip;
	public Text Name;
	public string Index;
	public Text InProgress;
	public Button button;
	public StrategicShipyardUIManager Manager;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AssignSlip(Slipway S, string n){
		Slip = S;
		Index = n;
		Name.text =  "#"+n;
		UpdateUI();
	}

	public void UpdateUI(){

		if(Slip.InUse){
			InProgress.text = "In Progress: " + Slip.TurnsToCompletion; 
		}
		else{
			InProgress.text = "Unassigned";
		}
	}

	public void Selected(){
		Manager.SelectSlipway(this);
	}
}
