using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Phase{
	ORDERS,
	GO,
	INTERRUPT,
	REVIEW
}



public class StrategicClock : MonoBehaviour {

	public static StrategicClock strategicClock;


	public static PhaseEvent PhaseChange = new PhaseEvent();
	public static BoolEvent PauseEvent = new BoolEvent ();


	static string[]Months = new string[12]{"January","February","March","April","May","June","July","August","September","October","November","December"};

	public Text PhaseText;

	//Turn = month

	public int TurnNumber=1;

	static int month = 1;
	static int year = 2700;

	public static bool isPaused = false;
	float TurnLengthBase = 2f; //30
	float GoTurnLength = 5f; //10f
	float LengthMultiplier = 1f;
	float TimeRemaining = 0f;

	public bool TimedTurns = false;

	public Phase currPhase = Phase.ORDERS;
	public static Phase GetPhase(){
		return strategicClock.currPhase;
	}

	void Awake(){
		strategicClock = this;
		PhaseChange.AddListener (TurnManagement);
		PauseEvent.AddListener (Pause);
	}

	// Use this for initialization
	void Start () {
		EventInvoke (currPhase);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (!isPaused) {
				RequestPause ();
			}
			else{
				Unpause ();		
			}
		}
	}


	public static void RequestPause(){
		PauseEvent.Invoke (true);
	}

	public static void Unpause(){
		PauseEvent.Invoke (false);
	}

	public static string GetDate(){
		return Months[month] +" " + year;
	}


	void EventInvoke(Phase p){
		currPhase = p;
		PhaseText.text = currPhase.ToString ();
		PhaseChange.Invoke (currPhase);
	}

	void Pause(bool b){
		isPaused = b;
	}



	IEnumerator OrdersPhase(){
		TimeRemaining = TurnLengthBase * LengthMultiplier;

		while (TimeRemaining >= 0) {
			if(!isPaused)
				TimeRemaining -= Time.deltaTime;
			yield return null;
		}
		EndTurn ();
	}

	IEnumerator GoPhase(){
			TimeRemaining = GoTurnLength;
			while (TimeRemaining >= 0) {
			PhaseText.text =TimeRemaining.ToString("#.00");
				if (!isPaused)
					TimeRemaining -= Time.deltaTime;
				yield return null;
			}
		EventInvoke (Phase.ORDERS);
		yield return null;
	}

	public void EndTurn(){
		if (currPhase == Phase.ORDERS) {
			StopAllCoroutines ();
			EventInvoke (Phase.GO);
		}
	}

	public void Interrupt(){
		EventInvoke (Phase.INTERRUPT);
	}

	void EndReview(){
		EventInvoke (Phase.ORDERS);
	}

	void TurnManagement(Phase p){
		Debug.Log ("Turn Manager is starting the " + p.ToString () + " phase.");
		switch (p) {
		case(Phase.ORDERS):
			{
				ProgressTime ();
				if (TimedTurns) {
					StartCoroutine (OrdersPhase ());
				}
				break;
			}
		case(Phase.GO):
			{
				StartCoroutine (GoPhase ());
				break;
			}
		case (Phase.REVIEW):
			{
				//todo
				EndReview();
				break;
			}
		case (Phase.INTERRUPT):
			{
				//dont use
				break;
			}

		}
	}

	void ProgressTime(){

		TurnNumber++;
		month++;
		if (month > 12) {
			month = 1;
			year++;
		}
	}

}
