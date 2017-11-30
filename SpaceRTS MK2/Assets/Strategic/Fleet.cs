using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Text;
using System.Linq;

public enum MoveMode{
	NORMAL,
	STATIONKEEPING,
	INTERCEPT,
	MISSILE,
	BALLISTIC,
	DOCK,
	DOCKED
}

public class Fleet : MonoBehaviour {

	const float TurnsToRaiseShields = 4;
	static bool pause = false;

	public MoveMode Mode;

	static FleetEvent FleetDeathEvent = new FleetEvent();

	public NavMeshAgent Agent;

	//intercepts
	public NavMeshAgent TargetAgent;
	public Fleet Target;
	public StrategicShipyard TargetShipyard;
	public StrategicShipyard DockedShipyard;

	LineRenderer lr;
	LineRenderer standlr;
	LineRenderer downlr;

	public bool AI = false;


	public FAC Faction;
	public Empire empire;
	public string FleetName = "Fleet";
	public string FleetShipNames;

	public List<StrategicShip>Ships = new List<StrategicShip>();
	public List<StrategicMissile>Missiles = new List<StrategicMissile>();
	public float MaxSpeed;
	public float Speed;

	public List<Vector3> Waypoints = new List<Vector3>();

	public List<GameObject> NearbyGameObjects = new List<GameObject>();

	//Combat
	public List<Fleet>EnemyClose = new List<Fleet>();
	public List<Fleet>FriendlyClose = new List<Fleet>();

	//shields are raised 
	public bool ShieldsUp = false;
	public float ShieldStrength = 0f;

	public void AddShip(StrategicShip s){
		if(!Ships.Contains(s))
			Ships.Add (s);
		s.ParentFleet = this;
		RegenerateFleetShipNames ();
	}
	public void RemoveShip(StrategicShip s){
		if(Ships.Contains(s))
			Ships.Remove (s);
		s.ParentFleet = null;
		RegenerateFleetShipNames ();
	}

	void Pause(bool b){
		pause = b;
		if (pause) {
			AllowMovement (false);
		} else {
			if (StrategicClock.GetPhase () == Phase.GO) {
				AllowMovement (true);
			}
		}
	}

	void Awake(){
		FleetDeathEvent.AddListener (CleanReferences);

	}

	// Use this for initialization
	void Start () {
		
		Agent = GetComponent<NavMeshAgent> ();
		StrategicClock.PhaseChange.AddListener (PhaseManager);
		StrategicClock.PauseEvent.AddListener (Pause);
		SetupStand (Color.cyan);
		SortShipsType ();
		AllowMovement (false);
	}

	void PhaseManager(Phase p){
		switch (p) {
		case(Phase.ORDERS):
			{
				HandleShields ();
				AllowMovement (false);
				break;
			}
		case(Phase.GO):
			{
				CalculateFleetSpeed ();
				StartCoroutine (FuelConsumption());
				AllowMovement (true);
				break;
			}
		case (Phase.REVIEW):
			{
				AllowMovement (false);
				break;
			}
		case (Phase.INTERRUPT):
			{
				AllowMovement (false);
				break;
			}

		}	
	}

	void HandleShields(){
		if (!ShieldsUp) {
			ShieldStrength = 0f;
		} else if(ShieldStrength < 1f) {
			ShieldStrength += 1f / TurnsToRaiseShields;
		}
		//literally impossible but we'll cover for it
		if (ShieldStrength > 1f)
			ShieldStrength = 1f; 
		StrategicUIManager.UpdateUI();
	}

	void CalculateFleetSpeed(){
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync (this, FleetSpeedIterator ());
		if (Speed > MaxSpeed)
			Speed = MaxSpeed;
		if (Mode == MoveMode.DOCKED)
			Speed = 0f;
	}


	IEnumerator FuelConsumption(){
		if (Mode != MoveMode.DOCKED) {
			Ships.ForEach (x => {
				x.UseShieldFuel (ShieldStrength);
			});
			float time = 1f;
			while (StrategicClock.GetPhase() == Phase.GO) {
				while (StrategicClock.isPaused) {
					yield return null;
				}
				time += Time.deltaTime;
				if (time >= 1) {
					time = 0f;
					Ships.ForEach (x => {
						x.UseMovementFuel (Agent.speed*1000 );
					});
					CalculateFleetSpeed ();
				}
				yield return null;

			}
		}
		yield return null;
	}


