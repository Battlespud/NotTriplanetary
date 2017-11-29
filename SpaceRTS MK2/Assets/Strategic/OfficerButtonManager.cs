using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OfficerButtonManager : MonoBehaviour {

	public OfficerManagerUI Manager;
	public Character Officer;

	public void Select(){
		Manager.SelectChar (Officer);
		Update ();
	}

	public void Assign(Character c){
		Officer = c;
		Text t = GetComponentInChildren<Text> ();
		try{
			t.text = Officer.GetNameString(true);
			t.fontSize = 20;
		}
		catch{
		}
		//t.resizeTextForBestFit = true;
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
		if (Officer.Noble) {
			Color NobleColor = new Color (255f, 231f, 154f);
			cb.normalColor = Color.yellow;

		}
		b.colors = cb;
	}

	Color DefaultColor;
	Color SelectedColor;

	RectTransform t;

	// Update is called once per frame
	void Update () {
		//if (gameObject.active)
		//	t.rotation = Camera.main.transform.rotation;
	}
}
