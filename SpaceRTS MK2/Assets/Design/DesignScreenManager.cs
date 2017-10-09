using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class DesignScreenManager : MonoBehaviour {

	public List<ShipTemplate> ShipDesigns = new List<ShipTemplate>();

	//references
	public InputField DesignName;
	public Text ArmorText;
	public Button SaveButton;
	public Button ExitButton;
	public Dropdown HullDesignation;
	public Text Cost;
	public InputField DeploymentTime;
	public Text Requirements;


	public List<ShipComponents> Components = new List<ShipComponents> ();
	public List<ShipComponents> AddedComponents = new List<ShipComponents>();
	public List<Engine> Engines = new List<Engine>();
	public Dictionary<ShipComponents, int> ComponentNumbers = new Dictionary<ShipComponents, int>();

	//Calculations
	public int Tonnage; //in kilotons



	//Info
	public int Armor = 1;



	public void AddArmor(){
		Armor++;
		ArmorText.text = Armor.ToString ();
		OnChange ();
	}
	public void ReduceArmor(){
		if (Armor > 1) {
			Armor--;
			ArmorText.text = Armor.ToString ();
			OnChange ();
		}
	}

	// Use this for initialization
	void Start () {
		SetupScreen ();
	}

	void SetupScreen(){
		Debug.Log ("Setting up design screen");
		HullDesignation.ClearOptions ();
		HullDesignation.AddOptions (HullDes.HullTypes);
		Armor = 1;
		ArmorText.text = Armor.ToString ();
		DesignName.text = "";

		RunTest ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	bool RequirementsMet = false;
	IEnumerator CheckRequirements(){
		RequirementsMet = true;
		string OutstandingRequirements = "";
		if (ShipDesign.DesignDictionary.ContainsKey (DesignName.text)) {
			RequirementsMet = false;
			OutstandingRequirements += DesignName.text + " is already being used as a class name.\n";
		}
		if (Engines.Count < 1) {
			RequirementsMet = false;
			OutstandingRequirements += "Valid ship designs must contain at least one engine.\n";
		}
		if (Armor <= 0) {
			RequirementsMet = false;
			OutstandingRequirements += "Error, please increase Armor to a positive value of at least 1.";
		}
		Requirements.text = OutstandingRequirements;
		yield return null;
		}


	void OnChange(){
		StartCoroutine (CheckRequirements());
	}

	void RunTest(){
		Debug.Log ("Outputting test design " + DesignName.text);
		Armor = 5;
		DesignName.text = "Stalingrad";

		SaveAsDesign ();
	}

	void SaveAsDesign(){
		OnChange ();
		if (!RequirementsMet) {
		//	return;
		}
		ShipDesign design = new ShipDesign(DesignName.text);
		design.HullDesignation = HullDes.DesDictionary[HullDesignation.options [HullDesignation.value].text];
		design.ArmorLayers = Armor;
		design.Output ();
	}
}
