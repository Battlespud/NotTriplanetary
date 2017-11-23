using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrategicShipUIManager : MonoBehaviour {

	public StrategicShip SelectedShip;
	public static Empire ActiveEmpire;


	//Ships Scrollview
	public GameObject ShipScrollParent;
	public RectTransform ShipScrollRect;


	//Details Elements
	public Text Name;
	public Text Age;
	public Text HullDesignation;
	public Text Location;
	public Text Points;
	public Text CommissionDate;
	public Text Status;
	public Text ClassNumber;
	public Text ClassName;
	public Text	OverhaulClock;

	public GameObject CharactersOnboardScroll;
	public RectTransform CharactersOnboardRect;


	//Tabs
	public Text ArmorText;









	GameObject ButtonPrefab;

	List<GameObject>ShipButtons = new List<GameObject>();

	void UpdateShipScroll(int i = 0){
		if (!Initialized)
			Initialize ();
		OfficerRoles r = (OfficerRoles)i;

		int yOff = -45;
		int interval = 1;
		foreach (GameObject g in ShipButtons) {
			Destroy (g);
		}

		ShipButtons.Clear ();
		foreach (StrategicShip d in ActiveEmpire.Ships) {
		///	if (string.IsNullOrEmpty (NameFilter.text)  || d.GetNameString().IndexOf(NameFilter.text.Trim(),System.StringComparison.InvariantCultureIgnoreCase) > -1 ||(d.Location != null && d.Location.GetLocationName().IndexOf(NameFilter.text.Trim(),System.StringComparison.InvariantCultureIgnoreCase) > -1))  {
		///		if (!NoblesOnly () || (NoblesOnly () && d.Noble)) {
		///			if (!UnassignedOnly () || r != OfficerRoles.Navy || (UnassignedOnly () && d.NavalRole == NavalCommanderRole.NONE)) {
						GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
						ShipButtons.Add (g);
						RectTransform h = g.GetComponent<RectTransform> ();
						ShipButtonManager manager = g.AddComponent<ShipButtonManager> ();
						manager.Manager = this;
						manager.Assign (d);
						h.SetParent (ShipScrollParent.transform);
						h.rotation = Camera.main.transform.rotation;
						h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
						h.sizeDelta = new Vector2 (800f, 35f);
						h.localScale = new Vector3 (1f, 1f, 1f);
						interval++;
			///		}
		///		}
		///	}
		}
	}

	void UpdateShipDetails(){
		Name.text = SelectedShip.ShipName;
		Age.text = "TODO";
		HullDesignation.text = SelectedShip.DesignClass.HullDesignation.Prefix;
		Location.text = "TODO";
		Points.text = "TODO";
		CommissionDate.text = CommissionDate.ToString();
		Status.text = (SelectedShip.GetFunctionality () * 100).ToString ("P");
		ClassNumber.text = "TODO";
		ClassName.text = SelectedShip.DesignClass.DesignName;
		OverhaulClock.text = SelectedShip.MaintClock.ToString ();
	}

	public void SelectShip(StrategicShip S){
		SelectedShip = S;
		UpdateShipDetails ();
	}

	void UpdateOfficerScrollBoolProxy(bool b){
	//	UpdateShipScroll (RolesDrop.value);
		ResetScroll ();
	}
	void UpdateOfficerScrollStringProxy(string b){
	//	UpdateShipScroll (RolesDrop.value);
		ResetScroll ();
	}

	void ResetScroll(){
		if (Initialized) {
			ShipScrollRect.localPosition = new Vector3 (0f, 0f, 0f);
		}
	}

	bool Initialized = false;

	// Use this for initialization
	void Initialize () {
		Initialized = true;
	//	DefaultPortrait = Portrait.sprite;
		ButtonPrefab = Resources.Load<GameObject>("Button") as GameObject;
		//RolesDrop.ClearOptions ();
	//	RolesDrop.AddOptions (RolesStrings);
	//	RolesDrop.onValueChanged.RemoveAllListeners ();
	//	RolesDrop.onValueChanged.AddListener (UpdateOfficerScroll);
	//	NobilityFilter.onValueChanged.AddListener (UpdateOfficerScrollBoolProxy);
	//	RolesDrop.onValueChanged.AddListener (ClearNameFilter);
	//	NameFilter.onValueChanged.AddListener (UpdateOfficerScrollStringProxy);
	//	LocationsParentRect = LocationsParent.GetComponent<RectTransform> ();
		ShipScrollRect = ShipScrollParent.GetComponent<RectTransform> ();
	}

}
