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
	public Dictionary<GeneralDirection,Screen> dic = new Dictionary<GeneralDirection,Screen> ();
	public ShipClass parent;
	Screen ForeScreen = new Screen (2); 
	Screen AftScreen = new Screen (0);
	Screen PortScreen = new Screen (1);
	Screen StarboardScreen = new Screen (1);
	public Screen WallScreen = new Screen(2);
	public Screens(ShipClass p){
		parent = p;
		dic.Add (GeneralDirection.Forwards, ForeScreen);
		dic.Add (GeneralDirection.Back, AftScreen);
		dic.Add (GeneralDirection.Left, PortScreen);
		dic.Add (GeneralDirection.Right, StarboardScreen);
	}
	public void Damage(float dam, Screen s, Vector3 source){
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
				parent.integrity -= Random.Range (20f, 110f) * dam;		
				if (parent.integrity <= 0f) {
					parent.ship.SpawnDebris (source);
				}
			}

		}
	}
	public bool ScreensWillHold(float dam, Vector3 source){
		if (dic [Direction.GetDirection (parent.transform.position, source)].strength >= dam)
			return true;
		return false;
	}
}