﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipButtonManager : MonoBehaviour {

	public StrategicShipUIManager Manager;
	public StrategicShip S;

	public void Select(){
		Manager.SelectShip(S);
		Update ();
	}

	public void Assign(StrategicShip c){
		S = c;
		Text t = GetComponentInChildren<Text> ();
		try{
			t.text = S.ShipName;
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
		cb.highlightedColor = Color.yellow;
		cb.normalColor = Color.grey;
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
