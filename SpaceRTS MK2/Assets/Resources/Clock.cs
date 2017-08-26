﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Clock : MonoBehaviour {



	public const float TurnLength =5f; //time in seconds
	float timer = 0f;

	public UnityEvent TurnEvent = new UnityEvent();


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		timer += Time.deltaTime;
		if (timer >= TurnLength) {
			timer -= TurnLength;
			TurnEvent.Invoke ();
		}
	}
}
