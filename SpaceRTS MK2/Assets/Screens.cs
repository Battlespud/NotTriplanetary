using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class Screen{
	public float strength;
	public float mStrength;
	Color color;
	public Screen(float m ){
		mStrength = m;
		strength = mStrength;
		color = Color.white;
	}
}

public class Screens{
	//linear transformation formula
	//NewValue = (((OldValue - OldMin) * (NewMax - NewMin)) / (OldMax - OldMin)) + NewMin

	public static GameObject ScreenPrefab;

	bool dead = false;

	public Dictionary<GeneralDirection,Screen> dic = new Dictionary<GeneralDirection,Screen> ();
	public ShipClass parent;
	public ShipAbstract abs;
	public Screen ForeScreen ; 
	public Screen AftScreen ; 
	public Screen PortScreen; 
	public Screen StarboardScreen; 
	public Screen WallScreen; 

	public Screens(ShipClass p){
		parent = p;
			 ForeScreen = new Screen (parent.StartingScreenStrengths[0]); 
			 AftScreen = new Screen (parent.StartingScreenStrengths[1]); 
			 PortScreen = new Screen (parent.StartingScreenStrengths[2]); 
			 StarboardScreen = new Screen (parent.StartingScreenStrengths[3]); 
			 WallScreen = new Screen (parent.StartingScreenStrengths[4]); 
		dic.Add (GeneralDirection.Forwards, ForeScreen);
		dic.Add (GeneralDirection.Back, AftScreen);
		dic.Add (GeneralDirection.Left, PortScreen);
		dic.Add (GeneralDirection.Right, StarboardScreen);
		dic.Add (GeneralDirection.Up, WallScreen);
		dic.Add (GeneralDirection.Down, WallScreen);
	}

	public void Damage(float dam, Screen s, Vector3 source, List<Vector2> pattern){
		GameObject g = GameObject.Instantiate (ScreenPrefab);
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
				dam -= WallScreen.strength;
				WallScreen.strength -= applyDam;
				if (WallScreen.strength < 0f)
					WallScreen.strength = 0f;
			 if(dam > 0f) {
				parent.ship.DamageArmor (pattern, (int)dam);
				if (abs.integrity <= 0f && !dead) {
					dead = true;
					parent.ship.Die ();
					parent.ship.SpawnDebris (source);
				}
			}
		}
	}


	public void PhysicsDamage(float dam, Screen s, Vector3 source, Vector3 force, Transform en, List<Vector2> pattern){
		GameObject g = GameObject.Instantiate (ScreenPrefab);
		LineRenderer l = g.GetComponent<LineRenderer> ();
		g.transform.position = parent.transform.position;
		g.transform.position = Vector3.MoveTowards (g.transform.position, source, .2f);
		parent.ScreenProxyDelete (g);
		if (!ScreensWillHold(dam,source, en)) {
			parent.ship.rb.AddForce (force);
		}
		if (s.strength > 0f) {
			float applyDam = dam;
			dam -= s.strength;
			s.strength -= applyDam;
			if (s.strength < 0f)
				s.strength = 0f;
		} if(dam >0f) {
			if (WallScreen.strength > 0f) {
				float applyDam = dam;
				dam -= WallScreen.strength;
				WallScreen.strength -= applyDam;
				if (WallScreen.strength < 0f)
					WallScreen.strength = 0f;
			} if(dam > 0f) {
				parent.ship.DamageArmor (pattern, (int)dam);
				//parent.integrity -= Random.Range (10f, 25f) * dam;		
				if (abs.integrity <= 0f && !dead) {
					parent.ship.Die ();
					parent.ship.SpawnDebris (source);
					dead = true;
				}
			}

		}
	}



	public bool ScreensWillHold(float dam, Vector3 source, Transform s){
		if (dic [Direction.GetDirection (source, s, parent.transform.position,parent.transform)].strength >= dam)
			return true;
		return false;
	}
}