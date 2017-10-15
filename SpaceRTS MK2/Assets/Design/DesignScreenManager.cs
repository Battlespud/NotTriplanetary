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

		//Design test component
		ShipComponents c = new ShipComponents();
		c.name = "Heavy Test";
		c.Mass = 500;
		c.CrewRequired = 200;
		c.SetHTK (3);
		ShipComponents b = new ShipComponents();
		b.name = "Light Test";
		b.Mass = 250;
		b.CrewRequired = 50;
		b.SetHTK (2);
		ShipComponents a = new ShipComponents();
		a.name = "Bridge";
		a.Mass = 50;
		a.CrewRequired = 15;
		a.control = true;
		a.Category = CompCategory.REQUIRED;
		ShipComponents e = new ShipComponents ();
		e.name = "Engine E";
		e.Thrust = 1;
		e.Mass = 50;
		e.CrewRequired = 25;
		e.isEngine = true;
		e.Category = CompCategory.ENGINE;
		ShipComponents f = new ShipComponents ();
		f.name = "Engine F";
		f.Thrust = 1;
		f.Mass = 150;
		f.CrewRequired = 80;
		f.isEngine = true;
		f.Category = CompCategory.ENGINE;
		ShipComponents berths = new ShipComponents();
		berths.name = "Crew Quarters";
		berths.Mass = 25;
		berths.quarters = 100;
		berths.Category = CompCategory.REQUIRED;
		ShipComponents ls = new ShipComponents();
		ls.name = "Life Support";
		ls.Mass = 25;
		ls.lifeSupport = 250;
		ls.Category = CompCategory.REQUIRED;
		ShipComponents.DesignedComponents.Add (ls);
		ShipComponents.DesignedComponents.Add(berths);
		ShipComponents.DesignedComponents.Add (f);
		ShipComponents.DesignedComponents.Add (e);
		ShipComponents.DesignedComponents.Add (c);
		ShipComponents.DesignedComponents.Add (b);
		ShipComponents.DesignedComponents.Add (a);
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

	IEnumerator CalculateMass(){
		int M = 0;
		foreach (ShipComponents c in ShipComponents.DesignedComponents) {
			int number;
			if (AddedComponents.TryGetValue (c, out number)) {
				M += (c.Mass * number );
			}
		}
		Mass = M;
		yield return Ninja.JumpToUnity;
		MassText.text = "Mass: " + M + "kt";
	}

	void OnChange(){
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CheckCrewRequirements());
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CalculateMass());

		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CheckRequirements());
		ArmorText.text = ArmorLength.ToString() + " x " +  ArmorThickness.ToString ();

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
		ShipClass.DebugDesignTemplate = design;
		Debug.Log (design.DesignName + " has been set as default template.");
	}
}
