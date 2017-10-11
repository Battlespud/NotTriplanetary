using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelTank  {

	public float mFuel;
	public float fuel;

	public bool UseFuel(float amount){
		if (fuel >= amount) {
			fuel -= amount;
			return true;
		}
		return false;
	}

}
