using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ResearchScreenManager : MonoBehaviour {

	static ResearchScreenManager Manager;

	public int TotalLabs;

	public static Empire ActiveEmpire;

	//UI Elements
	public GameObject ButtonPrefab;

	public GameObject TechsContent;
	public RectTransform TechsContentRect;

	public Dropdown TechSections;
	public List<string> TechSectionNames = new List<string> (); 

	public GameObject ScientistsContent;
	public RectTransform ScientistsContentRect;

	public Character SelectedScientist;
	public Tech SelectedTech;

	public GameObject ResearchProjectsParent;
	public RectTransform ResearchProjectsContentRect;


	public List<GameObject> TechsObjects = new List<GameObject>();
	public List<GameObject> ScientistObjects = new List<GameObject>();


	// Use this for initialization
	void Awake () {
		Manager = this;
		ButtonPrefab = Resources.Load<GameObject>("Button") as GameObject;
		foreach( TechSection t in System.Enum.GetValues(typeof(TechSection))){
			TechSectionNames.Add (t.ToString ());
		}
		SetupScreen ();

		TechSections.onValueChanged.AddListener (OnTechSectionChange);
		ToggleActive ();
		InvokeRepeating ("UpdateUI", 0f, 1f);
	}

	void SetupScreen(){
		TechSections.ClearOptions ();
		TechSections.AddOptions (TechSectionNames);
		OnTechSectionChange (0);
		ScientistButtons ();
	}

	void OnTechSectionChange(int techSect){
		int yOff = -45;
		int interval = 1;

		foreach (GameObject g in TechsObjects) {
			Destroy (g);
		}
		TechsObjects.Clear ();

		if (techSect == 0) {
			foreach (Tech d in ActiveEmpire.AvailableTechs) {
				GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
				TechsObjects.Add (g);
				RectTransform h = g.GetComponent<RectTransform> ();
				TechButtonManager manager = g.AddComponent<TechButtonManager> ();
				manager.Manager = this;
				manager.tech = d;
				h.SetParent (TechsContent.transform);
				h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
				h.sizeDelta = new Vector2 (170f, 35f);
				h.localScale = new Vector3(1f, 1f, 1f);
				interval++;
			}
		} else {
			TechSection t = (TechSection)techSect;
		}
	}

	void ScientistButtons(){
		int yOff = -45;
		int interval = 1;
		foreach (GameObject g in ScientistObjects) {
			Destroy (g);
		}
		ScientistObjects.Clear ();
		foreach (Character d in ActiveEmpire.GetCharactersByType(OfficerRoles.Research)) {
			bool busy = false;
			foreach (ResearchProject r in ActiveEmpire.EmpireTechTree.ResearchProjects) {
				if (d == r.Scientist)
					busy = true;
			}
			if (!busy) {
				GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
				ScientistObjects.Add (g);
				RectTransform h = g.GetComponent<RectTransform> ();
				ScientistManager manager = g.AddComponent<ScientistManager> ();
				manager.Manager = this;
				manager.Assign (d);
				h.SetParent (ScientistsContent.transform);
				h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
				h.sizeDelta = new Vector2 (180f, 35f);
				h.localScale = new Vector3 (1f, 1f, 1f);
				interval++;
			}
		}
	}

	public List<GameObject> ResearchProjectObjects = new List<GameObject>();
	void ResearchProjects(){
		int yOff = -45;
		int interval = 1;
		foreach (GameObject g in ResearchProjectObjects) {
			Destroy (g);
		}
		ResearchProjectObjects.Clear ();
		foreach (ResearchProject d in ActiveEmpire.EmpireTechTree.ResearchProjects) {
			GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
			ResearchProjectObjects.Add (g);
			RectTransform h = g.GetComponent<RectTransform> ();
			ResearchProjectButtonManager manager = g.AddComponent<ResearchProjectButtonManager> ();
			manager.Setup(d,this);
			h.SetParent (ResearchProjectsParent.transform);
			h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
			h.sizeDelta = new Vector2 (190f, 35f);
			h.localScale = new Vector3 (1f, 1f, 1f);
			interval++;
		}
	}

	void ResetScroll(){
		ResearchProjectsContentRect.localPosition = new Vector3 (0f, 0f, 0f);
		ScientistsContentRect.localPosition = new Vector3 (0f, 0f, 0f);
		TechsContentRect.localPosition  = new Vector3 (0f, 0f, 0f);
	}

	void Update(){

	}

	int GetSelectedLabs(){
		return 1; //TODO
	}

	public void CreateResearchProject()
	{
		if(SelectedScientist != null && SelectedTech != null)
			ActiveEmpire.EmpireTechTree.CreateResearch (SelectedScientist, SelectedTech, GetSelectedLabs ());
		ResearchProjects ();
		ResetScroll ();
	}

	public void UpdateUI(){
		if (gameObject.active) {
			OnTechSectionChange (TechSections.value);
			ScientistButtons ();
			if (ResearchProjectsContentRect.localPosition.y < 0) {
				ResearchProjectsContentRect.transform.localPosition = new Vector3 (0f, 0f, 0f);
			}
			if (ScientistsContentRect.localPosition.y < 0) {
				ScientistsContentRect.transform.localPosition = new Vector3 (0f, 0f, 0f);
			}
			if (TechsContentRect.localPosition.y < 0) {
				TechsContentRect.transform.localPosition = new Vector3 (0f, 0f, 0f);
			}
		}
	}

	public void ToggleActive(){
		if (gameObject.active) {
			StrategicClock.Unpause ();
			gameObject.active = false;
	} else {
			ResetScroll ();
			StrategicClock.RequestPause ();
			gameObject.active = true;
		}
	}

}
