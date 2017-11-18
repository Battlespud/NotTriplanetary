using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResearchProjectButtonManager : MonoBehaviour {

	ResearchScreenManager Manager;
	ResearchProject Res;

	Text t;
	Button b;

	public void Setup(ResearchProject p,ResearchScreenManager r){
		Res = p;
		Manager = r;
		Debug.Log("New research project " + Res.tech.Name);
		t = GetComponentInChildren<Text> ();
		b = GetComponent<Button> ();

		t.fontSize = 10;
		t.GetComponent<RectTransform> ().localScale = new Vector3 (.5f, 1f, 1f);

		t.text = string.Format ("{0}\tCost: {1}\tLead: {2} ",Res.tech.Name,Res.cost,Res.Scientist.GetNameString(true));
	}

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
