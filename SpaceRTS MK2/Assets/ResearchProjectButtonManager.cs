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
		t.text = "";//string.Format ("{0}\tCost: {1}\tLead: {2} ",Res.tech.Name,Res.cost,Res.Scientist.GetNameString(true));
	}

	// Use this for initialization
	void Start () {
		t = GetComponentInChildren<Text> ();
		b = GetComponent<Button> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
