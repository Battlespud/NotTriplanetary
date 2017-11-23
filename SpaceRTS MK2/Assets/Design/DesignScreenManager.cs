using System.Collections;
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

public enum EngineTypes{
	CONVENTIONAL = 1,
	NUCLEAR = 3,
	ION = 5,
	MAGNETO = 6,
	FUSION = 9,
	MAGNETICFUSION = 12,
	INERTIALFUSION = 14,
	ANTIMATTER = 18,
	PHOTONIC = 25
};

public class DesignScreenManager : MonoBehaviour {

	static Color highlight = new Color (181f,242f,242f);
	static Color pressed = new Color (36f,212f,205f);

	//armor width = 1.2*(Mass/50)^2/3

	public Empire ActiveEmpire;

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
	public GameObject ShipComponentsUIButton;
	public Text MassText;
	public Text InstalledText;
	public ScrollRect scrollview;

	//we'll figure out how this figures in later.
	public int IntendedDeploymentInTurns = 20;

	public Text ComponentStrings;

	public Text Description;

	public static bool LoadAllComponents = false;

	public bool HideObsolete = false;

	//Components selectable by the player go in Components
	public List<ShipComponents> Components = new List<ShipComponents> ();
	public List<GameObject> UIObjects = new List<GameObject>();


	public Dictionary<ShipComponents, int> AddedComponents = new Dictionary<ShipComponents, int>();

	public ShipComponents EngineDesign; //only one type of engine permitted. Whatever is set here is that type.
	public int EngineCount; //since engines arent tracked the same as the other components we need this.

	//How many of each component
	public Dictionary<ShipComponents, int> ComponentNumbers = new Dictionary<ShipComponents, int>();

	//Calculations
	public int Tonnage; //in kilotons

	//maint
	public float BaseFailRate = 0f;
	public float EffectiveFailRate = 0f;
	public float EngineeringPercent;

	//Info
	public int ArmorThickness = 1;
	public int ArmorLength = 1;
	public int ReqCrew;
	public int Mass;
	ArmorTypes ArmorType = ArmorTypes.DURANIUM;

	public int Quarters;   //how much space for crew there is with current loadout.


