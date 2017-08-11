using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public enum ShipPrefabTypes{
	DEF, //defensive wall ship
	DD, //destroyer 
	CS, //strike cruiser
	DN, //dreadnought
	CV, //carrier
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
		screens.Damage (f, screens.dic[Direction.GetDirection (transform.position, origin)], origin);
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



}