	//AT SPEED 10, acceleration 8, WILL MOVE 25 UNITS OVER 5 SECONDS
	//threaded, so disregard performance cost for now
	IEnumerator FleetSpeedIterator(){
		float max = 100000f;
		if (Ships.Count > 0) {
			foreach (StrategicShip s in Ships) {
				//Ninja.JumpToUnity
				s.ChangeStats();
			//	Ninja.JumpBack;
					//Fleet max is top speed of slowest ship
				float mS = s.MaxSpeed;
				if (mS < max)
					max = mS;
			}
		}
		if (Mode==MoveMode.MISSILE) {
			float accel = 500f;
			foreach (StrategicMissile m in Missiles) {
				if (m.MaxAcceleration < max)
					max = m.MaxAcceleration;
			}
			max += Agent.speed;
		}
		if (Mode == MoveMode.BALLISTIC) {
			max = Agent.speed;
		}
		else  if(Ships.Count <= 0 && Missiles.Count <= 0){
			max = 15;
		}

		yield return Ninja.JumpToUnity;

		MaxSpeed = max;
	}		


	void AllowMovement(bool f){
		Agent.Resume ();
		CalculateFleetSpeed ();
		Agent.speed = MaxSpeed; //change to speed
		Agent.angularSpeed = 90f;
		if (!f) {
		//	Agent.speed = 0f;
		//	Agent.angularSpeed = 0f;
			Agent.Stop();
		}
		
	}




	bool AcceptsOrders(){
		return (!pause && StrategicClock.strategicClock.currPhase == Phase.ORDERS);
	}

	public void SetupStand(Color c){
		GameObject g = new GameObject ();
		lr = g.AddComponent<LineRenderer> ();
		lr.material = new Material(Shader.Find("Particles/Additive"));
		lr.SetColors (Color.blue, Color.blue);
		lr.SetWidth (.35f, .35f);
		GameObject stand = new GameObject();
		standlr = stand.AddComponent<LineRenderer> ();
		stand.transform.position = new Vector3 (this.transform.position.x, StrategicExtensions.yLayer, transform.position.z);
		stand.transform.parent = transform;
		float size = 1f*gameObject.transform.localScale.x;
		RenderCircle (standlr, size);
		GameObject st = new GameObject();
		downlr = st.AddComponent<LineRenderer> ();
		downlr.SetWidth (.5f, .5f);
		st.transform.position = new Vector3 (this.transform.position.x, StrategicExtensions.yLayer, transform.position.z);
		st.transform.parent = transform;
		downlr.useWorldSpace = false;
		standlr.SetColors (c, c);
		downlr.material = new Material(Shader.Find("Particles/Additive"));
		downlr.SetPositions(new Vector3[]{new Vector3(0f,0f,0f), new Vector3 (transform.position.x, StrategicExtensions.yLayer*-6.5f, transform.position.z)});
		downlr.SetColors (c, c);
	}

	void CleanReferences(Fleet f){
		if (EnemyClose.Contains (f))
			EnemyClose.Remove (f);
		if (FriendlyClose.Contains (f))
			FriendlyClose.Remove (f);
	}


	// Update is called once per frame
	void Update () {
		SetPaths ();
		downlr.SetPositions(new Vector3[]{new Vector3(0f,0f,0f), new Vector3 (0f, StrategicExtensions.yLayer*-6.5f, 0f)});
		if (Agent.speed > 0f && Mode == MoveMode.NORMAL) {
			WaypointProgress();
		}
		else if (Mode == MoveMode.INTERCEPT && Target != null) {
			Waypoints.Clear ();
			Vector3 last = Agent.destination;
			try{
				Agent.destination = CalculateIntercept();
			}
			catch{
				Agent.destination = last;
			}
			Waypoints.Add (Agent.destination);
		}
		else if (Mode == MoveMode.DOCK && TargetShipyard != null) {
			Waypoints.Clear ();
			Agent.destination = TargetShipyard.transform.position;
			if (NearbyGameObjects.Contains (TargetShipyard.gameObject)) {
				TargetShipyard.RequestDock (this);
			}
			Waypoints.Add (Agent.destination);
		}
	}

	public void PerformDock(){
		Debug.LogError ("Fleet has docked.");
		Mode = MoveMode.DOCKED;
		DockedShipyard = TargetShipyard;
		TargetShipyard = null;
		ShieldsUp = false;
		ShieldStrength = 0f;
		Waypoints.Clear ();
	}

	public void PerformUndock(){
		Debug.LogError ("Fleet has undocked.");
		DockedShipyard = null;
		Mode = MoveMode.NORMAL;
		ShieldsUp = false;
		ShieldStrength = 0f;
	}

	public float TimeToIntercept;

	Vector3 CalculateIntercept(){
		TimeToIntercept = Vector3.Distance (transform.position, Target.transform.position) / (Agent.velocity - TargetAgent.velocity).magnitude;
		if (TargetAgent.velocity == new Vector3 ())
			return Target.transform.position;
		Vector3 calculated = Target.transform.position + TargetAgent.desiredVelocity * (Vector3.Distance (transform.position, Target.transform.position) / MaxSpeed);
			if(calculated == transform.position)
			return Target.transform.position;
			return (calculated);
	}


