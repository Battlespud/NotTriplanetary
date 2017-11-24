using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShipComponentUIManager : MonoBehaviour {

	public DesignScreenManager Manager;

	public ShipComponents comp;
	public Text Name;
	static Vector3 NamePosition = new Vector3 (-1192,0,0);
	public Text Mass;
	static Vector3 MassPosition = new Vector3 (-566,0,0);
	public Text Category;
	static Vector3 CategoryPosition = new Vector3 (-157,0,0);
	public Text Crew;
	static Vector3 CrewPosition = new Vector3 (126,0,0);
	public Text Description;
	static Vector3 DescriptionPosition = new Vector3 (926,0,0);
	public Button button;

	Image I;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	//	transform.rotation = Camera.main.transform.rotation;
		if (Manager.AddedComponents.ContainsKey(comp)) {
			I.color = Color.green;
		} else {
			I.color = Color.white;
		}
	}

	public void Assign(ShipComponents s){
	//	button.onClick.AddListener (AddShipComponent);
		I = GetComponent<Image>();
		Name.gameObject.GetComponent<RectTransform> ().localPosition = NamePosition;
		Mass.gameObject.GetComponent<RectTransform> ().localPosition = MassPosition;
		Category.gameObject.GetComponent<RectTransform> ().localPosition = CategoryPosition;
		Crew.gameObject.GetComponent<RectTransform> ().localPosition = CrewPosition;
		Description.gameObject.GetComponent<RectTransform> ().localPosition = DescriptionPosition;

		Name.text = s.Name;
		Mass.text =s.Mass.ToString() + "Kt";
		Crew.text =s.CrewRequired.ToString ();
		Category.text = s.Category.ToString ();
		Description.text = s.Description;
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
