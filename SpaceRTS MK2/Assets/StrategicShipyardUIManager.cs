using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.Events;

public class StrategicShipyardUIManager : MonoBehaviour {

	public static StrategicShipyardUIManager Manager;

	public StrategicShipyard Shipyard;


	public SlipwayButtonUIManager SelectedSlipway;

	public List<SlipwayButtonUIManager> Slipways = new List<SlipwayButtonUIManager> ();

	public GameObject SlipwaysPrefab;
	public GameObject SlipwaysContentParent;

	public Dropdown DesignsDrop;
	public Button RetoolButton;
	public Text RetoolSummaryText;

	public Text SelectedSlipwayText;

	public Button BuildShipButton;
	public Button CancelBuildButton;

	public GameObject DockedShipPrefab;
	public GameObject DockedShipsContentParent;

	public Button ExitButton;


	public void AssignShipyard(StrategicShipyard s){
		Shipyard = s;
		SetupRetoolUI ();
		UpdateUI ();

	}

	void UpdateUI(){
		if (!Shipyard)
			return;
		UpdateRetoolUI ();
		foreach (SlipwayButtonUIManager s in Slipways) {
			s.UpdateUI ();
		}
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
		//	d.GetComponent<RectTransform>().localScale = new Vector3 (.8f, .3f, 1f);
			d.GetComponent<RectTransform> ().SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, 45f, d.GetComponent<RectTransform>().rect.width);
			d.GetComponent<RectTransform> ().SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, yOff*interval, d.GetComponent<RectTransform>().rect.height);
			d.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (93f, interval * yOff, 0f);
			d.GetComponent<RectTransform> ().rotation = Camera.main.transform.rotation; // SlipwaysContentParent.GetComponent<RectTransform> ().rotation;

	//		d.GetComponent<RectTransform>().localPosition = new Vector3 (93, interval * yOff, 0f);
		//	d.transform.localPosition = new Vector3();

			e.AssignSlip (c, interval.ToString() );
			interval++;
		}
	}

	/*
	void SetupDockedShips(){

		int yOff = -40;
		int interval = 1;

		foreach (DockedShipPrefab g in DockedShipPrefabs) {
			Destroy (g.gameObject);
		}
		DockedShipPrefabs.Clear();

		foreach (StrategicShip c in Shipyard.DockedShips) {
			GameObject d = Instantiate (DockedShipPrefab) as GameObject;
			DockedShipManager e = d.GetComponent<DockedShipManager> ();
			e.Manager = this;
			DockedShipsManager.Add (e);
			d.transform.SetParent (DockedShipsContentParent.transform);
			//	d.GetComponent<RectTransform>().localScale = new Vector3 (.8f, .3f, 1f);
			d.GetComponent<RectTransform> ().SetInsetAndSizeFromParentEdge (RectTransform.Edge.Left, 45f, d.GetComponent<RectTransform>().rect.width);
			d.GetComponent<RectTransform> ().SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, yOff*interval, d.GetComponent<RectTransform>().rect.height);
			d.GetComponent<RectTransform> ().anchoredPosition3D = new Vector3 (93f, interval * yOff, 0f);
			d.GetComponent<RectTransform> ().rotation = DockedShipsContentParent.GetComponent<RectTransform> ().rotation;

			//		d.GetComponent<RectTransform>().localPosition = new Vector3 (93, interval * yOff, 0f);
			//	d.transform.localPosition = new Vector3();

		//	e.AssignSlip (c, interval.ToString() );
			interval++;
		}
	}

*/

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
		if (Shipyard.CurrentTooling != null) {
			if (DropToShip () != Shipyard.CurrentTooling) {
				RetoolSummaryText.text = string.Format ("Time Required: {0}\nCurrent: {1}\nTarget: {2}", Shipyard.CalcToolingTime (DropToShip ()), Shipyard.CurrentTooling.DesignName, DropToShip ().DesignName);
			} else {
				RetoolSummaryText.text = string.Format ("Time Required: {0}\nCurrent: {1}\nTarget: {2}", "Already Tooled", Shipyard.CurrentTooling.DesignName, DropToShip ().DesignName);
			}
		} else {
			RetoolSummaryText.text = string.Format ("Time Required: {0}\nCurrent: {1}\nTarget: {2}",  Shipyard.CalcToolingTime(DropToShip()), "None", DropToShip ().DesignName);
		}
	}

	//Returns the dropdown selection as a shipdesign
	ShipDesign DropToShip(){
		return ShipDesign.Designs [DesignsDrop.value];
	}

	public void Retool(){
		if(DropToShip() != Shipyard.CurrentTooling)
			Shipyard.Retool (DropToShip ());
	}

	public void SelectSlipway(SlipwayButtonUIManager s){
		SelectedSlipway = s;
		SelectedSlipwayText.text = string.Format ("Selected Slipway: #{0}\n{1} Turns Remain", SelectedSlipway.Index, SelectedSlipway.Slip.TurnsToCompletion);
	}

	public void Close(){
		Shipyard = null;
		ResetUI();
		gameObject.SetActive (false);
		StrategicClock.Unpause ();
	}

	public void Open(StrategicShipyard s){
		AssignShipyard (s);
		gameObject.SetActive (true);
		StrategicClock.RequestPause ();
		SetupSlipways ();
	}

	void ResetUI(){

	}

	public void BuildShip(){
		if(SelectedSlipway)
			SelectedSlipway.Slip.Assign ();
		UpdateUI ();
	}

	public void CancelBuildShip(){
		if (SelectedSlipway)
			SelectedSlipway.Slip.Cancel ();
		UpdateUI ();
	}


	// Use this for initialization
	void Start () {
		DesignsDrop.onValueChanged.AddListener (UpdateRetoolUI);
		Manager = this;
		if(!SlipwaysPrefab)
		SlipwaysPrefab= Resources.Load<GameObject> ("SlipwayUIButton") as GameObject;
		Close ();
	}
	
	// Update is called once per frame
	void Update () {
		foreach (SlipwayButtonUIManager a in Slipways) {
			a.transform.rotation = DockedShipsContentParent.transform.rotation;
		//	a.transform.position = new Vector3 (93);
		}
	}


}
