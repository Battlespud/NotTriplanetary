using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScientistManager : MonoBehaviour {

	public ResearchScreenManager Manager;
	public Character Scientist;

	public void Select(){
		Manager.SelectedScientist = Scientist;
//		Debug.Log("Selecting " + Scientist.GetNameString());
		Manager.UpdateUI ();
	}

	public void Assign(Character c){
		Scientist = c;
		Text t = GetComponentInChildren<Text> ();
		try{
			t.text = Scientist.GetNameString(true);
		}
		catch{
		}
		t.resizeTextForBestFit = true;
	}

	// Use this for initialization
	void Start () {
		Button b = GetComponent<Button> ();
		b.onClick.AddListener (Select);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
