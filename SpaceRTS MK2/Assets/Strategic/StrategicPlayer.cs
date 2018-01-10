using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class StrategicPlayer : MonoBehaviour {


	public Fleet SelectedFleet;

	Vector3 MousePosition;
	public bool Shift;
	int layer_mask;


	// Use this for initialization
	void Start () {
		StrategicUIManager.UpdateUIEvent.AddListener (UpdateUI);
		StrategicUIManager.Manager.RaiseShields.onValueChanged.AddListener (TryShields);
		StrategicClock.PhaseChange.AddListener (TurnManagement);
		layer_mask = LayerMask.GetMask ("GroundLayer");
	}
	
	void TurnManagement(Phase p){
		UpdateUI ();
				switch (p) {
				case(Phase.ORDERS):
					{
				if (SelectedFleet != null) {
					Camera.main.ZoomTo(SelectedFleet);
				}
						break;
					}
				case(Phase.GO):
					{
						break;
					}
				case (Phase.REVIEW):
					{
						break;
					}
				case (Phase.INTERRUPT):
					{
						break;
					}

				}
			}

	// Update is called once per frame
	void Update () {
		MousePosition = Input.mousePosition;
		Shift = Input.GetKey (KeyCode.LeftShift);
		if(Click()){
			TrySelect ();
		}
		if (RightClick ()) {
			TryMove ();
		}
	}

	void TrySelect(){
		if (EventSystem.current.IsPointerOverGameObject ()) {
		//	Debug.Log ("Click over UI (Anything child'd to Controller will block clicks from world)");
			return;
		}// is the touch on the GUI
		RaycastHit hit;
		Ray ray = Camera.main.ScreenPointToRay (MousePosition);
		if(SelectedFleet)SelectedFleet.DeselectColor ();
		if (Physics.Raycast (ray, out hit)) {

			if (hit.collider.GetComponent<Fleet>())
			{
				Select(hit.collider.GetComponent<Fleet>());
			}
			else if(hit.collider.GetComponentInParent<HexGrid>())
			{
				hit.collider.GetComponentInParent<HexGrid>().TouchCell(hit.point);
			}
		}
	}


	void NegateSelect(){
		SelectedFleet = null;
		StrategicUIManager.Manager.FleetShipNamesText.text = "";
		StrategicUIManager.Manager.FleetName.text = "";
		StrategicUIManager.Manager.ShieldStatus.text = "";

	}

	void Select(Fleet f){
		SelectedFleet = f;
		SelectedFleet.SelectColor ();
		StrategicUIManager.Manager.FleetShipNamesText.text = SelectedFleet.FleetShipNames;
		StrategicUIManager.Manager.FleetName.text = SelectedFleet.FleetName;
		StrategicUIManager.Manager.ShieldStatus.text = SelectedFleet.ShieldStrength.ToString("P1");
		StrategicUIManager.Manager.RaiseShields.isOn = SelectedFleet.ShieldsUp;
	}

	void UpdateUI(){
		if (SelectedFleet) {
			StrategicUIManager.Manager.FleetShipNamesText.text = SelectedFleet.FleetShipNames;
			StrategicUIManager.Manager.FleetName.text = SelectedFleet.FleetName;
			StrategicUIManager.Manager.ShieldStatus.text = SelectedFleet.ShieldStrength.ToString("P1");
			StrategicUIManager.Manager.RaiseShields.isOn = SelectedFleet.ShieldsUp;
		}
	}

	void TryShields(bool b){
		if (SelectedFleet != null) {
			SelectedFleet.ShieldsUp = b;
		}
	}

	void TryMove(){
		if (SelectedFleet != null) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (MousePosition);
			if(SelectedFleet)SelectedFleet.DeselectColor ();
			if (Physics.Raycast (ray, out hit)) {

				if (hit.collider.GetComponent<Fleet> ()) {
					SelectedFleet.OrderIntercept (hit.collider.GetComponent<Fleet> ());
					Debug.Log ("Intercept Plotted");
					return;
				}
				if (hit.collider.GetComponent<Planet> ()) {
				//	SelectedFleet.EnterOrbit (hit.collider.GetComponent<Planet> ());
					return;
				}
				if (hit.collider.GetComponent<StrategicShipyard> ()) {
					SelectedFleet.OrderDock (hit.collider.GetComponent<StrategicShipyard> ());
					return;
				}
			}

			Vector3 target = new Vector3();
			RaycastHit moveHit;
			Ray moveRay = Camera.main.ScreenPointToRay (MousePosition);
			if (Physics.Raycast (ray, out hit, 10000f, layer_mask)) {
				SelectedFleet.AddWaypoint(hit.point, Shift);
				Debug.Log (hit.point);
			}
		}
	}



	bool Click(){
		return Input.GetMouseButtonDown (0);
	}

	bool RightClick(){
		return Input.GetMouseButtonDown (1);
	}

}
