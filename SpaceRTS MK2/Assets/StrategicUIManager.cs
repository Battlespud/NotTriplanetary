using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class StrategicUIManager : MonoBehaviour {

	public static void ZoomTo(Vector3 pos){
		Camera.main.gameObject.transform.position = new Vector3 (pos.x, 120f, pos.z - 50f);
	}

	public static StrategicUIManager Manager;
	public static UnityEvent UpdateUIEvent = new UnityEvent ();
	public static void UpdateUI (){
		UpdateUIEvent.Invoke ();
	}


	public Text FleetName;
	public Text ShieldStatus;
	public Toggle RaiseShields;
	public Text FleetShipNamesText;
	public GameObject UIPanel;

	public GameObject DesignUI;
	public void ToggleDesignUI(){
		DesignUI.active = !DesignUI.active;
		if (!DesignUI.active) {
			StrategicClock.Unpause ();
		} else {
			StrategicClock.RequestPause ();
		}
	}


	public GameObject BattleUI;

	void TurnManagement(Phase p){
		UpdateUI ();
		switch (p) {
		case(Phase.ORDERS):
			{
				RaiseShields.interactable = true;
				break;
			}
		case(Phase.GO):
			{
				RaiseShields.interactable = false;
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

	void Awake(){
		Manager = this;
		StrategicClock.PhaseChange.AddListener (TurnManagement);

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
