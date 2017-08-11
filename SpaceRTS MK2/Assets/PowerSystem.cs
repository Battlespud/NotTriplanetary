﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerSystem {

	public struct FuelTank{
		float mFuel;
		float Fuel;
		public bool UseFuel(float a){
			if (a < Fuel) {
				Fuel -= a;
				return true;
			} else {
				return false;
			}
		}
		public FuelTank(float m){
			mFuel = m;
			Fuel = mFuel;
		}
	}

	struct Generator{
		float mOutput;
		float eff;
		public float GetOutput(){
			return eff * mOutput;
		}
		public Generator(float mout){
			mOutput = mout;
			eff = 100f;
		}
	}

	struct Battery{
		public float charge;
		float mCharge;
		public void Recharge(float input){
			charge += input;
			if (charge > mCharge)
				charge = mCharge;
		}
		public Battery(float m){
			mCharge = m;
			charge = 0f;
		}
	}

	Generator generator = new Generator(10f);
	Battery battery = new Battery(10f);

	public bool UsePower(float a){
		if (generator.GetOutput () > a) {
			return true;
		}
		else if (generator.GetOutput() + battery.charge > a) {
			a -= generator.GetOutput ();
			battery.charge -= a;
			return true;
		}
		return false;
	}

	public PowerSystem(){

	}

}
