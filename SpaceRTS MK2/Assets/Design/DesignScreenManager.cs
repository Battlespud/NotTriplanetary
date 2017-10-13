using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Threading;
using System.Linq;

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
	public Text CrewReqText;
	public Transform ContentParentScrollview;
	public GameObject ButtonPrefab;
	public Text MassText;

	public bool HideObsolete = false;
	public List<ShipComponents> Components = new List<ShipComponents> ();
	public Dictionary<ShipComponents, int> AddedComponents = new Dictionary<ShipComponents, int>();
	public List<Engine> Engines = new List<Engine>();
	public Dictionary<ShipComponents, int> ComponentNumbers = new Dictionary<ShipComponents, int>();

	//Calculations
	public int Tonnage; //in kilotons



	//Info
	public int Armor = 1;
	public int ReqCrew;

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
			d.transform.localScale = new Vector3 (.65f, 1f, 1f);
			d.GetComponentInChildren<Text> ().text = c.GenerateDesignString ();
			ComponentPassThrough pass = d.AddComponent<ComponentPassThrough> ();
			pass.component = c;
			pass.Manager = this;
			d.GetComponent<Button> ().onClick.AddListener (pass.AddShipComponent);
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
		ButtonPrefab = Resources.Load<GameObject> ("ButtonWide") as GameObject;
		SetupScreen ();
	}

	void SetupScreen(){
		Debug.Log ("Setting up design screen");

		//Design test component
		ShipComponents c = new ShipComponents();
		c.name = "Test";
		c.Mass = 500;
		c.CrewRequired = 200;
		ShipComponents b = new ShipComponents();
		b.name = "Test-B";
		b.Mass = 350;
		b.CrewRequired = 50;
		ShipComponents a = new ShipComponents();
		a.name = "Bridge";
		a.Mass = 50;
		a.CrewRequired = 15;
		a.control = true;
		ShipComponents.DesignedComponents.Add (c);
		ShipComponents.DesignedComponents.Add (b);
		ShipComponents.DesignedComponents.Add (a);
		PopulateComponentList ();
		HullDesignation.ClearOptions ();
		HullDesignation.AddOptions (HullDes.HullTypes);
		Armor = 1;
		ArmorText.text = Armor.ToString ();
		DesignName.text = "";
		OnChange ();
		RunTest ();
	}
	
	// Update is called once per frame
	void Update () {
		
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
		if (Engines.Count < 1) {
			RequirementsMet = false;
			OutstandingRequirements += "Valid ship designs must contain at least one engine.\n";
		}
		if (Armor <= 0) {
			RequirementsMet = false;
			OutstandingRequirements += "Error, please change Armor to a positive value of at least 1.\n";
		}
		if (Quarters < ReqCrew) {
			RequirementsMet = false;
			OutstandingRequirements += string.Format("Quarters for only {0} of the {1} required crewmembers is present.\n",Quarters,ReqCrew);
		}
		if (LifeSupport < ReqCrew) {
			RequirementsMet = false;
			OutstandingRequirements += string.Format("Life Support for only {0} of the {1} required crewmembers is present.\n",Quarters,ReqCrew);
		}
		yield return Ninja.JumpToUnity;
		Requirements.text = OutstandingRequirements;
		}

	IEnumerator CheckCrewRequirements(){
		int quarter = 0;
		int minimum = 0;
		foreach (ShipComponents c in ShipComponents.DesignedComponents) {
			int number;
			if (AddedComponents.TryGetValue (c, out number)) {
				minimum += (c.CrewRequired * number );
				quarter += (c.quarters * number);
			}
		}
		Quarters = quarter;
		ReqCrew = minimum;
		yield return Ninja.JumpToUnity;
		CrewReqText.text = string.Format ("Crew Required: {0}", ReqCrew);
	}

	IEnumerator CalculateMass(){
		float M = 0f;
		foreach (ShipComponents c in ShipComponents.DesignedComponents) {
			int number;
			if (AddedComponents.TryGetValue (c, out number)) {
				M += (c.Mass * number );
			}
		}
		yield return Ninja.JumpToUnity;
		MassText.text = M + "kt";
	}

	void OnChange(){
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CheckCrewRequirements());
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CalculateMass());

		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CheckRequirements());
		}

	void RunTest(){
		Debug.Log ("Outputting test design " + DesignName.text);
		DesignName.text = "Test ship";

		SaveAsDesign ();
	}

	void SaveAsDesign(){
		OnChange ();
		if (!RequirementsMet) {
			Debug.Log ("Requirements not met. DEBUG MODE ENABLED, output will continue anyway.");
		//	return;
		}
		ShipDesign design = new ShipDesign(DesignName.text);
		design.HullDesignation = HullDes.DesDictionary[HullDesignation.options [HullDesignation.value].text];
		design.ArmorLayers = Armor;
		design.CrewMin = ReqCrew;
		design.CrewBerths = Quarters;
		design.LifeSupport = LifeSupport;
		design.Output ();
	}
}
