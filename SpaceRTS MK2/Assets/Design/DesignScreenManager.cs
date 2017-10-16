﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Threading;
using System.Linq;


public enum ArmorTypes{
	STEEL = 1,
	SMARTSTEEL = 3,
	DURANIUM=5,
	HIGHDENSITYDURANIUM=7,
	COMPOSITE=8,
	CERAMICCOMPOSITE=10,
	LAMINATECOMPOSITE=12,
	COMPRESSEDCARBON=15,
	BIPHASECARBIDE=18,
	CRYSTALLINECOMPOSITE=21,
	SUPERDENSE=25,
	BONDEDSUPERDENSE=30
}

public class DesignScreenManager : MonoBehaviour {

	static Color highlight = new Color (181f,242f,242f);
	static Color pressed = new Color (36f,212f,205f);

	//armor width = 1.2*(Mass/50)^2/3

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
	public Text CrewReqText;
	public Transform ContentParentScrollview;
	public GameObject ButtonPrefab;
	public Text MassText;
	public ScrollRect scrollview;

	public bool LoadAllComponents = false;

	public bool HideObsolete = false;
	public List<ShipComponents> Components = new List<ShipComponents> ();
	public Dictionary<ShipComponents, int> AddedComponents = new Dictionary<ShipComponents, int>();
	public ShipComponents EngineDesign; //only one type of engine permitted
	public int EngineCount;
	public Dictionary<ShipComponents, int> ComponentNumbers = new Dictionary<ShipComponents, int>();

	//Calculations
	public int Tonnage; //in kilotons



	//Info
	public int ArmorThickness = 1;
	public int ArmorLength = 1;
	public int ReqCrew;
	public int Mass;
	ArmorTypes ArmorType = ArmorTypes.DURANIUM;

	public int LifeSupport; //how many crew the current loadout supports;
	public int Quarters;   //how much space for crew there is with current loadout.


	public void PopulateComponentList(){
		Components.Clear();
		if (HideObsolete) {
			foreach (ShipComponents c in ShipComponents.DesignedComponents) {
				if (!c.Obsolete)
					Components.Add (c);
			}
		} else {
			foreach (ShipComponents c in ShipComponents.DesignedComponents) {
					Components.Add (c);
			}
		}
		int yOff = -30;
		int interval = 0;
		foreach (ShipComponents c in Components) {
			GameObject d = Instantiate (ButtonPrefab) as GameObject;
			d.transform.SetParent (ContentParentScrollview);
			d.transform.localPosition = new Vector3 (0f, interval * yOff, 0f);
			d.transform.rotation.eulerAngles.Set(0f,0f,0f);
			d.transform.localScale = new Vector3 (.6f, 1f, 1f);
			d.GetComponentInChildren<Text> ().text = c.GenerateDesignString ();
			ComponentPassThrough pass = d.AddComponent<ComponentPassThrough> ();
			pass.component = c;
			pass.Manager = this;
			Button b = d.GetComponent<Button> ();
			b.onClick.AddListener (pass.AddShipComponent);
			ColorBlock block = b.colors;
			block.highlightedColor = highlight;
			block.pressedColor = pressed;
			b.colors = block;
			interval++;
		}
	}

	public void AddComponent(ShipComponents comp){
		Debug.Log("Adding " + comp.name);
		if(AddedComponents.ContainsKey(comp)){
			AddedComponents[comp] += 1;
		}
		else{
			AddedComponents.Add (comp, 1);
		}
		OnChange ();
	}

	public void AddEngine(ShipComponents engine){
		Debug.Log("Adding " + engine.name);
		if (EngineDesign == null) {
			EngineDesign = engine;
		}
		if(EngineDesign != engine){
			EngineCount = 1;
			if (AddedComponents.ContainsKey (EngineDesign)) {
				AddedComponents.Remove (EngineDesign);
			}
			Debug.Log("Setting new engine pattern");
			EngineDesign = engine;
		}
		else{
			EngineCount++;
		}
		if(AddedComponents.ContainsKey(EngineDesign)){
			AddedComponents [EngineDesign] = EngineCount;
		}
		else
		{
			AddedComponents.Add (EngineDesign, EngineCount);
		}
		OnChange ();
	}

	public void AddArmor(){
		ArmorThickness++;
		OnChange ();
	}

	public void ReduceArmor(){
		if (ArmorThickness > 1) {
			ArmorThickness--;
			OnChange ();
		}
	}

	// Use this for initialization
	void Start () {
		ButtonPrefab = Resources.Load<GameObject> ("ButtonWide") as GameObject;
		SetupScreen ();
	}

	void SetupScreen(){
		Debug.Log ("Setting up design screen");

		LoadAllComponents = true;
		PopulateComponentList ();
		HullDesignation.ClearOptions ();
		HullDesignation.AddOptions (HullDes.HullTypes);
		ArmorThickness = 1;
		ArmorText.text = ArmorThickness.ToString ();
		DesignName.text = "";
		OnChange ();
	//	RunTest ();
	}
	
