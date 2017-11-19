using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraitButtonManager : MonoBehaviour {

	public Trait trait;
	public TraitManager Manager;

	Text tx;
	Button b;

	public void Assign(TraitManager t, Trait td){
		Manager = t;
		trait = td;
		tx = GetComponentInChildren<Text> ();
		b = GetComponent<Button> ();
		b.onClick.AddListener (Select);
		tx.fontSize = 16;
		tx.GetComponent<RectTransform> ().localScale = new Vector3 (.75f, 1f, 1f);

		tx.text = string.Format ("{0}",trait.Name);
	}

	public void Select(){
		Manager.SelectTrait (trait);
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
