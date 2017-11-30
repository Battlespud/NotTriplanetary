using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FleetButtonManager : MonoBehaviour {
	public StrategicShipUIManager Manager;
	public Fleet S;

	public void Select(){
		Manager.SelectFleet(S);
		Update ();
	}

	public void Assign(Fleet c){
		S = c;
		Text t = GetComponentInChildren<Text> ();
		try{
			t.text = S.FleetName + "[" + S.Ships.Count + "]";
			t.fontSize = 20;
		}
		catch{
		}
		//		t.resizeTextForBestFit = true;
	}
	Button b;

	// Use this for initialization
	void Start () {
		b = GetComponent<Button> ();
		b.onClick.AddListener (Select);
		DefaultColor = b.colors.normalColor;
		//	SelectedColor = Color.cyan;
		ColorBlock cb = b.colors;
		cb.highlightedColor = Color.cyan;
		cb.normalColor = Color.white;
		t = GetComponent<RectTransform> ();
		b.colors = cb;
	}

	Color DefaultColor;
	Color SelectedColor;

	RectTransform t;

	// Update is called once per frame
	void Update () {
		//	if (gameObject.active)
		//	t.rotation = Camera.main.transform.rotation;
	}
}