	void WaypointProgress(){
			if (Agent.remainingDistance <= float.Epsilon && Waypoints.Count != 0) {
				Waypoints.Remove (Waypoints [0]);
	//			Agent.Stop (); // TODO
				if (Waypoints.Count != 0) {
					IssueMovementCommand (Waypoints [0]);
				}
			}
	}

	public void OrderIntercept(Fleet t)
	{
		if (Mode == MoveMode.DOCKED) {
			DockedShipyard.RequestUndock (this);
		}
		Target = t;
		TargetAgent = t.Agent;
		Mode = MoveMode.INTERCEPT;
		Waypoints.Clear ();
		Agent.Stop ();
		SetPaths ();
	}

	public void OrderDock(StrategicShipyard s){
		if (Mode == MoveMode.DOCKED) {
			DockedShipyard.RequestUndock (this);
		}
		TargetShipyard = s;
		Mode = MoveMode.DOCK;
		Waypoints.Clear ();
		Waypoints.Add (s.transform.position);
		Agent.Stop ();
		SetPaths ();
	}

	public void SetPaths(){
		List<Vector3> LinePoints = new List<Vector3> ();
		LinePoints.Add (transform.position);
		foreach (Vector3 v in Waypoints) {
			LinePoints.Add(new Vector3(v.x,transform.position.y, v.z));
		}
		lr.positionCount = LinePoints.Count;
		lr.SetPositions (LinePoints.ToArray());
	}

	public void SelectColor(){
		standlr.SetColors (Color.green, Color.green);
		downlr.SetColors (Color.green, Color.green);
	}

	public void DeselectColor(){
		standlr.SetColors (Color.cyan, Color.cyan);
		downlr.SetColors (Color.cyan, Color.cyan);

	}

	public void RenderCircle (LineRenderer l, float r) {
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

	//Movement
	public void AddWaypoint(Vector3 targ,bool shift){
		if (!AcceptsOrders())
			return;
		if (Mode == MoveMode.DOCKED) {
			DockedShipyard.RequestUndock (this);
		}

		if (shift) {
			Waypoints.Add (targ);
			IssueMovementCommand(Waypoints[0]);
		} else {
			Waypoints.Clear();
			Waypoints.Add (targ);
			IssueMovementCommand(Waypoints[0]);
		}
	}

	public void RegenerateFleetShipNames(){
		
		FleetShipNames = "";
		StringBuilder sb = new StringBuilder();
		if (Ships.Count > 0) {
			foreach (StrategicShip s in Ships) {
				sb.AppendLine (s.DesignClass.HullDesignation.Prefix + " " + s.ShipName);
				if (s.isDamaged) {
					sb.Append (" [D]");
				}
			}
		} else {
			for (int i = 1; i < 40; i++) {
				sb.AppendLine ( HullDes.DesDictionary[ HullDes.HullTypes[Random.Range(0,2)]].Prefix + " " + NameManager.GrabShipName ());
			}
		}
		FleetShipNames = sb.ToString ();
		StrategicUIManager.UpdateUI();

	}

	public void SortShipsMass(){
		RegenerateFleetShipNames ();
		Ships = Ships.OrderByDescending (s => s.Mass).ThenBy (s => s.DesignClass.DesignName).ToList();
		StrategicUIManager.UpdateUI();

	}

	public void SortShipsType(){
		RegenerateFleetShipNames ();
		Ships = Ships.OrderBy (s => s.DesignClass.HullDesignation.Prefix).ThenBy (s => s.DesignClass.DesignName).ToList();
		StrategicUIManager.UpdateUI();

	}

	public static Vector3 ToVector3(Vector2 vec){
		return new Vector3 (vec.x, .6f, vec.y);
	}

	public void IssueMovementCommand(Vector3 vec){
		try{
		Agent.SetDestination (new Vector3(vec.x,transform.position.y, vec.z));
		}
		catch{
			Debug.Log ("No agent assigned");
		}
		StrategicUIManager.UpdateUI();

	}

	void OnTriggerEnter(Collider col){
		if (col.GetComponent<Fleet> ()) {
			Fleet f = col.GetComponent<Fleet> ();
			if (FactionMatrix.IsHostile (Faction, f.Faction)) {
				EnemyClose.Add (f);
				if(!pause)
					PromptAction ();
			} else if(Faction == f.Faction){
				FriendlyClose.Add (f);
			}

		}
		NearbyGameObjects.Add (col.gameObject);
		StrategicUIManager.UpdateUI();

	}



	void OnTriggerExit(Collider col){
		NearbyGameObjects.Remove (col.gameObject);
		if (col.GetComponent<Fleet> ()) {
			Fleet f = col.GetComponent<Fleet> ();
			if (EnemyClose.Contains (f))
				EnemyClose.Remove (f);
			if (FriendlyClose.Contains (f))
				FriendlyClose.Remove (f);
		}
	}

	void PromptAction(){
		StrategicUIManager.UpdateUI();
		StrategicClock.RequestPause ();

	}

}
