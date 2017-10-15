using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.IO;


public class ShipClass : MonoBehaviour {
	public static ShipDesign DebugDesignTemplate;

	public const int MaxAttempts = 20; //After this many failed DAC rolls, the ship will be considered destroyed.

	public static int[]MaxCrews = new int[5]{200, 250, 450, 800, 1200};

	public Ship ship;
//	public ShipPrefabTypes BaseClass;

	//just for inspector testing. These show the strength of the corresponding screens.
	public float Fore;
	public float Aft;
	public float Star;
	public float Port;
	public float Wall;
	public float BatteryPower;

	public Screens screens;


	//Resources and parts
	public int crew;
	public int mCrew;

	public int survivors = 0;

	public int marines;
	public int mMarines;

	public int Torpedos = 12;


	public PowerSystem Power = new PowerSystem();
	public Tractor tractor = new Tractor();

	//REWORK
	//COMPONENT BASED DESIGN OVERHAUL
	#region Components
	ShipDesign DesignTemplate;
	public List<ShipComponents> Components = new List<ShipComponents>();
	public List<ShipComponents> DamagedComponents = new List<ShipComponents>();

	public Dictionary<int,ShipComponents> DAC = new Dictionary<int, ShipComponents> ();
	public Dictionary<ShipComponents,Range>DACRanges = new Dictionary<ShipComponents, Range>();

	public string ShipName;
	public string ShipClassName;
	public HullDes HullDesignation;

	public int MaxDAC; //exclusive, actual highest value is 1 less.

	public void ImportDesign(ShipDesign template){
		ShipClassName = template.DesignName;
		HullDesignation = template.HullDesignation;
		DesignTemplate = template;
		foreach (ShipComponents c in template.Components) {
			ShipComponents copy = c.CloneProperties ();
			Components.Add (copy);
		}
		SetupDAC ();
	}

	public void SetupDAC(){
		int curr = 0;
		foreach (ShipComponents c in Components) {
			Debug.Log ("Registering for DAC: " + c.name);
			int pCounter = 0;
			int start = curr;
			for (int i = 0; c.Mass > i; i += ShipDesign.Slot) {
				DAC.Add (curr, c);
				pCounter++;
				curr++;
			}
			int end = curr-1;
			DACRanges.Add(c,new Range(start,end));
			Debug.Log (c.name + " given " + pCounter + " slots. " + DACRanges[c].ToString());
		}
		MaxDAC = curr; //exclusive
	}

	public ShipComponents RollDAC(){
		int i = Random.Range (0, MaxDAC);
		Debug.Log ("Rolled #" + i + " " + DAC[i].name);
		return DAC [i];
	}

	public void TakeComponentDamage(int amount){
		if (DesignTemplate == null) {
			return;
		}
		int attempts = 0;
		ShipComponents target;
		while (amount > 0) {
			target = RollDAC ();
			while (target.isDamaged ()) {
				target = RollDAC ();
				attempts++;
				if (attempts > MaxAttempts) {
					Debug.Log ("Ship destroyed from excessive damage");
					ship.SpawnDebris (ship.transform.position);
					break;
				}
			}
			if (amount >= target.GetHTK ()) {
				target.Damage ();
				DamagedComponents.Add (target);
				amount -= target.GetHTK ();
			} else if (amount < target.GetHTK ()) {
				float chance = amount / target.GetHTK ();
				if (Random.Range (0f, 1f) < chance) {
					target.Damage ();
					DamagedComponents.Add (target);
				}
				amount = 0;
			}
		}
		OutputReport ();
	}


	public void OutputReport(){
		Debug.Log ("Printing Ship Report of " + ship.ShipName + " to text file..");
		string path="Assets/Output/Reports/" + ship.ShipName + ".txt";
		using(StreamWriter writer = new StreamWriter(path)){
			writer.WriteLine ("Active Ship Report");
			writer.WriteLine ( "\n" + HullDesignation.Prefix + " " + ShipName + "-Class "+  HullDesignation.HullType + "\nMass: " + DesignTemplate.mass + "KT\nArmor Thickness: " +DesignTemplate.ArmorLayers +"\n" + "Crew: " + DesignTemplate.CrewMin 
				+ "\nSpare Berths: " + (DesignTemplate.CrewBerths - DesignTemplate.CrewMin) + "\n");
			foreach (ShipComponents c in Components) {
				writer.WriteLine (DACRanges[c].ToString() + " " + c.name + ": " + c.isDamaged().ToString());
			}
			writer.Close ();
		}
		Debug.Log ("Done. Check " + path);
	}
	//

	#endregion


	public List<float> ScreenStrengthsUI = new List<float>();

	// Use this for initialization
	void Awake () {
	//	BaseClass = ship.BaseType;
		screens  = new Screens(this);
		screens.abs = GetComponent<ShipAbstract> ();
		ship = GetComponent<Ship> ();
	//	mCrew = MaxCrews [(int)BaseClass];
		mCrew = 200;
		crew = mCrew;
		if (DebugDesignTemplate != null) {
			ImportDesign (DebugDesignTemplate); //TODO
		}
	}

	// Update is called once per frame
	void Update () {
		if (tractor.active)
			TractorLoop ();
		BatteryPower = Power.GetBatteryPower ();
		Power.GeneratePower ();
		UpdateScreenUI ();


		//DEBUG ONLY
		if (Input.GetKeyDown (KeyCode.N)) {
			TakeComponentDamage (15);
		}
	}



	public void ScreenProxyDelete(GameObject g){
		StartCoroutine ("Delete",g);
	}

	public IEnumerator Delete(GameObject g){
		float a = 0f;
		while (a < .5f) {
			a += Time.deltaTime;
			yield return null;
		}
		GameObject.Destroy (g);
	}

	void UpdateScreenUI(){
		Fore = screens.dic [GeneralDirection.Forwards].strength;
		Aft = screens.dic [GeneralDirection.Back].strength;
		Star = screens.dic [GeneralDirection.Right].strength;
		Port = screens.dic [GeneralDirection.Left].strength;
		Wall = screens.WallScreen.strength;
	}
	public string[] ScreenOrder = new string[5]{"Fore","Aft","Port","Star","Wall"};
	public float[] StartingScreenStrengths= new float[5];

	public void Damage(float dam, Vector3 origin, Transform en, List<Vector2> pattern){
		screens.Damage (dam, screens.dic[Direction.GetDirection (origin,en,transform.position,transform)], origin, pattern);
	}

	public void PhysicsDamage(float f, Vector3 origin, float fMag, Transform s, List<Vector2> pattern){
		screens.PhysicsDamage (f, screens.dic[Direction.GetDirection (origin,s,transform.position,transform)], origin,(transform.position - origin)*fMag,s, pattern);
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
