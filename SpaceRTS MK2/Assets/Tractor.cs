using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tractor{
	public Ship target;
	public bool active;
	public float mRange;
	public float force = 5f;
	public Tractor(){
		active = false;
		mRange = 20f;
	}
}
