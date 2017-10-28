using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class StrategicUIManager : MonoBehaviour {

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
