using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class EmpireLogManagerUI : MonoBehaviour {

	public Empire ActiveEmpire;
	static List<string>PriorityColors = new List<string>(){"<color=green>","<color=red>","<color=orange>","<color=yellow>","<color=cyan>","<color=blue>","<color=white>"};
	Dictionary<LogCategories,string> CatColor = new Dictionary<LogCategories, string> ();

	public GameObject EntriesContentParent;
	public RectTransform EntriesContentParentRect;

	public List<LogCategories>Categories = new List<LogCategories>(){LogCategories.DEFAULT,LogCategories.ECONOMIC,LogCategories.EXPLORATION,LogCategories.MILITARY,LogCategories.TECH};

	public Dropdown Months;
	public Dropdown Years;

	public Dropdown Priority;
	bool PriorityLowerOrEqual = false;

	//Readout
	public Text HeadlineReadout;
	public Text DateReadout;
	public Text PriorityReadout;
	public Text DescriptionReadout;
	public Text CategoryReadout;


	static List<string> priorityStrings = new List<string> ();
	public List<GameObject> Buttons = new List<GameObject> ();
	GameObject ButtonPrefab;

	public EmpireLogEntry SelectedEntry;

	// Use this for initialization
	void Awake () {
		for (int i = 0; i < PriorityColors.Count; i++) {
			priorityStrings.Add(string.Format("<color=white>[</color>{0}{1}</color><color=white>]</color>",PriorityColors[i],i));
		}
		CatColor.Add (LogCategories.DEFAULT, "<color=white>");
		CatColor.Add (LogCategories.ECONOMIC, "<color=green>");
		CatColor.Add (LogCategories.EXPLORATION, "<color=magenta>");
		CatColor.Add (LogCategories.MILITARY, "<color=red>");
		CatColor.Add (LogCategories.TECH, "<color=cyan>");
		Priority.ClearOptions();
		Priority.AddOptions(priorityStrings);
		Priority.onValueChanged.AddListener(UpdateLog);
		Months.onValueChanged.AddListener (ResetScroll);
		Years.onValueChanged.AddListener (ResetScroll);
		Months.onValueChanged.AddListener (UpdateLog);
		Years.onValueChanged.AddListener (UpdateLog);
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
				Months.value = StrategicClock.month;
				Years.value = StrategicClock.year;
				break;
			}
		case(Phase.GO):
			{
				break;
			}
		case (Phase.REVIEW):
			{

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
	//	Debug.Log(string.Format ("{0} {1}", StrategicClock.Months [Months.value], StrategicClock.Years [Years.value]) + " Date Log Request");
		return string.Format ("{0} {1}", StrategicClock.Months [Months.value], StrategicClock.Years [Years.value]).Trim();
	}



	void UpdateLog(int z = 0){
		List<EmpireLogEntry> Entries = ActiveEmpire.GetLogs( Date());
		List<EmpireLogEntry> ToDisplay = new List<EmpireLogEntry> ();
		foreach (EmpireLogEntry e in Entries) {
			if ( (e.Priority <= Priority.value && PriorityLowerOrEqual) || Priority.value == e.Priority || Priority.value <= 0) {
	//			if (Categories.Contains (e.Category)) {
					ToDisplay.Add (e);
				}
		//	}
		}
		ToDisplay = ToDisplay.OrderBy (x => x.Priority).ToList ();

		int yOff = -35;
		int interval = 1;
		foreach (GameObject g in Buttons) {
			Destroy (g);
		}
		Buttons.Clear ();
		foreach (EmpireLogEntry d in ToDisplay) {
			GameObject g = Instantiate<GameObject> (ButtonPrefab) as GameObject;
			Buttons.Add (g);
			RectTransform h = g.GetComponent<RectTransform> ();
			EmpireLogButtonManager manager = g.AddComponent<EmpireLogButtonManager> ();
			manager.Setup(d,this);
			h.SetParent (EntriesContentParent.transform);
			//	h.rotation = manager.GetComponent<RectTransform> ().rotation;
			h.rotation = Camera.main.transform.rotation;
			//	g.transform.rotation = manager.transform.rotation;
			h.anchoredPosition3D = new Vector3 (0f, yOff * interval, 0f);
			//	h.SetInsetAndSizeFromParentEdge (RectTransform.Edge.Top, yOff*interval,h.rect.height);
			h.sizeDelta = new Vector2 (600f, 25f);
			h.localScale = new Vector3 (2f, 1f, 1f);
			interval++;
		}

	}

	void UpdateReadout(){
		if (SelectedEntry != null) {
			CategoryReadout.text = string.Format ("{0}{1}</color>", CatColor [SelectedEntry.Category], SelectedEntry.Category.ToString ());
			DateReadout.text = SelectedEntry.Date;
			DescriptionReadout.text = SelectedEntry.Description;
			HeadlineReadout.text = SelectedEntry.Headline;
			PriorityReadout.text = string.Format ("<color=white>[</color>{0}{1}</color><color=white>]</color>", PriorityColors [SelectedEntry.Priority], SelectedEntry.Priority);

		}
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