	// Update is called once per frame
	void Update () {
		if (LoadAllComponents) {
			LoadAllComponents = false;
			ShipComponents.LoadAllComponents ();
			PopulateComponentList ();
		}
	}

	bool RequirementsMet = false;
	IEnumerator CheckRequirements(){
		string OutstandingRequirements = "";
		RequirementsMet = false;
		foreach(ShipComponents c in AddedComponents.Keys.ToList()){
			if (c.control || c.flagControl) {
				RequirementsMet = true;
				break;
			}
		}
		if(!RequirementsMet)
			OutstandingRequirements += "Missing Bridge\n";
		if (ShipDesign.DesignDictionary.ContainsKey (DesignName.text)) {
			RequirementsMet = false;
			OutstandingRequirements += DesignName.text + " is already being used as a class name.\n";
		}
		if (EngineCount < 1) {
			RequirementsMet = false;
			OutstandingRequirements += "Valid ship designs must contain at least one engine.\n";
		}
		if (ArmorThickness <= 0) {
			RequirementsMet = false;
			OutstandingRequirements += "Error, please change Armor to a positive value of at least 1.\n";
		}
		if (Quarters < ReqCrew) {
			RequirementsMet = false;
			OutstandingRequirements += string.Format("Quarters for only {0} of the {1} required crewmembers is present.\n",Quarters,ReqCrew);
		}
		if (LifeSupport < ReqCrew) {
			RequirementsMet = false;
			OutstandingRequirements += string.Format("Life Support for only {0} of the {1} required crewmembers is present.\n",LifeSupport,ReqCrew);
		}
		yield return Ninja.JumpToUnity;
		Requirements.text = OutstandingRequirements;
	}

	IEnumerator CheckCrewRequirements(){
		int quarter = 0;
		int lifesupport = 0;
		int minimum = 0;
		foreach (ShipComponents c in ShipComponents.DesignedComponents) {
			int number;
			if (AddedComponents.TryGetValue (c, out number)) {
				minimum += (c.CrewRequired * number );
				quarter += (c.quarters * number);
				lifesupport += (c.lifeSupport * number);
			}
		}
		LifeSupport = lifesupport;
		Quarters = quarter;
		ReqCrew = minimum;
		yield return Ninja.JumpToUnity;
		CrewReqText.text = string.Format ("Crew Required: {0}", ReqCrew);
	}


	int ArmorMass(){
		return (50 / (int)ArmorType);
	}

	IEnumerator CalculateMass(){
		int M = 0;
		foreach (ShipComponents c in ShipComponents.DesignedComponents) {
			int number;
			if (AddedComponents.TryGetValue (c, out number)) {
				M += (c.Mass * number );
			}
		}
		M += ArmorMass () * (ArmorLength * (ArmorThickness + (int)Mathf.Pow(ArmorThickness, .35f)));
		Mass = M;
		yield return Ninja.JumpToUnity;
		MassText.text = "Mass: " + M + "kt";
	}

	void OnChange(){
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CheckCrewRequirements());
		CalculateArmorWidth ();
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CalculateMass());
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CheckRequirements());
		ArmorText.text = ArmorLength.ToString() + " x " +  ArmorThickness.ToString ();
	}

	void CalculateArmorWidth(){
		int hs = Mass / 50;
		ArmorLength =(int)(1 + 1.1*(Mathf.Pow (hs, .66f)));
	}

	void RunTest(){
		Debug.Log ("Outputting test design " + DesignName.text);
		DesignName.text = "Test ship";

		SaveAsDesign ();
	}

	public void SaveAsDesign(){
		OnChange ();
		if (!RequirementsMet) {
			Debug.Log ("Requirements not met. DEBUG MODE ENABLED, output will continue anyway.");
		//	return;
		}
		ShipDesign design = new ShipDesign(DesignName.text);
		design.HullDesignation = HullDes.DesDictionary[HullDesignation.options [HullDesignation.value].text];
		design.ArmorLayers = ArmorThickness;
		design.ArmorLength = ArmorLength;
		design.CrewMin = ReqCrew;
		design.CrewBerths = Quarters;
		design.LifeSupport = LifeSupport;
		design.mass = Mass;
			foreach (ShipComponents c in ShipComponents.DesignedComponents) {
				int number;
				if (AddedComponents.TryGetValue (c, out number)) {
				for (int i = 0; i < number; i++) {
					ShipComponents comp = new ShipComponents();
					comp = c.CloneProperties ();
					Debug.Log ("Original " + c.name);
					Debug.Log ("Clone " + comp.name);
					design.Components.Add (comp);
					}
				}
			}
		design.SetupDAC ();
		design.Output ();
	}


}
