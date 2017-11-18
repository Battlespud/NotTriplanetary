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
		if (SelectedChar.Role == OfficerRoles.Army)
			RoleImage.color = Color.green;
		if (SelectedChar.Role == OfficerRoles.Government)
			RoleImage.color = Color.cyan;
		if (SelectedChar.Role == OfficerRoles.Research)
			RoleImage.color = Color.yellow;
		PP.text = string.Format("PP: {0}",SelectedChar.PromotionPoints);
		DateComm.text = SelectedChar.CommissionDate;
		Health.text = string.Format ("{0}/100", SelectedChar.HP);
		Color HealthColor = Color.green;
		if (SelectedChar.HP <= 0)
			HealthColor = Color.black;
		if (SelectedChar.HP > 0 && SelectedChar.HP < 26)
			HealthColor = Color.red;
		if (SelectedChar.HP >25 && SelectedChar.HP < 76)
			HealthColor = Color.yellow;
		HealthImage.color = HealthColor;
		RankNum.text = string.Format("{0}-{1}",Character.RolesAbbrev[(int)SelectedChar.Role],SelectedChar.Rank);
		RankName.text = SelectedChar.GetJobTitle ();
		Nobility.text = SelectedChar.GetNobleTitle ();
		Historytext.text = SelectedChar.History;
	}

	List<GameObject>OfficerButtons = new List<GameObject>();

	void UpdateOfficerScroll(int i){
		if (!Initialized)
			Initialize ();
		OfficerRoles r = (OfficerRoles)i;

		int yOff = -45;
		int interval = 1;
		foreach (GameObject g in OfficerButtons) {
			Destroy (g);
		}

		OfficerButtons.Clear ();
		foreach (Character d in ActiveEmpire.GetCharactersByType (r))
			 {
			GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
			OfficerButtons.Add (g);
			RectTransform h = g.GetComponent<RectTransform> ();
			OfficerButtonManager manager = g.AddComponent<OfficerButtonManager> ();
			manager.Manager = this;
			manager.Assign(d);
			h.SetParent (OfficersParent.transform);
			h.rotation = Camera.main.transform.rotation;
			h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
			h.sizeDelta = new Vector2 (800f, 35f);
			h.localScale = new Vector3 (1f, 1f, 1f);
			interval++;
		}

	}


	bool Initialized = false;

	// Use this for initialization
	void Initialize () {
		Initialized = true;
		ButtonPrefab = Resources.Load<GameObject>("Button") as GameObject;
		RolesDrop.ClearOptions ();
		List<string> RolesStrings = new List<string> ();
		foreach( OfficerRoles t in System.Enum.GetValues(typeof(OfficerRoles))){
			RolesStrings.Add (t.ToString ());
		}
		RolesDrop.AddOptions (RolesStrings);
		RolesDrop.onValueChanged.RemoveAllListeners ();
		RolesDrop.onValueChanged.AddListener (UpdateOfficerScroll);
	}


	public void ToggleActive(){
		if (gameObject.active) {
			StrategicClock.Unpause ();
			gameObject.active = false;
		} else {
			StrategicClock.RequestPause ();
			UpdateOfficerScroll (0);
			gameObject.active = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
