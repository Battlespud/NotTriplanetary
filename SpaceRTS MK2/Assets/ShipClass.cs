using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GeneralDirection {
	None,
	Forwards,
	Back,
	Left,
	Right,
	Up,
	Down
}

class Screen{
	public float strength;
	public float mStrength;
	Color color;
	public Screen(float m ){
		mStrength = m;
		strength = mStrength;
		color = Color.white;
	}
}

class Screens{
	public Dictionary<GeneralDirection,Screen> dic = new Dictionary<GeneralDirection,Screen> ();
	ShipClass parent;
	Screen ForeScreen = new Screen (10); 
	Screen AftScreen = new Screen (10);
	Screen PortScreen = new Screen (10);
	Screen StarboardScreen = new Screen (10);
	public Screen WallScreen = new Screen(20);
	public Screens(ShipClass p){
		parent = p;
		dic.Add (GeneralDirection.Forwards, ForeScreen);
		dic.Add (GeneralDirection.Back, AftScreen);
		dic.Add (GeneralDirection.Left, PortScreen);
		dic.Add (GeneralDirection.Right, StarboardScreen);
	}
	public void Damage(float dam, Screen s){
		if (s.strength >= 0f) {
			s.strength -= dam;
			if (s.strength < 0f)
				s.strength = 0f;
		} else {
			if (WallScreen.strength >= 0f) {
				WallScreen.strength -= dam;
				if (WallScreen.strength < 0f)
					WallScreen.strength = 0f;
			} else {
				parent.integrity -= Random.Range (2.25f, 10f) * dam;		
			}

		}
	}
}


public class ShipClass : MonoBehaviour {

	public float integrity = 100f;



	public void Damage (float dam, Vector3 source){
		GeneralDirection dir = GetDirection(transform.position,source);
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



	public static GeneralDirection GetDirection (Vector3 ourPosition, Vector3 PositionShotFrom) {

		GeneralDirection result = GeneralDirection.None;
		float shortestDistance = Mathf.Infinity;
		float distance = 0;

		Vector3 vectorPosition = ourPosition + PositionShotFrom;

		distance = Mathf.Abs (((ourPosition + Vector3.forward) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Forwards;
		}
		distance = Mathf.Abs (((ourPosition  -Vector3.forward) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Back;
		}
		distance = Mathf.Abs (((ourPosition + Vector3.up) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Up;
		}
		distance = Mathf.Abs (((ourPosition + -Vector3.up) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Down;
		}
		distance = Mathf.Abs (((ourPosition + Vector3.left) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Left;
		}
		distance = Mathf.Abs (((ourPosition + Vector3.right) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Right;
		}

		return result;

	}
}
