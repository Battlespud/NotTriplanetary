using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmpireLogButtonManager : MonoBehaviour {

	static List<string>Colors = new List<string>(){"<INVALID>","<color=red>","<color=orange>","<color=yellow>","<color=cyan>","<color=blue>","<color=white>"};
	Dictionary<LogCategories,string> CatColor = new Dictionary<LogCategories, string> ();

	EmpireLogManagerUI Manager;
	EmpireLogEntry e;

	RectTransform r;

	Text t;
	Button b;

	public void Setup(EmpireLogEntry p,EmpireLogManagerUI h){
		e = p;
		Manager = h;
		GetComponent<Image> ().color = Color.grey;
		r = GetComponent<RectTransform> ();
		t = GetComponentInChildren<Text> ();
		b = GetComponent<Button> ();
		CatColor.Add (LogCategories.DEFAULT, "<color=white>");
		CatColor.Add (LogCategories.ECONOMIC, "<color=green>");
		CatColor.Add (LogCategories.EXPLORATION, "<color=magenta>");
		CatColor.Add (LogCategories.MILITARY, "<color=red>");
		CatColor.Add (LogCategories.TECH, "<color=cyan>");
		t.fontSize = 18;
		t.GetComponent<RectTransform> ().localScale = new Vector3 (1.5f, 1f, 1f);
		b.onClick.AddListener (Select);


		t.GetComponent<RectTransform>().offsetMin = new Vector2 (130f, t.GetComponent<RectTransform>().offsetMin.y);
		t.alignment = TextAnchor.MiddleLeft;

		t.text = string.Format ("{0}[{1}{2}</color>] {3}{4}</color> {5}","",Colors[e.Priority] ,e.Priority, CatColor[e.Category],e.Category.ToString(),e.Headline);
	}

	public void Select(){
		Manager.SelectEntry (e);
	}


	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		if (gameObject.active && r!=null)
			r.rotation = Camera.main.transform.rotation;
	}
}
