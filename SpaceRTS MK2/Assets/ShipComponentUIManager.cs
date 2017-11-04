using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipComponentUIManager : MonoBehaviour {

	public DesignScreenManager Manager;

	public ShipComponents comp;
	public Text Name;
	public Text Mass;
	public Text Crew;
	public Text Special;
	public Button button;

	// Use this for initialization
	void Start () {
	//	button.onClick.AddListener (AddShipComponent);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Assign(ShipComponents s){
		Name.text = s.Name;
		Mass.text =s.Mass.ToString() + "Kt";
		Crew.text =s.CrewRequired.ToString ();
		comp = s;
	}

	public void AddShipComponent(){
		if (comp.Category == CompCategory.ENGINE) {
			Manager.AddEngine (comp);
		} else {
			Manager.AddComponent (comp);
		}
	}

	public void RemoveShipComponent(){
		//	Manager.RemoveComponent(comp);
		}
	}
