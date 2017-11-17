using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechButtonManager : MonoBehaviour {

	public Tech tech;
	public ResearchScreenManager Manager;

	public void Select(){
		Debug.Log ("Selecting " + tech.Name);
		Manager.SelectedTech = tech;
	}

	// Use this for initialization
	void Start () {
		Button b = GetComponent<Button> ();
		b.onClick.AddListener (Select);
		Text t = GetComponentInChildren<Text> ();
		t.text = tech.Name;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
