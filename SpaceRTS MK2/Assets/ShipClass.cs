using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum GeneralDirection {
	None,
	Forwards,
	Back,
	Left,
	Right,
	Up,
	Down
}

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

public class Tractor{
	public Ship target;
	public bool active;
	public float mRange;
	public float force = 5f;
	public Tractor(){
		active = false;
		mRange = 20f;
	}
}


public class ShipClass : MonoBehaviour {

	public float integrity = 100f;
	public Ship ship;

	//just for inspector testing. These show the strength of the corresponding screens.
	public float Fore;
	public float Aft;
	public float Star;
	public float Port;
	public float Wall;

	public Screens screens;


	//Resources and parts
	public int crew;
	public int mCrew;

	public int marines;
	public int mMarines;

	public PowerSystem Power = new PowerSystem();
	public Tractor tractor = new Tractor();
	//

	public List<float> ScreenStrengthsUI = new List<float>();

	// Use this for initialization
	void Start () {
		screens  = new Screens(this);
		ship = GetComponent<Ship> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (tractor.active)
			TractorLoop ();
	}





	void UpdateScreenUI(){
		Fore = screens.dic [GeneralDirection.Forwards].strength;
		Aft = screens.dic [GeneralDirection.Back].strength;
		Star = screens.dic [GeneralDirection.Right].strength;
		Port = screens.dic [GeneralDirection.Left].strength;
		Wall = screens.WallScreen.strength;
	}

	public void Damage(float f, Vector3 origin){
		screens.Damage (f, screens.dic[GetDirection (transform.position, origin)], origin);
	}




















	//Tractorbeams
	void TractorLoop(){
		Debug.DrawLine (transform.position, tractor.target.transform.position, Color.green);
		if (tractor.active && !Power.UsePower (11f * Time.deltaTime)) {
			DeactivateTractor();
		}
		if (tractor.active) {
			Vector3 dir = tractor.target.transform.position - transform.position;
			Rigidbody rb = tractor.target.GetComponent<Rigidbody> ();
			if(Vector3.Distance(transform.position,tractor.target.transform.position) > tractor.mRange - tractor.mRange*.75f)
				rb.AddForce (dir*-1f*tractor.force*(Mathf.Sqrt(Vector3.Distance(transform.position,tractor.target.transform.position))));

		}
	}

	public void ActivateTractor(Ship targ){
		if (Vector3.Distance (transform.position, targ.transform.position) > tractor.mRange || targ.underTractor){
			return;
		}
		if (tractor.target)
			DeactivateTractor ();
		tractor.target = targ;
		tractor.active = true;
		targ.underTractor=true;
		//targ.transform.parent = ship.transform;
		ship.usingTractor = true;
		Rigidbody rb = tractor.target.GetComponent<Rigidbody> ();
		rb.drag = 1f;
		//	targ.Agent.enabled =(false);
	}

	public void DeactivateTractor(){
		if (!tractor.target)
			return;
		//tractor.target.Agent.enabled = true;
		ship.usingTractor = false;
		tractor.active = false;
		tractor.target.underTractor = false;
		Rigidbody rb = tractor.target.GetComponent<Rigidbody> ();
		rb.drag = 0f;
		//tractor.target.transform.parent = null;

	}








	//ToDo not quite accurate. Doesnt account for rotation for some reason. Probably an issue in inputs, not the formula itself.
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
