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

	public List<ShipComponents> Components;
	public List<ShipComponents> AddedComponents;
	public Dictionary<ShipComponents, int> ComponentNumbers;

	//Calculations
	public int Tonnage; //in kilotons



	//Info
	public int Armor = 1;



	public void AddArmor(){
		Armor++;
		ArmorText.text = Armor.ToString ();
	}
	public void ReduceArmor(){
		if (Armor > 1) {
			Armor--;
			ArmorText.text = Armor.ToString ();
		}
	}

	// Use this for initialization
	void Start () {
		SetupScreen ();
	}

	void SetupScreen(){
		HullDesignation.ClearOptions ();
		HullDesignation.AddOptions (HullDes.HullTypes);
		Armor = 1;
		ArmorText.text = Armor.ToString ();
		DesignName.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void SaveAsDesign(){
		ShipDesign design = new ShipDesign();
		design.DesignName = DesignName.text;
		design.ArmorLayers = Armor;

	}
}
