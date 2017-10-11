using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileTube : ShipComponents {

	public float MaxSize;

	public Missile LoadedMissile;

	public void FireMissile(GameObject source, ICAPTarget cap){
		GameObject G = GameObject.Instantiate (Missile.MissilePrefab);
		MissileControl M = G.AddComponent<MissileControl> ();
		M.MissileEngine = LoadedMissile.MissileEngine;
		M.MissileFuelTank = LoadedMissile.MissileFuel;
		M.MissileWarhead = LoadedMissile.MissileWarhead;
		M.Launch (source, cap);
	}

	public void LoadMissile(MissileMagazine Magazine){
		if (LoadedMissile == null) {
			LoadedMissile = Magazine.LoadMissile ();
		}
	}


}
