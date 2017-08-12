using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Ship : MonoBehaviour {

	//this only handles things specific to this entity, like movement.

	public static GameObject Debris; //spawns on death.
	public static GameObject Torpedo;

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

	//Pathfinding
	public List<Vector2>Waypoints = new List<Vector2>();

	LineRenderer lr;
	public Renderer render;


	//main

	public string ShipName;

	public float mass; //kg
	public float enginePower; //newtons
	public Vector3 VelocityUI;
	public float DotProdUI;
	public GeneralDirection dir;

	public NavMeshAgent Agent;
	// Use this for initialization
	void Start () {
		if (faction == 0)
			GetComponentInChildren<Renderer> ().material.color = Color.green;
		else {
			GetComponentInChildren<Renderer> ().material.color = Color.red;
		}
		Debris = Resources.Load<GameObject> ("Debris") as GameObject;
		Torpedo = Resources.Load<GameObject> ("Torpedo") as GameObject;
		Agent = GetComponent<NavMeshAgent> ();
		foreach (SpaceGun sg in GetComponentsInChildren<SpaceGun>()) {
			Guns.Add (sg);
		}
		shipClass = gameObject.AddComponent<ShipClass> ();
		lr = this.gameObject.AddComponent<LineRenderer> ();
		lr.enabled = false;
		lr.SetWidth (.035f, .035f);
	//	render = GetComponentInChildren<Renderer> ();

	}
	
	// Update is called once per frame
	void Update () {
		if(lr.enabled)SetPaths() ;
		mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
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

	public void SpawnDebris(Vector3 source){
		if (dead)
			return;
		dead = true;
		Vector3 dir = transform.position - source;
		dir = dir.normalized;
		GameObject deb = Instantiate (Debris);
		deb.transform.position = transform.position+dir*.1f;
		Rigidbody rb = deb.GetComponent<Rigidbody> ();
		rb.AddForce (dir*125f);
		Explode ();
	}

	public void Explode(){
		Collider[] col = Physics.OverlapSphere (transform.position, 5f);
		StartCoroutine ("ExplosionRadius");
		foreach (Collider hit in col) {
			Rigidbody rb = hit.GetComponent<Rigidbody> ();
			float Force = 160f;
			if (rb) {
				ShipClass c = rb.GetComponent<ShipClass> ();
				if (c) {
					if (!c.screens.ScreensWillHold (20f * (1f / Vector3.Distance (c.transform.position, transform.position)), transform.position))
						rb.AddExplosionForce (Force, transform.position, 5f, 0f);
					c.Damage (20f * (2f / Vector3.Distance (c.transform.position, transform.position)), transform.position);
					Debug.Log (c.name);
				} else {
					rb.AddExplosionForce (Force, transform.position, 5f, 0f);
				}
			}
		}
		Destroy (render);
	}

	IEnumerator ExplosionRadius(){
		GameObject g = new GameObject();
		g.name = "Explosion Radius";
		g.transform.position = this.transform.position;
		g.transform.parent = null;
		LineRenderer l = g.AddComponent<LineRenderer>();
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

	void RenderCircle (LineRenderer l, float r) {
		int numSegments = 255;
		float radius = r;
		l.material = new Material(Shader.Find("Particles/Additive"));
		l.SetColors(Color.yellow, Color.yellow);
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

	public void FireTorpedo(){
		if (shipClass.Torpedos > 0) {
			shipClass.Torpedos -= 1;
			GameObject t = Instantiate (Torpedo);
			t.transform.position = transform.position + transform.forward * .25f;
			t.GetComponent<Rigidbody> ().AddForce (transform.forward * 150f);
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
		StopAllCoroutines ();
	//	Agent.SetDestination (transform.position);
		Agent.velocity = new Vector3();
		StartCoroutine("Movement", (new Vector3(vec.x,transform.position.y, vec.y)));
	}

	public IEnumerator Movement(Vector3 dest){
		Agent.isStopped = false;
		float dotProd = 0f;
		Agent.SetDestination (dest);
		while (dotProd < .85) {
			Agent.acceleration = .01f*Agent.acceleration;
			dotProd = Vector3.Dot (transform.forward, (dest - transform.position).normalized);
			DotProdUI = dotProd;
			yield return null;
		}
	}
}
