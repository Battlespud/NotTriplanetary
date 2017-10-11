using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileMagazine : ShipComponents {



	public List<Missile> Missiles = new List<Missile>();

	public Missile LoadMissile(){
		Missile m = Missiles [0];
		Missiles.Remove (m);
		return m;
	}

}