	public void PopulateComponentList(){
		List<ShipComponents> LoadedComponents = ShipComponents.GetComponents(ActiveEmpire.Token).OrderByDescending (x => x.Category).ToList ();
		Components.Clear();
		if (HideObsolete) {
			foreach (ShipComponents c in LoadedComponents) {
				if (!c.Obsolete)
					Components.Add (c);
			}
		} else {
			Components.AddRange (LoadedComponents);
		}
	//	Debug.Log (Components.Count + " components loaded.");
		float yOff = -7.5f;
		int interval = 0;
		foreach (GameObject g in UIObjects) {
			Destroy (g);
		}
		foreach (ShipComponents c in Components) {
			GameObject d = Instantiate (ShipComponentsUIButton) as GameObject;
			UIObjects.Add (d);
		d.transform.SetParent (ContentParentScrollview);
			d.GetComponent<RectTransform> ().rotation = Camera.main.transform.rotation;
			d.GetComponent<RectTransform> ().transform.localScale = new Vector3 (1f, 1f, 1f);
			d.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1458, 10f);
			d.GetComponent<RectTransform>().anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
			ShipComponentUIManager s = d.GetComponent<ShipComponentUIManager> ();
			s.Manager = this;
			s.Assign (c);
			interval++;
		}
	}

	public void AddComponent(ShipComponents comp){
		Debug.Log("Adding " + comp.Name);
		if(AddedComponents.ContainsKey(comp)){
			AddedComponents[comp] += 1;
		}
		else{
			AddedComponents.Add (comp, 1);
		}
		OnChange ();
	}

	//engines have some special rules so we add them using this.
	public void AddEngine(ShipComponents engine){
		Debug.Log("Adding " + engine.Name);
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

	#region armor
	//Have to be methods for the ui to work.
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

	int ArmorMass(){
		return (50 / (int)ArmorType);
	}
	#endregion


	// Use this for initialization
	void Start () {
		ButtonPrefab = Resources.Load<GameObject> ("ButtonWide") as GameObject;
		ShipComponentsUIButton= Resources.Load<GameObject> ("ShipComponentsUIButton") as GameObject;
		SetupScreen ();
	}

	void SetupScreen(){
		Debug.Log ("Setting up design screen");
		HullDesignation.ClearOptions ();
		HullDesignation.AddOptions (HullDes.HullTypes);
		ArmorThickness = 1;
		ArmorText.text = ArmorThickness.ToString ();
		DesignName.text = "";
		OnChange ();
	}
	
	// Update is called once per frame
	void Update () {
		if (LoadAllComponents) {
			LoadAllComponents = false;
			PopulateComponentList ();
		}
	}

	bool RequirementsMet = false;
	IEnumerator CheckRequirements(){
		string OutstandingRequirements = "Requirements:\n";
		RequirementsMet = false;
		int bridgeCount = 0;
		ShipComponents CommandComp = null;
		foreach (ShipComponents c in AddedComponents.Keys.ToList()) {
			foreach (Ability a in c.Abilities) {
				if (a.AbilityType == AbilityCats.CONTROL) {
					CommandComp = c;
					bridgeCount++;
					break;
					}
				}
			}
			if (bridgeCount < 1) {
				OutstandingRequirements += "Missing Bridge\n";
			} 
		else if (AddedComponents[CommandComp] > 1) { //Multiple of the same type of commandcomponent. Only safe to check if at least one is found. 
				OutstandingRequirements += "Cannot have more than 1 Bridge\n";
			}
		if (bridgeCount > 1) {  //Would only occur if there are multiple different components with the control ability.
			OutstandingRequirements += "Cannot have more than 1 Bridge\n";
		}
		if (ShipDesign.DesignDictionary.ContainsKey (DesignName.text)) {
			RequirementsMet = false;
			OutstandingRequirements += DesignName.text + " is already being used as a class name.\n";
		}
		if (EngineCount < 1) {
			RequirementsMet = false;
			OutstandingRequirements += "Valid ship designs must contain at least one engine.\n";
		}
		if (ArmorThickness <= 0) {  //This shouldnt be possible outside of an error in the actual unity editor.
			RequirementsMet = false;
			OutstandingRequirements += "Error, please change Armor to a positive value of at least 1.\n";
		}
		if (Quarters < ReqCrew) {      //Quarters sums the crew quartering ability of all added components and compares it to the Required Number.
			RequirementsMet = false;
			OutstandingRequirements += string.Format("Quarters for only {0} of the {1} required crewmembers is present.\n",Quarters,ReqCrew);
		}
		yield return Ninja.JumpToUnity;
		Requirements.text = OutstandingRequirements;
	}

	IEnumerator CheckCrewRequirements(){
		int quarter = 0;
		int minimum = 0;
		foreach (ShipComponents c in Components) {
			int number;
			if (AddedComponents.TryGetValue (c, out number)) {
				minimum += (c.CrewRequired * number );
				foreach (Ability a in c.Abilities) {
					if (a.AbilityType == AbilityCats.CREW) {
						quarter += ((int)a.Rating * number);

					}
				}
			}
		}
		Quarters = quarter;
		ReqCrew = minimum;
		yield return Ninja.JumpToUnity;
		CrewReqText.text = string.Format ("Crew Required: {0}", ReqCrew);
	}



	IEnumerator CalculateMass(){
		int M = 0;
		foreach (ShipComponents c in Components) {
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

	public void SetupMaint(){
		float MaintMass = 0;
		foreach (ShipComponents c in Components) {
			if (c.isMaint() && AddedComponents.ContainsKey(c)) {
				MaintMass += c.getMaintMass ()*AddedComponents[c];
			}
		}
		Debug.Log (MaintMass + " Maint Mass");
		float percent = (MaintMass / Mass)*100f;
		BaseFailRate = Mass / 950  * (2 / (percent)); //Chance of breakdown during 20 turn deployment
		if (percent == 0f) 
			BaseFailRate = Mass / 5; 
		EffectiveFailRate = BaseFailRate * (StrategicShip.TimeBetweenRolls / 20);
		EngineeringPercent = percent;
	}

	void OnChange(){
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CheckCrewRequirements());
		CalculateArmorWidth ();
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CalculateMass());
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CheckRequirements());
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,CalcMaxSpeed());
		ArmorText.text = ArmorLength.ToString() + " x " +  ArmorThickness.ToString ();
		SetupMaint ();
		InstalledText.text = GenerateInstalledText();
		Description.text = BuildDescription();

	}

	string GenerateInstalledText(){
		string a = "Added:\n";
		foreach (ShipComponents c in AddedComponents.Keys) {
			a += string.Format ("{0}KT: {1} {2}x\n",c.Mass*AddedComponents[c], c.Name, AddedComponents [c]);
		}
		return a;
	}

	void CalculateArmorWidth(){
		int hs = Mass / 50;
		ArmorLength =(int)(1 + 1.1*(Mathf.Pow (hs, .55f))); //.66 works well
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
		design.mass = Mass;
		design.ArmorType = ArmorType;
		foreach (ShipComponents c in Components) {
				int number;
				if (AddedComponents.TryGetValue (c, out number)) {
				for (int i = 0; i < number; i++) {
					ShipComponents comp = new ShipComponents();
					comp = c.CloneProperties ();
					Debug.Log ("Original " + c.Name);
					Debug.Log ("Clone " + comp.Name);
					design.Components.Add (comp);
					}
				}
			}
		design.SetupDAC ();
		design.CalculateCost ();
//		design.Output ();
	}

	float MaxSpeed;

	IEnumerator CalcMaxSpeed(){
		float thrust = 0;
		//Speed = (Total Thrust / Total Class Size in HS) * 1000 km/s
		foreach (ShipComponents c in Components) {
			int number;
			if(c.Category == CompCategory.ENGINE){
				if (AddedComponents.TryGetValue (c, out number)) {
					thrust += c.GetThrust()*number;
				}
			}
		}
		Debug.Log ("Max thrust " + thrust);
		MaxSpeed = thrust / ((float)Mass/50f)*1000f;
		yield return Ninja.JumpToUnity;
	}

	string BuildDescription(){
		string description = string.Format ("{0} Class {1}\t{2} KTons\t{3} Crew\t{4} BuildCost\n{5} km/s\nBFR: {6}%\tEFR: {7}%\tEngineering:{8}%",
											DesignName.text, HullDesignation.options [HullDesignation.value].text, Mass, ReqCrew, "#",MaxSpeed, BaseFailRate*100f, EffectiveFailRate*100f, EngineeringPercent);
		return description;
	}

}
/*
 											 Sample
Alaska class Escort Cruiser    17 300 tons     319 Crew     2939.2 BP      TCS 346  TH 648  EM 0
1872 km/s     Armour 14-59     Shields 0-0     Sensors 8/1/0/0     Damage Control Rating 6     PPV 31
Maint Life 2.18 Years     MSP 637    AFR 399%    IFR 5.5%    1YR 179    5YR 2691    Max Repair 168 MSP
Intended Deployment Time: 6 months    Spare Berths 0    
*/