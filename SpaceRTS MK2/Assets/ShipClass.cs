using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.IO;

public class ShipClass : MonoBehaviour {

	public const int MaxAttempts = 15; //After this many failed DAC rolls, the ship will be considered destroyed.

	public static System.Random random = new System.Random ();

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

	public string CrewString;

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


	//updates ship stats based on component damage
	IEnumerator ChangeStats(){
		LifeSupport = 0;
		Controllable = false;
		Thrust = 0;
		TurnThrust = 0;
		Mass = (int)DesignTemplate.mass;
		foreach (ShipComponents c in Components) {
			if (!c.isDamaged()) {
				LifeSupport += c.lifeSupport;
				if (c.control || c.flagControl) {
					Controllable = true;
				}
				TurnThrust += c.TurnThrust;
				Thrust += c.Thrust;
			}
		}
		if (LifeSupport < crew) {
			crew = LifeSupport;
		}
		CrewString = string.Format ("Crew: {0}/{1}", crew, mCrew);
		yield return Ninja.JumpToUnity;
	}

	//Current Ship Stats based on components
	public int LifeSupport; //current functioning lifesupport. Any crew over this limit will be killed on update.
	public bool Controllable; //Is there a working bridge
	public float Thrust;  //max thrust from functioning engines
	public float TurnThrust; //Max turnrate from functioning engines/thrusters
	public int Mass;





	//





	public bool usingTemplate = false;
	public void ImportDesign(ShipDesign template){
		usingTemplate = true;
		ShipClassName = template.DesignName;
		HullDesignation = template.HullDesignation;
		DesignTemplate = template;
		foreach (ShipComponents c in template.Components) {
			ShipComponents copy = c.CloneProperties ();
			Components.Add (copy);
		}
		ship.SetupArmor (template.ArmorLength,template.ArmorLayers);
		mCrew = template.CrewMin;
		crew = mCrew;
		SetupDAC ();
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,ChangeStats());
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
		int i = random.Next (0, MaxDAC);
		Debug.Log ("Rolled #" + i + " " + DAC[i].name + " HTK: " + DAC[i].GetHTK());
		return DAC [i];
	}
	/*
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
				crew -= (int)(Random.Range (.75f, 1.2f) * target.CrewRequired);
			} else if (amount < target.GetHTK ()) {
				float chance = amount / target.GetHTK ();
				if (Random.Range (0f, 1f) < chance) {
					target.Damage ();
					crew -= (int)(Random.Range (.25f, .95f) * target.CrewRequired);
					DamagedComponents.Add (target);
				} else {
					crew -= (int)(Random.Range (.15f, .35f) * target.CrewRequired);
				}

				amount = 0;
			}
			if (crew < 0) {
				crew = 0;
				ship.SpawnDebris (transform.position); //TODO make it just go neutral instead of exploding  
			}
		}
		CalculateIntegrity ();
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,ChangeStats());
		OutputReport ();
	}
	*/
	public IEnumerator TakeComponentDamage(int amount){
		int attempts = 0;
		ShipComponents target;
		while (amount > 0) {
			target = RollDAC ();
			while (target.isDamaged ()) {
				target = RollDAC ();
				attempts++;
				if (attempts > MaxAttempts) {
					Debug.Log ("Ship destroyed from excessive damage");
					yield return Ninja.JumpToUnity;
					ship.SpawnDebris (ship.transform.position);
					break;
				}
			}
			if (amount >= target.GetHTK ()) {
				target.Damage ();
				DamagedComponents.Add (target);
				amount -= target.GetHTK ();
				crew -= (int)(random.Next (80, 110) / 100 * target.CrewRequired);
			} else if (amount < target.GetHTK ()) {
				float chance = amount / target.GetHTK ();
				if (random.Next (0, 100)/100 < chance) {
					target.Damage ();
					crew -= (int)(random.Next (35, 95) / 100 * target.CrewRequired);
					DamagedComponents.Add (target);
				} else {
					crew -= (int)(random.Next (5, 25) / 100 * target.CrewRequired);
				}

				amount = 0;
			}
			if (crew < 0) {
				crew = 0;
			//	ship.SpawnDebris (transform.position); //TODO make it just go neutral instead of exploding  
			}
		}
		yield return Ninja.JumpToUnity;
		CalculateIntegrity ();
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,ChangeStats());
		OutputReport ();
	}

	public float Integrity =1f; //flavor only

	void CalculateIntegrity(){
		if (DamagedComponents.Count < 1) {
			Integrity = 1f;
		} else {
			Debug.Log (DamagedComponents.Count + " components are damaged.");
			float total = 0f;
			foreach (ShipComponents c in Components) {
				total += c.GetHTK ();
			}
			float damaged = 0f;
			foreach (ShipComponents d in DamagedComponents) {
				damaged += d.GetHTK();
//				Debug.Log ("damaged " + damaged);
			}
			Integrity = (total-damaged) / total;
		//	Debug.Log (Integrity);
		}
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
			writer.WriteLine ("\nIntegrity: " + Integrity *100f + "%"); 
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
			TakeComponentDamage (4);
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
