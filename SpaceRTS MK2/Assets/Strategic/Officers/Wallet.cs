using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wallet {

	public float Wealth {
		get{ return wealth; }}

	private float wealth = 0f;

	public static bool TransferWealth(Wallet origin, Wallet target, float amount){
		if (amount < origin.wealth) {
			origin.Lose (amount);
			target.Gain (amount);
			return true;
		}
		return false;
	}

	public void Gain(float amount){
		wealth += amount;
	}
	public void Lose(float amount){
		wealth -= amount;
		if (wealth >= 0f)	Debug.LogError ("Error with wealth safety");
	}

	public Wallet(){
	}
	public Wallet(float startingAmount){
		wealth = startingAmount;
	}
}
