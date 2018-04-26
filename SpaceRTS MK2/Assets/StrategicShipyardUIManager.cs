using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Events;


//NEEDS OVERHAUL TODO
public class StrategicShipyardUIManager : MonoBehaviour
{

	public Empire empire;
	
	public static StrategicShipyardUIManager Manager;

	public StrategicShipyard Shipyard;

	public Dropdown ShipyardDropdown;

	public SlipwayButtonUIManager SelectedSlipway;

	public List<SlipwayButtonUIManager> Slipways = new List<SlipwayButtonUIManager> ();

	public GameObject SlipwaysPrefab;
	public GameObject SlipwaysContentParent;
	public RectTransform SlipwaysContentParentRect;

	public Dropdown DesignsDrop;
	public Dropdown HullsDrop;
	public Button RetoolButton;
	public Text RetoolSummaryText;

	public Text SelectedSlipwayText;

	public Text ShipyardNameText;
	
	public Button BuildShipButton;
	public Button CancelBuildButton;

	public List<GameObject> DockedShipButtons = new List<GameObject>();
	public GameObject DockedShipsContentParent;
	public RectTransform DockedShipsContentParentRect;

	public Button ExitButton;


	public void AssignShipyard(StrategicShipyard s){
		Shipyard = s;
		SetupRetoolUI ();
		UpdateUI ();
	}
	
	void AssignShipyard(int s){
		Open(empire.Yards[s]);
	}

	void UpdateUI(){
		if (!Shipyard)
			return;
		ShipyardNameText.text = Shipyard.ShipYardName;
		UpdateRetoolUI ();
		foreach (SlipwayButtonUIManager s in Slipways) {
			s.UpdateUI ();
		}
		SetupDockedShips ();
	}

	void SetupSlipways(){

		int yOff = -40;
		int interval = 1;

		foreach (SlipwayButtonUIManager g in Slipways) {
			Destroy (g.gameObject);
		}
		Slipways.Clear();

		foreach (Slipway c in Shipyard.Slipways) {
			GameObject d = Instantiate (SlipwaysPrefab) as GameObject;
			SlipwayButtonUIManager e = d.GetComponent<SlipwayButtonUIManager> ();
			e.Manager = this;
			Slipways.Add (e);
			d.transform.SetParent (SlipwaysContentParent.transform);
			d.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (0f, interval * yOff, 0f);
			d.transform.localScale = new Vector3 (1f, 1f, 1f);
		//	d.GetComponent<RectTransform> ().rotation = Camera.main.transform.rotation; // SlipwaysContentParent.GetComponent<RectTransform> ().rotation;
			e.AssignSlip (c, interval.ToString() );
			interval++;
		}
	}


	void SetupDockedShips(){

		foreach (GameObject g in DockedShipButtons) {
			Destroy (g);
		}
		DockedShipButtons.Clear ();

		int yOff = -20;
		int interval = 1;
		foreach (StrategicShip d in Shipyard.DockedShips) {
			GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
			DockedShipButtons.Add (g);
			RectTransform h = g.GetComponent<RectTransform> ();
		//	ILocationButtonManager manager = g.AddComponent<ILocationButtonManager> ();
		//	manager.Manager = this;
		//	manager.Assign (d);
			g.GetComponentInChildren<Text>().text = d.ShipName;
			h.SetParent (DockedShipsContentParent.transform);
			//	h.rotation = Camera.main.transform.rotation;
			h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
			h.sizeDelta = new Vector2 (300f, 15f);
			h.localScale = new Vector3 (1f, 1f, 1f);
			interval++;
		}
	}



	//Resets the dropdown
	void SetupRetoolUI(){
		if (ShipDesign.Designs.Count <= 0)
			return;
		DesignsDrop.ClearOptions ();
		DesignsDrop.AddOptions (ShipDesign.DesignNames);
		UpdateRetoolUI ();
	}

