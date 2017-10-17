using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleBlock : MonoBehaviour {

	public static Color ActiveC = Color.red;
	public static Color InactiveC = Color.green;

	public bool Active = false;

	Renderer ren;

	//coords
	public int x;
	public int y;

	public void SetOrder(int x){
		Order = x;
		OrderText.text = Order.ToString ();
	}

	int Order =0;
	public Text OrderText;

	public void Toggle(){
		Active = !Active;
		if (Active) {
			ren.material.color = ActiveC;
			OrderText.text = Order.ToString ();
			OrderText.enabled = true;
		} else {
			ren.material.color = InactiveC;
			OrderText.enabled = false;

		}
	}

	public void Delete(){
		Destroy (this);
	}

	// Use this for initialization
	void Start () {
		ren = GetComponent<Renderer> ();
		ren.material.color = InactiveC;
		OrderText.enabled = false;
	}
	

}
