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
			s.strength -= dam;
			if (s.strength < 0f)
				s.strength = 0f;
		} else {
			if (WallScreen.strength > 0f) {
				WallScreen.strength -= dam;
				if (WallScreen.strength < 0f)
					WallScreen.strength = 0f;
			} else {
				parent.integrity -= Random.Range (20f, 110f) * dam;		
				if (parent.integrity <= 0f) {
					parent.ship.SpawnDebris (source);
				}
			}

		}
	}
}