using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfficerManagerUI : MonoBehaviour {

	public Character SelectedChar;
	public static Empire ActiveEmpire;

	//UI Elements
	public Dropdown RolesDrop;
	public GameObject RanksParent;
	public GameObject OfficersParent;
	public RectTransform OfficersParentRect;
	public GameObject LocationsParent;
	public RectTransform LocationsParentRect;

	public Text Historytext;

	public Button ExitButton;

	GameObject ButtonPrefab;

		//Officer
	public Text OfficerName;
	public Text Age;
	public Text Sex;
	public Text Personality;
	public Text Role;
	public Image RoleImage;
	public Text PP;
	public Text DateComm;
	public Text Health;
	public Image HealthImage;
	public Text RankNum;
	public Text RankName;
	public Text Nobility;
	public Image Portrait;
	Sprite DefaultPortrait;
	public Text Location;
	public Text TraitsText;

	public RectTransform HistoryRect;

	//Filters
	public Toggle NobilityFilter;
	bool NoblesOnly(){
		return NobilityFilter.isOn;
	}

	public Toggle UnassignedFilter;
	bool UnassignedOnly(){
		return NobilityFilter.isOn;
	}
	public InputField NameFilter;


	List<Text>OfficerFields = new List<Text>();


	public void SelectChar(Character c){
		SelectedChar = c;
		UpdateOfficerUI ();
	}

	void UpdateOfficerUI(){
		OfficerName.text = SelectedChar.GetNameString (true);
		Age.text = string.Format ("Age: {0}", SelectedChar.Age);
		Sex.text = SelectedChar.sex.ToString() [0].ToString();
		Personality.text = SelectedChar.GetPersonalitySummary ();
		Role.text = SelectedChar.Role.ToString ();
		if (SelectedChar.Role == OfficerRoles.Navy)
			RoleImage.color = Color.blue;
		else if (SelectedChar.Role == OfficerRoles.Army)
			RoleImage.color = Color.green;
		else if (SelectedChar.Role == OfficerRoles.Government)
			RoleImage.color = Color.cyan;
		else if (SelectedChar.Role == OfficerRoles.Research)
			RoleImage.color = Color.yellow;
		PP.text = string.Format("PP: {0}",SelectedChar.PromotionPoints);
		DateComm.text = SelectedChar.CommissionDate;
		Health.text = string.Format ("{0}/100", SelectedChar.HP);
		Color HealthColor = Color.green;
		if (SelectedChar.HP <= 0)
			HealthColor = Color.black;
		else if (SelectedChar.HP > 0 && SelectedChar.HP < 26)
			HealthColor = Color.red;
		else if (SelectedChar.HP >25 && SelectedChar.HP < 76)
			HealthColor = Color.yellow;
		HealthImage.color = HealthColor;
		RankNum.text = string.Format("{0}-{1}",Character.RolesAbbrev[SelectedChar.Role],SelectedChar.Rank);
		RankName.text = SelectedChar.GetJobTitle ();
		Nobility.text = SelectedChar.GetNobleTitle ();
		Historytext.text = SelectedChar.History;
		Portrait.sprite = SelectedChar.GetPortrait();

		Location.text = "";
		if (SelectedChar.Location != null) {
			System.Type t = SelectedChar.Location.GetLocType ();
			if (t == typeof(StrategicShip)) {
				if (SelectedChar.NavalRole != NavalCommanderRole.NONE)
					Location.text = SelectedChar.NavalRole.ToString () + " | ";
			}
			else if((t == typeof(StrategicShipyard)))
			{
				if (SelectedChar.NavalRole == NavalCommanderRole.CMD) {
					Location.text = "S.O. | ";
				}
			}
			Location.text += SelectedChar.Location.GetLocationName ();

		} else {
			Location.text = "Null";
		}
		UpdateLocations ();
		TraitsText.text = "";
		foreach (Trait t in SelectedChar.Traits) {
			TraitsText.text += t.Name + "\n";
		}
	}

	//valid transfers
	public List<GameObject> LocationButtons = new List<GameObject>();

	void UpdateLocations(){
		List<ILocation> Locs = new List<ILocation> ();

		while (LocationButtons.Count > 0) {
			foreach (GameObject g in LocationButtons) {
				Destroy (g);
			}
			LocationButtons.Clear ();
		}
//		Debug.Log (LocationButtons.Count + " Loc Count");
		if (SelectedChar.Location == null) {
			foreach (StrategicShip s in SelectedChar.empire.Ships) {
				Locs.Add (s);
			}
			foreach (StrategicShipyard s in SelectedChar.empire.Yards) {
				Locs.Add (s);
			}
		} else {
			Locs.Add (SelectedChar.Location);
			object obj = SelectedChar.Location.GetLocation ();
			System.Type t = SelectedChar.Location.GetLocType ();
			string tName = t.FullName;
			string ShipT = typeof(StrategicShip).FullName;
			string ShipyardT = typeof(StrategicShipyard).FullName;

			StrategicShip ship;
			StrategicShipyard yard;
			if (tName == ShipT) {
				ship = (StrategicShip)obj;
				if (ship.ParentFleet != null) {
					foreach (StrategicShip s in ship.ParentFleet.Ships) {
						Locs.Add (s);
					}
				}
			} else if (tName == ShipyardT) {
				yard = (StrategicShipyard)obj;
				foreach (StrategicShip s in yard.DockedShips) {
					Locs.Add (s);
				}
			}
		}

		int yOff = -75;
		int interval = 1;
		foreach (ILocation d in Locs) {
						GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
						LocationButtons.Add (g);
						RectTransform h = g.GetComponent<RectTransform> ();
						ILocationButtonManager manager = g.AddComponent<ILocationButtonManager> ();
						manager.Manager = this;
						manager.Assign (d);
						h.SetParent (LocationsParent.transform);
						h.rotation = Camera.main.transform.rotation;
						h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
						h.sizeDelta = new Vector2 (600f, 35f);
						h.localScale = new Vector3 (1f, 1f, 1f);
						interval++;
		}
	}

	public void SelectLocation(ILocation Loc){
		SelectedChar.MoveTo (Loc);
		UpdateOfficerUI ();
	}


	List<GameObject>OfficerButtons = new List<GameObject>();

	void UpdateOfficerScroll(int i = 0){
		if (!Initialized)
			Initialize ();
		OfficerRoles r = (OfficerRoles)i;

		int yOff = -45;
		int interval = 1;
		foreach (GameObject g in OfficerButtons) {
			Destroy (g);
		}

		OfficerButtons.Clear ();
		foreach (Character d in ActiveEmpire.GetCharactersByType (r)) {
			if (string.IsNullOrEmpty (NameFilter.text)  || d.GetNameString().IndexOf(NameFilter.text.Trim(),System.StringComparison.InvariantCultureIgnoreCase) > -1 ||(d.Location != null && d.Location.GetLocationName().IndexOf(NameFilter.text.Trim(),System.StringComparison.InvariantCultureIgnoreCase) > -1))  {
				if (!NoblesOnly () || (NoblesOnly () && d.Noble)) {
					if (!UnassignedOnly () || r != OfficerRoles.Navy || (UnassignedOnly () && d.NavalRole == NavalCommanderRole.NONE)) {
						GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
						OfficerButtons.Add (g);
						RectTransform h = g.GetComponent<RectTransform> ();
						OfficerButtonManager manager = g.AddComponent<OfficerButtonManager> ();
						manager.Manager = this;
						manager.Assign (d);
						h.SetParent (OfficersParent.transform);
						h.rotation = Camera.main.transform.rotation;
						h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
						h.sizeDelta = new Vector2 (800f, 35f);
						h.localScale = new Vector3 (1f, 1f, 1f);
						interval++;
					}
				}
			}
		}
	}

	void UpdateOfficerScrollBoolProxy(bool b){
		UpdateOfficerScroll (RolesDrop.value);
		ResetScroll ();
	}
	void UpdateOfficerScrollStringProxy(string b){
		UpdateOfficerScroll (RolesDrop.value);
		ResetScroll ();
	}

	void ResetScroll(){
		if (Initialized) {
			OfficersParentRect.localPosition = new Vector3 (0f, 0f, 0f);
			LocationsParentRect.position = new Vector3 (0f, 0f, 0f);
		}
	}

	bool Initialized = false;

	// Use this for initialization
	void Initialize () {
		Initialized = true;
		DefaultPortrait = Portrait.sprite;
		ButtonPrefab = Resources.Load<GameObject>("Button") as GameObject;
		RolesDrop.ClearOptions ();
		List<string> RolesStrings = new List<string> ();
		foreach( OfficerRoles t in System.Enum.GetValues(typeof(OfficerRoles))){
			RolesStrings.Add (t.ToString ());
		}
		RolesDrop.AddOptions (RolesStrings);
		RolesDrop.onValueChanged.RemoveAllListeners ();
		RolesDrop.onValueChanged.AddListener (UpdateOfficerScroll);
		NobilityFilter.onValueChanged.AddListener (UpdateOfficerScrollBoolProxy);
		RolesDrop.onValueChanged.AddListener (ClearNameFilter);
		NameFilter.onValueChanged.AddListener (UpdateOfficerScrollStringProxy);
		LocationsParentRect = LocationsParent.GetComponent<RectTransform> ();
		OfficersParentRect = OfficersParent.GetComponent<RectTransform> ();
	}

	void ClearNameFilter(int proxy){
		NameFilter.text = "";
	}


	public void ToggleActive(){
		if (gameObject.active) {
			StrategicClock.Unpause ();
			gameObject.active = false;
		} else {
			StrategicClock.RequestPause ();
			ResetScroll ();
			UpdateOfficerScroll (0);
			gameObject.active = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.active) {
			if (LocationsParentRect.localPosition.y < 0) {
				LocationsParentRect.transform.localPosition = new Vector3 (0f, 0f, 0f);
			}
			if (OfficersParentRect.localPosition.y < 0) {
				OfficersParentRect.transform.localPosition = new Vector3 (0f, 0f, 0f);
			}
			if (HistoryRect.localPosition.y < 0) {
				HistoryRect.transform.localPosition = new Vector3 (0f, 0f, 0f);
			}
		}
	}
}