	//Updates the text without resetting the dropdown
	void UpdateRetoolUI(int doesNothingJustHereBecauseOfDumbUnityEventRequirements = 0){
		if (ShipDesign.Designs.Count <= 0)
			return;
		/*
		if (Shipyard.CurrentTooling != null) {
			if (DropToShip () != Shipyard.CurrentTooling) {
				RetoolSummaryText.text = string.Format ("Time Required: {0}\nCurrent: {1}\nTarget: {2}", Shipyard.CalcToolingTime (DropToShip ()), Shipyard.CurrentTooling.HullName, DropToShip ().DesignName);
			} else {
				RetoolSummaryText.text = string.Format ("Time Required: {0}\nCurrent: {1}\nTarget: {2}", "Already Tooled", Shipyard.CurrentTooling.HullName, DropToShip ().DesignName);
			}
		} else {
			RetoolSummaryText.text = string.Format ("Time Required: {0}\nCurrent: {1}\nTarget: {2}",  Shipyard.CalcToolingTime(DropToShip()), "None", DropToShip ().DesignName);
		}
		*/
	}

	
	//Returns the dropdown selection as a shipdesign
	ShipDesign DropToShip(){
		return ShipDesign.Designs [DesignsDrop.value];
	}

	public void ChangeShipDesign(){
		///if(DropToShip() != Shipyard.
			//Shipyard.Retool (DropToShip ());
		UpdateRetoolUI ();
	}
	
	

	public void SelectSlipway(SlipwayButtonUIManager s){
		SelectedSlipway = s;
		SelectedSlipwayText.text = string.Format ("Selected Slipway: #{0}\n{1} Turns Remain", SelectedSlipway.Index, SelectedSlipway.Slip.TurnsToCompletion);
	}

	public void Close(){
		ResetUI();
		gameObject.SetActive (false);
		StrategicClock.Unpause ();
	}

	public void Open(StrategicShipyard s){
		AssignShipyard (s);
		ShipyardDropdown.ClearOptions();
		List<string>YardNames = new List<string>();
		empire.Yards.ForEach(x =>
		{
			YardNames.Add(x.ShipYardName);
		});
		ShipyardDropdown.AddOptions(YardNames);
		gameObject.SetActive (true);
		StrategicClock.RequestPause ();
		SetupSlipways ();
	}
	
	public void Open(){
		ShipyardDropdown.ClearOptions();
		List<string>YardNames = new List<string>();
		empire.Yards.ForEach(x =>
		{
			YardNames.Add(x.ShipYardName);
		});
		ShipyardDropdown.AddOptions(YardNames);
		if(!Shipyard)
			AssignShipyard(0);
		gameObject.SetActive (true);
		StrategicClock.RequestPause ();
		SetupSlipways ();
	}

	void ResetUI(){

	}

	public void BuildShip(){
		if(SelectedSlipway)
		//	SelectedSlipway.Slip.Assign (Shipyard.SelectedDesign);
		UpdateUI ();
	}

	public void CancelBuildShip(){
		if (SelectedSlipway)
			SelectedSlipway.Slip.Cancel ();
		UpdateUI ();
	}

	GameObject ButtonPrefab;

	// Use this for initialization
	void Start () {
		ButtonPrefab = Resources.Load<GameObject>("Button") as GameObject;
		DesignsDrop.onValueChanged.AddListener (UpdateRetoolUI);
		Manager = this;
		if(!SlipwaysPrefab)
		SlipwaysPrefab= Resources.Load<GameObject> ("SlipwayUIButton") as GameObject;
		ShipyardDropdown.onValueChanged.AddListener(AssignShipyard);
		Close ();
	}
	
	// Update is called once per frame
	void Update () {

			if (DockedShipsContentParentRect.localPosition.y < 0) {
				DockedShipsContentParentRect.transform.localPosition = new Vector3 (0f, 0f, 0f);
			}
			if (SlipwaysContentParentRect.localPosition.y < 0) {
				SlipwaysContentParentRect.transform.localPosition = new Vector3 (0f, 0f, 0f);
			}
		}
	}
	
