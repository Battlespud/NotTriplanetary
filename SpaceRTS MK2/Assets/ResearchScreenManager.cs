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
	public Dropdown TechSections;
	public List<string> TechSectionNames = new List<string> (); 

	public GameObject ScientistsContent;

	public Character SelectedScientist;
	public Tech SelectedTech;

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
		InvokeRepeating ("UpdateUI", 0f, .2f);
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
			//	h.rotation = manager.GetComponent<RectTransform> ().rotation;
				h.rotation = Camera.main.transform.rotation;
			//	g.transform.rotation = manager.transform.rotation;
				h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);

			//	h.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, yOff*interval,h.rect.height);
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
		foreach (Character d in ActiveEmpire.Characters) {
		//	if (d.Role == OfficerRoles.Research) {
				GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
				ScientistObjects.Add (g);
				RectTransform h = g.GetComponent<RectTransform> ();
				ScientistManager manager = g.AddComponent<ScientistManager> ();
				manager.Manager = this;
				manager.Assign(d);
				h.SetParent (ScientistsContent.transform);
				//	h.rotation = manager.GetComponent<RectTransform> ().rotation;
				h.rotation = Camera.main.transform.rotation;
				//	g.transform.rotation = manager.transform.rotation;
				h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
				//	h.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, yOff*interval,h.rect.height);
				h.sizeDelta = new Vector2 (180f, 35f);
				h.localScale = new Vector3 (1f, 1f, 1f);
				interval++;
			}
		//}
	}

	void Update(){

	}

	public void UpdateUI(){
		try{
		OnTechSectionChange (TechSections.value);
		ScientistButtons ();
		}
		catch{
			Debug.LogError ("Failed to update research UI");
		}
	}

	public void ToggleActive(){
		if (gameObject.active) {
			StrategicClock.Unpause ();
			gameObject.active = false;
	} else {
			StrategicClock.RequestPause ();
			gameObject.active = true;
		}
	}

}
