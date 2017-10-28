using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class Shield{
	public float strength;
	public float mStrength;
	Color color;
	public Shield(float m ){
		mStrength = m;
		strength = mStrength;
		color = Color.white;
	}
}

public class Screens{
	//linear transformation formula
	//NewValue = (((OldValue - OldMin) * (NewMax - NewMin)) / (OldMax - OldMin)) + NewMin

	public static GameObject ShieldPrefab;

	bool dead = false;

	public Dictionary<GeneralDirection,Shield> dic = new Dictionary<GeneralDirection,Shield> ();
	public ShipClass parent;
	public ShipAbstract abs;
	public Shield ForeShield ; 
	public Shield AftShield ; 
	public Shield PortShield; 
	public Shield StarboardShield; 
	public Shield WallShield; 

	public Screens(ShipClass p){
		parent = p;
			 ForeShield = new Shield (parent.StartingScreenStrengths[0]); 
		AftShield = new Shield (parent.StartingScreenStrengths[1]); 
		PortShield = new Shield (parent.StartingScreenStrengths[2]); 
		StarboardShield = new Shield (parent.StartingScreenStrengths[3]); 
		WallShield = new Shield (parent.StartingScreenStrengths[4]); 
		dic.Add (GeneralDirection.Forwards, ForeShield);
		dic.Add (GeneralDirection.Back, AftShield);
		dic.Add (GeneralDirection.Left, PortShield);
		dic.Add (GeneralDirection.Right, StarboardShield);
		dic.Add (GeneralDirection.Up, WallShield);
		dic.Add (GeneralDirection.Down, WallShield);
	}

	public void Damage(float dam, Shield s, Vector3 source, List<Int2> pattern){
		GameObject g = GameObject.Instantiate (ShieldPrefab);
		LineRenderer l = g.GetComponent<LineRenderer> ();
		g.transform.position = parent.transform.position;
		g.transform.position = Vector3.MoveTowards (g.transform.position, source, .2f);
		parent.ScreenProxyDelete (g);
		if (dam > 0f) {
			float applyDam = dam;
			dam -= s.strength;
			s.strength -= applyDam;
			if (s.strength < 0f)
				s.strength = 0f;
		} if(dam >0f) {
				float applyDam = dam;
				dam -= WallShield.strength;
				WallShield.strength -= applyDam;
				if (WallShield.strength < 0f)
					WallShield.strength = 0f;
			 if(dam > 0f) {
				ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(parent,parent.ship.DamageArmor (pattern, (int)dam));
				if (abs.integrity <= 0f && !dead) {
					dead = true;
					parent.ship.Die ();
					parent.ship.SpawnDebris (source);
				}
			}
		}
	}


	public void PhysicsDamage(float dam, Shield s, Vector3 source, Vector3 force, Transform en, List<Int2> pattern){
		GameObject g = GameObject.Instantiate (ShieldPrefab);
		LineRenderer l = g.GetComponent<LineRenderer> ();
		g.transform.position = parent.transform.position;
		g.transform.position = Vector3.MoveTowards (g.transform.position, source, .2f);
		parent.ScreenProxyDelete (g);
		if (!ShieldsWillHold(dam,source, en)) {
			parent.ship.rb.AddForce (force);
		}
		if (s.strength > 0f) {
			float applyDam = dam;
			dam -= s.strength;
			s.strength -= applyDam;
			if (s.strength < 0f)
				s.strength = 0f;
		} if(dam >0f) {
			if (WallShield.strength > 0f) {
				float applyDam = dam;
				dam -= WallShield.strength;
				WallShield.strength -= applyDam;
				if (WallShield.strength < 0f)
					WallShield.strength = 0f;
			} if(dam > 0f) {
				ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(parent,parent.ship.DamageArmor (pattern, (int)dam));
				//parent.integrity -= Random.Range (10f, 25f) * dam;		
				if (abs.integrity <= 0f && !dead) {
					parent.ship.Die ();
					parent.ship.SpawnDebris (source);
					dead = true;
				}
			}

		}
	}



	public bool ShieldsWillHold(float dam, Vector3 source, Transform s){
		if (dic [Direction.GetDirection (source, s, parent.transform.position,parent.transform)].strength >= dam)
			return true;
		return false;
	}
}