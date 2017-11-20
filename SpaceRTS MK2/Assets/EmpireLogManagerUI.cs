using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EmpireLogManagerUI : MonoBehaviour {

	public Empire ActiveEmpire;
	static List<string>PriorityColors = new List<string>(){"<color=green>","<color=red>","<color=orange>","<color=yellow>","<color=cyan>","<color=blue>","<color=white>"};

	public GameObject EntriesContentParent;
	public RectTransform EntriesContentParentRect;

	public List<LogCategories>Categories = new List<LogCategories>();

	public Dropdown Months;
	public Dropdown Years;

	public Dropdown Priority;
	public bool PriorityLowerOrEqual = true;

	static List<string> priorityStrings = new List<string> ();
	public List<GameObject> Buttons = new List<GameObject> ();
	GameObject ButtonPrefab;

	public EmpireLogEntry SelectedEntry;

	// Use this for initialization
	void Awake () {
		for (int i = 0; i < PriorityColors.Count; i++) {
			priorityStrings.Add(string.Format("<color=white>[</color>{0}{1}</color><color=white>]</color>",PriorityColors[i],i));
		}
		Priority.ClearOptions();
		Priority.AddOptions(priorityStrings);
		Priority.onValueChanged.AddListener(UpdateLog);
		Months.onValueChanged.AddListener (ResetScroll);
		Years.onValueChanged.AddListener (ResetScroll);
		Months.ClearOptions ();
		Months.AddOptions ( StrategicClock.Months);
		ButtonPrefab = Resources.Load<GameObject>("Button") as GameObject;
		UpdateYears ();
		StrategicClock.PhaseChange.AddListener (PhaseManager);
		gameObject.active = false;
	}

	void UpdateYears(){
		Years.ClearOptions ();
		Years.AddOptions (StrategicClock.Years);
	}

	void PhaseManager(Phase p){
		switch (p) {
		case(Phase.ORDERS):
			{
				break;
			}
		case(Phase.GO):
			{
				break;
			}
		case (Phase.REVIEW):
			{
				Months.value = StrategicClock.month;
				Years.value = StrategicClock.year;
				break;
			}
		case (Phase.INTERRUPT):
			{
				break;
			}

		}	
	}


	string Date(){
	//	Months[month] +" " + year;
		return string.Format ("{0} {1}", StrategicClock.Months [Months.value], StrategicClock.Years [Years.value]);
	}



	void UpdateLog(int z = 0){
		List<EmpireLogEntry> Entries = ActiveEmpire.Logbook [Date()];
		List<EmpireLogEntry> ToDisplay = new List<EmpireLogEntry> ();
		foreach (EmpireLogEntry e in Entries) {
			if ((e.Priority <= Priority.value && PriorityLowerOrEqual) || Priority.value == e.Priority|| Priority.value <= 0) {
				if (Categories.Contains (e.Category)) {
					ToDisplay.Add (e);
				}
			}
		}
		ToDisplay = ToDisplay.OrderBy (x => x.Priority).ToList ();

		int yOff = -45;
		int interval = 0;
		foreach (GameObject g in Buttons) {
			Destroy (g);
		}
		Buttons.Clear ();
		foreach (EmpireLogEntry d in ToDisplay) {
			GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
			RectTransform h = g.GetComponent<RectTransform> ();
			EmpireLogButtonManager manager = g.AddComponent<EmpireLogButtonManager> ();
			manager.Setup(d,this);
			h.SetParent (EntriesContentParent.transform);
			//	h.rotation = manager.GetComponent<RectTransform> ().rotation;
			h.rotation = Camera.main.transform.rotation;
			//	g.transform.rotation = manager.transform.rotation;
			h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
			//	h.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, yOff*interval,h.rect.height);
			h.sizeDelta = new Vector2 (400f, 35f);
			h.localScale = new Vector3 (1f, 1f, 1f);
			interval++;
		}

	}

	void UpdateReadout(){
		
	}

	void ToggleCategory(LogCategories c)
	{
		if (Categories.Contains (c)) {
			Categories.Remove (c);
		} else {
			Categories.Add (c);
		}
	}

	void ResetScroll(int i = 0){
		EntriesContentParentRect.transform.localPosition = new Vector3 (0f, 0f, 0f);
	}

	public void SelectEntry(EmpireLogEntry e){
		SelectedEntry = e;
		UpdateReadout ();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameObject.active) {
			if (EntriesContentParentRect.localPosition.y < 0) {
				ResetScroll ();
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
			UpdateLog ();
		}
	}
}
