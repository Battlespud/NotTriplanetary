using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrategicShipUIManager : MonoBehaviour {

	public StrategicShip SelectedShip;
	public Empire ActiveEmpire;


	//Ships Scrollview
	public GameObject ShipScrollParent;
	public RectTransform ShipScrollRect;


	//Details Elements
	public Text Name;
	public Text Age;
	public Text PrefixHullDesignation;
	public Text FullHullDesignation;
	public Text Location;
	public Text Points;
	public Text CommissionDate;
	public Text Status;
	public Text ClassNumber;
	public Text ClassName;
	public Text	OverhaulClock;
	public Text Fuel;

	public Text CharactersOnboard;

	public Text History;

	public GameObject LocationsScroll;

	//Add scroll view content rects here
	public List<RectTransform> Monitor = new List<RectTransform>();

	//Tabs
	public Text ArmorText;

	public Text ComponentListText;







	GameObject ButtonPrefab;

	List<GameObject>ShipButtons = new List<GameObject>();

	void UpdateShipScroll(int i = 0){
		if (!Initialized)
			Initialize ();
	//	OfficerRoles r = (OfficerRoles)i;

		int yOff = -45;
		int interval = 1;
		foreach (GameObject g in ShipButtons) {
			Destroy (g);
		}

		ShipButtons.Clear ();
		foreach (StrategicShip d in ActiveEmpire.Ships) {
						GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
						ShipButtons.Add (g);
						RectTransform h = g.GetComponent<RectTransform> ();
						ShipButtonManager manager = g.AddComponent<ShipButtonManager> ();
						manager.Manager = this;
						manager.Assign (d);
						h.SetParent (ShipScrollParent.transform);
						h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
						h.sizeDelta = new Vector2 (800f, 35f);
						h.localScale = new Vector3 (1f, 1f, 1f);
						interval++;
		}
	}

	public List<GameObject> LocationButtons = new List<GameObject>();

	//Gets list of nearby ships and fleets
	void UpdateLocationScroll(int i = 0){
		if (!Initialized)
			Initialize ();

		int yOff = -55;
		int interval = 1;
		foreach (GameObject g in LocationButtons) {
			Destroy (g);
		}

		LocationButtons.Clear ();

		if (SelectedShip.ParentFleet != null) {
			GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
			LocationButtons.Add (g);
			RectTransform h = g.GetComponent<RectTransform> ();
			FleetButtonManager manager = g.AddComponent<FleetButtonManager> ();
			manager.Manager = this;
			manager.Assign (SelectedShip.ParentFleet);
			h.SetParent (LocationsScroll.transform);
			h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
			h.sizeDelta = new Vector2 (800f, 35f);
			h.localScale = new Vector3 (1f, 1f, 1f);
			interval++;

			foreach (StrategicShip d in SelectedShip.ParentFleet.Ships) {

				GameObject f = Instantiate<GameObject> (ButtonPrefab) as GameObject;
				LocationButtons.Add (f);
				RectTransform hk = f.GetComponent<RectTransform> ();
				ShipButtonManager sManager = f.AddComponent<ShipButtonManager> ();
				sManager.Manager = this;
				sManager.Assign (d);
				hk.SetParent (LocationsScroll.transform);
				hk.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
				hk.sizeDelta = new Vector2 (800f, 35f);
				hk.localScale = new Vector3 (1f, 1f, 1f);
				interval++;
			}
		}

		StrategicShipyard yard = null;
		SelectedShip.Emp.Yards.ForEach (x => {
			if(x.DockedShips.Contains(SelectedShip)){
				yard = x;
			}
		});

		if (yard != null) {
			foreach (StrategicShip d in yard.DockedShips) {

				GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
				LocationButtons.Add (g);
				RectTransform h = g.GetComponent<RectTransform> ();
				ShipButtonManager manager = g.AddComponent<ShipButtonManager> ();
				manager.Manager = this;
				manager.Assign (d);
				h.SetParent (LocationsScroll.transform);
				h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
				h.sizeDelta = new Vector2 (800f, 35f);
				h.localScale = new Vector3 (1f, 1f, 1f);
				interval++;
			}
			foreach (Fleet d in yard.DockedFleets) {

				GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
				LocationButtons.Add (g);
				RectTransform h = g.GetComponent<RectTransform> ();
				FleetButtonManager manager = g.AddComponent<FleetButtonManager> ();
				manager.Manager = this;
				manager.Assign (d);
				h.SetParent (LocationsScroll.transform);
				h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
				h.sizeDelta = new Vector2 (800f, 35f);
				h.localScale = new Vector3 (1f, 1f, 1f);
				interval++;
			}
		}
	}

	void UpdateShipDetails(){
		Name.text = SelectedShip.ShipName;
		Age.text = "TODO";
		Fuel.text = SelectedShip.GetFuelString();
		PrefixHullDesignation.text = SelectedShip.DesignClass.HullDesignation.Prefix;
		FullHullDesignation.text = SelectedShip.DesignClass.HullDesignation.HullType;
		if (SelectedShip.ParentFleet != null)
			Location.text = SelectedShip.ParentFleet.FleetName;
		else {
			StrategicShipyard d = null;
			foreach (StrategicShipyard s in SelectedShip.Emp.Yards) {
				if (s.DockedShips.Contains (SelectedShip)) {
					d = s;
					break;
				}
			}
			if (d != null)
				Location.text = d.ShipYardName;
			else
				Location.text = "Unknown";
		}
		Points.text = "PointsTODO";
		CommissionDate.text = CommissionDate.ToString();
		Status.text = (SelectedShip.GetFunctionality ()).ToString ("P");
		if (SelectedShip.isDamaged) {
			if(SelectedShip.GetFunctionality() < 65)
				Status.GetComponentInParent<Image> ().color = Color.red;
			else
				Status.GetComponentInParent<Image> ().color = Color.yellow;
		}
		else
			Status.GetComponentInParent<Image> ().color = Color.green;
		ClassNumber.text = "#TODO";
		ClassName.text = SelectedShip.DesignClass.DesignName;
		OverhaulClock.text = SelectedShip.MaintClock.ToString ();

		History.text = SelectedShip.ShipLog;

		ArmorText.text = SelectedShip.ArmorString;

		ComponentListText.text = "Components:\n--------------------------------------\n";
		foreach (string s in SelectedShip.ComponentStatus) {
			ComponentListText.text += s +"\n";
		}

		string CharactersOnboardString = string.Format ("Commanding Officer: {0}\nExecutive Officer:  {1}\n", SelectedShip.GetCaptainName (), SelectedShip.GetExecName ());
		CharactersOnboardString += "-------------------------------\nOther Officers Aboard\n-------------------------------\n";
		SelectedShip.CharactersAboard.ForEach (x => {
			if(x != SelectedShip.Captain && x != SelectedShip.Executive)
				CharactersOnboardString += x.GetNameString(false,true) + "\n";
		});

		CharactersOnboard.text = CharactersOnboardString;

		UpdateLocationScroll ();
	}

	public void SelectShip(StrategicShip S){
		SelectedShip = S;
		UpdateShipDetails ();
	}
	public void SelectFleet(Fleet f){
		SelectedShip.TransferToFleet (f);
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

	void Update(){
		foreach (RectTransform r in Monitor) {
			if (r.transform.localPosition.y < 0f) {
				r.transform.localPosition = new Vector3 (0f, 0f, 0f);
			}
		}
	}

	public void ToggleActive(){
		if (gameObject.active) {
			StrategicClock.Unpause ();
			gameObject.active = false;
		} else {
			StrategicClock.RequestPause ();
			ResetScroll ();
			UpdateShipScroll ();
			gameObject.active = true;
		}
	}

}
