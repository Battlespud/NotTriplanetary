using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipComponents {

	public const float HealthCutoff = .4f; //Below this health components will not function

	public int Mass; //kilotons
	public bool Enabled;
	public float PassiveSig;

	public Dictionary<RawResources, float> Cost = new Dictionary<RawResources, float>();
	public float Health = 1f;
	public bool isFunctional(){
		return Health >= HealthCutoff;
	}
}
