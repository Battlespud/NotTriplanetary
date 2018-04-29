using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentPassThrough : MonoBehaviour {

	//Simple container for keeping the component associated with a particular button and passing it along once the button is pressed.  
	//Should never have to add these manually, should all be handled by DesignScreenManager when it populates the component list.

	public	ShipComponents component;
	public DesignScreenManager Manager;

	void Start(){
	}

	public void AddShipComponent(){
		if (component.Category == CompCategory.ENGINE) {
			Debug.Log ("An Engine");
			Manager.AddEngine (component);
		} else {
			Debug.Log ("Not an engine");
			Manager.AddComponent (component);
		}
	}

}
