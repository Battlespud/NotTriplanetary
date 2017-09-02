using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Seasons{
	Spring,
	Summer,
	Autumn,
	Winter
};


public class Clock : MonoBehaviour {



	public const float TurnLength =5f; //time in seconds
	float timer = 0f;
	public int TurnCounter = 0;
	int subCounter = 0;

	Seasons season;
	int week;
	int Year = 800;

	public UnityEvent TurnEvent = new UnityEvent();


	// Use this for initialization
	void Start () {
	}

	public string GetDate(){
		return week + " " + season.ToString() + " " + Year;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer >= TurnLength) {
			timer -= TurnLength;
			TurnEvent.Invoke ();
			TurnCounter++;
			subCounter++;
			if (subCounter >= 10) {
				week++;
				subCounter = 0;
				if (week >= 4) {
					week = 0;
					if (season == Seasons.Winter) {
						season = Seasons.Spring;
						Year++;
					}
					else {
						season = (Seasons)(int)season++;
					}
				}
			}
		}
	}
}


