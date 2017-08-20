using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Ship : MonoBehaviour {

	//this only handles things specific to this entity, like movement.

	public static ShipEvent OnDeath = new ShipEvent();

	public static GameObject Debris; //spawns on death.
	public static GameObject Torpedo;
	//debugging direction
	public GameObject dirTarget;
	public GeneralDirection dirTest;
	public GeneralDirection dirInverse;


	bool moveAssigned = false;

	public ShipClass shipClass;

	Vector3 mousePos;

	bool dead = false;

	//combat
	public int faction;
	public List<SpaceGun> Guns = new List<SpaceGun> ();

	//movement
	public bool immobile;
	bool engineDisabled;
	public bool underTractor;
	public bool usingTractor;

	public Rigidbody rb;

	//Pathfinding
	public List<Vector2>Waypoints = new List<Vector2>();

	LineRenderer lr;
	public Renderer render;

	public List<Renderer> rens; 
	public GeneralDirection dir;

	//main

	public string ShipName;

	public float mass; //kg
	public float enginePower; //newtons
	public Vector3 VelocityUI;
	public float DotProdUI;

	public NavMeshAgent Agent;
	// Use this for initialization
	void Start () {
        ShipName = NameManager.AssignName();
		if (!render)
			render = GetComponentInChildren<Renderer> ();
		rens.AddRange( GetComponentsInChildren<Renderer> ());
		rb = GetComponent<Rigidbody> ();
		if (faction == 0) {
			foreach (Renderer r in rens) {
				r.material.color = Color.blue;
				r.material.SetColor ("_EmissionColor", Color.blue);
				r.material.EnableKeyword ("_EMISSION");
			}

		}
		else {
			foreach (Renderer r in rens) {
				r.material.color = Color.red;
				r.material.SetColor ("_EmissionColor", Color.red);
				r.material.EnableKeyword ("_EMISSION");
			}
		}

		Debris = Resources.Load<GameObject> ("Debris") as GameObject;
		Torpedo = Resources.Load<GameObject> ("Torpedo") as GameObject;
		Agent = GetComponent<NavMeshAgent> ();
		foreach (SpaceGun sg in GetComponentsInChildren<SpaceGun>()) {
			Guns.Add (sg);
		}
		shipClass = gameObject.GetComponent<ShipClass> ();
		lr = this.gameObject.AddComponent<LineRenderer> ();
		lr.enabled = false;
		lr.SetWidth (.035f, .035f);
        //	render = GetComponentInChildren<Renderer> ();
        gameObject.name = ShipName;
	}
	
	// Update is called once per frame
	void Update () {
		if (dirTarget) {
			dirTest = Direction.GetDirection (dirTarget.transform.position, dirTarget.transform, transform.position, transform);
		}
		if(lr.enabled)SetPaths() ;
		mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		if (enginePower == 0)
			enginePower = 1;
		Agent.acceleration = mass / enginePower;
		if (usingTractor)Agent.acceleration *= .35f;
		TractorLoop ();

		if (!moveAssigned) {
			if (Waypoints.Count != 0) {
				IssueMovementCommand(Waypoints[0]);
			}
		}
		if( Agent.remainingDistance <= float.Epsilon && Waypoints.Count != 0) {
			Waypoints.Remove (Waypoints [0]);
			moveAssigned = false;
			SetPaths ();
			if (Waypoints.Count != 0) {
				IssueMovementCommand(Waypoints[0]);
			}
		}
	}

	public List<LineRenderer> path = new List<LineRenderer>();

	public void TogglePath(){
		lr.enabled = !lr.enabled;
	}

	void TractorLoop(){

		//dir = Direction.GetDirection (transform.position, transform.position - new Vector3(mousePos.x,transform.position.y,mousePos.z));
		if (underTractor || engineDisabled)
			immobile = true;
		else
			immobile = false;
		if (immobile)
			Agent.acceleration=0f;
	}



	public void SetUnderTractor(){
		underTractor = true;
	}

	public void Die(){
		Debug.Log (name + " is lost!");
		DisableWeapons ();
		OnDeath.Invoke (this);
		NameManager.RecycleName (ShipName);
	}

	public void SpawnDebris(Vector3 source){
		if (dead)
			return;
		dead = true;
		Vector3 dir = transform.position - source;
		dir = dir.normalized;
		GameObject deb = Instantiate (Debris);
		deb.transform.position = transform.position+dir*.1f;
		Rigidbody r = deb.GetComponent<Rigidbody> ();
		r.AddForce (dir*125f);
		Explode ();
	}

	public void Explode(){
		Collider[] col = Physics.OverlapSphere (transform.position, 5f);
		//StartCoroutine ("ExplosionRadius");
		StartCoroutine("ExplosionExpansion",5f);
		foreach (Collider hit in col) {
			Rigidbody r = hit.GetComponent<Rigidbody> ();
			float Force = 5f;
			if (r) {
				ShipClass c = rb.GetComponent<ShipClass> ();
				if (c) {
					if (!c.screens.ScreensWillHold (20f * (1f / Vector3.Distance (c.transform.position, transform.position)), transform.position,c.transform))
						r.AddExplosionForce (Force, transform.position, 5f, 0f);
					c.Damage (20f * (2f / Vector3.Distance (c.transform.position, transform.position)), transform.position, c.transform);
//					Debug.Log (c.name);
				} else {
					r.AddExplosionForce (Force, transform.position, 5f, 0f);

				}
			}
		}
		Destroy (render);
	}

	#region Explosion Coroutines
	IEnumerator ExplosionRadius(){
		GameObject g = new GameObject();
		g.name = "Explosion Radius";
		g.transform.position = this.transform.position;
		g.transform.parent = null;
		LineRenderer l = g.AddComponent<LineRenderer>();
		l.startColor = Color.red;
		RenderCircle (l, 5f);
		float a = 0f;
		Color c = new Color (l.startColor.r, l.startColor.g, l.startColor.b);
		while( c.a > 0f){
			c = new Color (l.startColor.r, l.startColor.g, l.startColor.b, Mathf.Lerp (l.startColor.a, 0f, 5f * Time.deltaTime));
			a += Time.deltaTime;
			l.SetColors (c, c);
		yield return null;
		}
		Destroy (gameObject);
	}

	IEnumerator ExplosionExpansion(float eRadius){
		GameObject g = new GameObject();
		g.name = "Shockwave";
		g.transform.position = this.transform.position;
		g.transform.parent = null;
		LineRenderer l = g.AddComponent<LineRenderer>();
//		l.material = new Material (Shader.Find ("Particles/Additive"));
		l.startColor = Color.green;
		if(faction != 0)
		l.startColor = Color.red;
		Color c = new Color (l.startColor.r, l.startColor.g, l.startColor.b);
		float f = .1f;
		float b = 0f;
		float alpha = 100f;
		while(f < eRadius && b < 3f){
			RenderCircle (l, f);
			f = Mathf.Lerp (f, eRadius, 1f * Time.deltaTime);
			float a = 0f;
			b += Time.deltaTime;
			alpha = Mathf.Lerp (alpha, 0f, 4f* Time.deltaTime);
			c = new Color (l.startColor.r, l.startColor.g, l.startColor.b, alpha );
			a += Time.deltaTime;
			l.SetColors (c, c);
			yield return null;
		}
		Destroy (g);
		Destroy (gameObject);
	}

	void RenderCircle (LineRenderer l, float r) {
		int numSegments = 255;
		float radius = r;
		l.material = new Material(Shader.Find("Particles/Additive"));
	//	l.SetColors(Color.yellow, Color.yellow);
		l.SetWidth(0.025f, 0.025f);
		l.SetVertexCount(numSegments + 1);
		l.useWorldSpace = false;

		float deltaTheta = (float) (2.0 * Mathf.PI) / numSegments;
		float theta = 0f;

		for (int i = 0 ; i < numSegments + 1 ; i++) {
			float x = radius * Mathf.Cos(theta);
			float z = radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, 0, z);
			l.SetPosition(i, pos);
			theta += deltaTheta;
		}
	}
	#endregion

	public IEnumerator TorpedoArm(){
		GameObject g = new GameObject ();
		g.transform.rotation = transform.rotation;
		g.transform.position = transform.position;
		LineRenderer l = g.AddComponent<LineRenderer> ();
		l.positionCount = 2;
		l.useWorldSpace = true;
		l.material = new Material(Shader.Find("Particles/Additive"));
		l.SetColors (Color.grey, Color.red);
		l.SetWidth (.05f, .05f);
		Vector3 targ = new Vector3();
		while (Input.GetKey (KeyCode.B)) {
			targ = new Vector3 (Camera.main.ScreenToWorldPoint (Input.mousePosition).x, transform.position.y, Camera.main.ScreenToWorldPoint (Input.mousePosition).z);
			l.SetPositions (new Vector3[2]{ transform.position,targ	 });
			yield return null;
		}
		FireTorpedo (targ);
		Destroy (g);
	}

	public void FireTorpedo(Vector3 targ){
		if (shipClass.Torpedos > 0) {
			shipClass.Torpedos -= 1;
			GameObject t = Instantiate (Torpedo);
			t.transform.position = transform.position;
			t.transform.LookAt(targ);
		}
	}


	public void AddWaypoint(Vector2 targ,bool shift){
		if (shift) {
			Waypoints.Add (targ);
		} else {
			Waypoints.Clear();
			Waypoints.Add (targ);
			IssueMovementCommand(Waypoints[0]);
		}
	}

	public void SetPaths(){
		List<Vector3> LinePoints = new List<Vector3> ();
		LinePoints.Add (transform.position);
		foreach (Vector2 v in Waypoints) {
			LinePoints.Add(ToVector3(v));
		}
		lr.positionCount = LinePoints.Count;
		lr.SetPositions (LinePoints.ToArray());
	}

	public static Vector3 ToVector3(Vector2 vec){
		return new Vector3 (vec.x, .6f, vec.y);
	}

	public void IssueMovementCommand(Vector2 vec){
		moveAssigned = true;
		StopCoroutine("Movement");
		Agent.velocity = new Vector3();
		StartCoroutine("Movement", (new Vector3(vec.x,transform.position.y, vec.y)));
	}

	public IEnumerator Movement(Vector3 dest){
		Agent.isStopped = false;
		float dotProd = 0f;
		Agent.SetDestination (dest);
		while (dotProd < .85) {
			Agent.acceleration = .14f*Agent.acceleration;
			dotProd = Vector3.Dot (transform.forward, (dest - transform.position).normalized);
			DotProdUI = dotProd;
			yield return null;
		}
	}

	public void DisableWeapons(){
		foreach (SpaceGun s in Guns) {
			s.enabled = false;
		}
	}
}
