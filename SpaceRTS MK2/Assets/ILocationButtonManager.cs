using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ILocationButtonManager : MonoBehaviour {
	public OfficerManagerUI Manager;
	public ILocation Loc;

	public void Select(){
		Manager.SelectLocation (Loc);
		Update ();
	}

	public void Assign(ILocation c){
		Loc = c;
		Text t = GetComponentInChildren<Text> ();
		try{
			t.text = Loc.GetLocationName() + "\t[" + Loc.GetLocType().FullName+"]";
			t.fontSize = 20;
		}
		catch{
		}
		t.resizeTextForBestFit = true;
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
		t = GetComponent<RectTransform> ();
		if (Manager.SelectedChar.Location == Loc) {
			cb.normalColor = Color.yellow;
		}
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
