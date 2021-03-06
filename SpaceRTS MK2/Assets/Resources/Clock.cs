﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum Seasons{
	Spring,
	Summer,
	Autumn,
	Winter
};


public class Clock : MonoBehaviour {


	public string Date;

	public const float TurnLength =2f; //time in seconds
	float timer = 0f;
	public int TurnCounter = 0;
	int subCounter = 0;

	Seasons season;
	int week;
	int Year = 800;

	public UnityEvent TurnEvent = new UnityEvent();


	// Use this for initialization
	void Awake () {
		ClockStatic.clock = this;
	}

	public string GetDate(){
		return  week + " " + season.ToString() + " " + Year;
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer >= TurnLength) {
			timer -= TurnLength;
			TurnEvent.Invoke ();
			TurnCounter++;
			subCounter++;
			if (subCounter >= 2) {
				week++;
				subCounter = 0;
				if (week >= 20) {
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
			Date = GetDate ();
		}
	}
}


