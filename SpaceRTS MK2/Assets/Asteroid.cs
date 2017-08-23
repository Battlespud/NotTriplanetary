using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Asteroid : MonoBehaviour, IMineable {

	public ResourceDeposit deposit;

	public Text stockpileText;


	// Use this for initialization
	void Awake () {
		deposit = new ResourceDeposit ((RawResources)Random.Range (1, 6));
		gameObject.name = deposit.resource.ToString () + " Asteroid";
		Collections.Mineable.Add (this);
	}

	public float Mine(RawResources r,float amount){
		if (deposit.resource != r)
			return 0f;
		amount = amount * deposit.access;
		if (amount <= deposit.amount) {
			deposit.amount -= amount;
			UpdateText ();
			return amount;
		}
		return  0f;
	}

	void UpdateText(){
		stockpileText.text = "Resources||\n";
		foreach (Mine m in GetComponents<Mine>()) {
			stockpileText.text += string.Format ("{0}: {1} \n", m.resource.ToString (), m.Stockpile );
			}
		}

	}

