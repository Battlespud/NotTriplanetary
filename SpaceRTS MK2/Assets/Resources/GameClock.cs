using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum TimeState{
	NIGHT =0, //10PM - 6AM
	MORNING,  //6 AM - 11AM
	AFTERNOON,//11AM - 4PM
	EVENING   //4 PM - 10PM
};



public class GameClock : MonoBehaviour {

//THIS MUST BE TAGGED AS "Clock" in scene!!

	//Default timescale is 1 min = hr. 


	public UnityEvent HourChangeEvent;

	[Tooltip("How many seconds in an hour. Strongly recommended not to change while running")]
	public float timeScale = 10; //how many seconds to 1 hour. //45 will be final, anything else is just testing

	public TimeState timeState;

	//timestate constants
	 const int NightS = 22;
	 const int NightE = 5;

	 const int MorningS = 6;
	 const int MorningE = 10;

	 const int AfternoonS = 11;
	 const int AfternoonE = 16;

	 const int EveningS = 16;
	 const int EveningE = 21;

	public int day = 0;
	public int hour = 1; //24 = midnight is always the previous day
	public const int HoursInDay = 24;

	public float counter;
	float overflowCounter; //catch any overflow and add back to system.
	public float cumulativeOverflow; //just curious about how far we'd drift without the overflow counter
	//about 1% drift at 1 second intervals. Gets exponentially worse as intervals decrease. Not really an issue in practice, could cause problems with a time accelerator down the line though.
	//we should be good at anything up to 20x speed, assuming timescale = 60;


	// Use this for initialization
	void Start () {
		initializeEvents ();
	}
	
	// Update is called once per frame
	void Update () {
		counter += Time.deltaTime;
		if (counter >= timeScale) 
			AdvanceHour ();
		}

	void AdvanceHour(){
		hour++;
		HourChangeEvent.Invoke ();
		//catch the overflow and add it back to the counter for the next loop. add to cumulative so we can see how much it drifts.
		overflowCounter = counter - timeScale;
		cumulativeOverflow += overflowCounter;
		counter = overflowCounter;
		SetTimeState ();
		if (hour > HoursInDay) {
			AdvanceDay ();
		}
	//	Debug.Log ("hour change event invoked");
	}

	void AdvanceDay(){
		hour = 1;
		day++;
		HourChangeEvent.Invoke ();
	}

	void SetTimeState(){
		if (hour >= NightS || hour <= NightE)
			timeState = TimeState.NIGHT;
		if (hour >= MorningS &&  hour <= MorningE)
			timeState = TimeState.MORNING;
		if (hour >= AfternoonS && hour <= AfternoonE)
			timeState = TimeState.AFTERNOON;
		if (hour >= EveningS && hour <= EveningE)
			timeState = TimeState.EVENING;
	}


	static public Vector2 GetHours(TimeState time){
		switch (time) {
		case(TimeState.MORNING):
			{
				return new Vector2 (MorningS, MorningE);
			}
		case(TimeState.AFTERNOON):
			{
				return new Vector2 (AfternoonS, AfternoonE);
			}
		case(TimeState.EVENING):
			{
				return new Vector2 (EveningS, EveningE);
			}
		case(TimeState.NIGHT):
			{
				return new Vector2 (NightS, NightE);
			}
		default:
		//	Debug.Log ("Something fucked up with timestates");
			return new Vector2 (MorningS, MorningE);
		}
	}

	//Events

	public void initializeEvents(){
		HourChangeEvent = new UnityEvent (); //BROKEN TODO
	}


}
