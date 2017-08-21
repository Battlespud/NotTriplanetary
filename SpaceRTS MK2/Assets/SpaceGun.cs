using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public  class SpaceGun : MonoBehaviour {

	//TODO make guns go in barrels instead of on the turret object.

	public Light light;

	public Ship self;
	public List<Ship> targets = new List<Ship>();
	public Ship target;

	public List<IPDTarget> pdTargets = new List<IPDTarget>();
	public IPDTarget pdTarget;
	public GameObject pdTargObj;


	public bool shooting = true;
	public bool CanFire = true;

	public float powerCost = 1f;

	public float ReloadTime = 1.5f;
	public float Damage = 1f;
	public float ForceMagnitude = 0f;
	public Color lineC;

	public TurretType tType = TurretType.CAPITAL;

	public Turret turret;


	// Use this for initialization
	public void Start () {
		lineC = Color.red;
		CustomInitialize ();
		Initialize ();
		shooting = true;
		CanFire = true;

	}

	public void Initialize(){
		light = GetComponentInChildren<Light> ();
		if(light)
		light.enabled = false;
		self = GetComponentInParent<Ship> ();
		targets = new List<Ship> ();
		Ship.OnDeath.AddListener (RemoveTargetS);
		PDTargetAbstract.pdDeath.AddListener (RemoveTargetT);
		turret = GetComponentInParent<Turret> ();
		if (!turret) {
			turret = GetComponent<Turret> ();
		}
		turret.tType = tType;
	}

	public virtual void CustomInitialize(){

	}

	public void RemoveTargetS(Ship s){
		if(targets.Contains(s))
			targets.Remove (s);
		if (target == s)
			target = null;
			ResetTarget ();
	}

	public void RemoveTargetT(IPDTarget s){
		if(pdTargets.Contains(s))
			pdTargets.Remove (s);
		if (pdTarget == s)
			pdTarget = null;
			ResetTarget ();
	}

	public virtual void OnTriggerEnter(Collider col){
		if (tType == TurretType.CAPITAL) {
			Ship s = col.GetComponent<Ship> ();
			if (s) {
				if (s.faction != self.faction) {
					targets.Add (s);
				}
			}
		} else if(tType == TurretType.PD) {
			IPDTarget s = col.GetComponent<IPDTarget> ();
			if (s != null) {
				if (s.GetFaction() != self.faction) {
					pdTargets.Add (s);
				}
				SortList ();
			}


		}
	}

	public virtual void OnTriggerExit(Collider col){
		Ship s = col.GetComponent<Ship> ();
		IPDTarget t = col.GetComponent<IPDTarget> ();
		if (s) {
			if (targets.Contains (s)) {
				targets.Remove (s);
			}
				if (target == s) {
					target = null;
			}
			if (targets.Count != 0)
				target = targets [0];
		}
		else if (t != null) {
			if (pdTargets.Contains (t)) {
				pdTargets.Remove (t);
			}
			if (pdTarget == t) {
				pdTarget = null;
			}
			if (pdTargets.Count != 0)
				pdTarget = pdTargets [0];
		}
	}

	// Update is called once per frame
	public virtual void Update () {
		ResetTarget ();
		if (pdTarget != null)
			pdTargObj = pdTarget.GetGameObject ();
		if (target || pdTarget != null) {
			Fire ();
		}
	}

	public virtual void Fire(){
		if (tType == TurretType.CAPITAL) {
			if (!CanFire || !target || !self.shipClass.Power.UsePower (powerCost))
				return;
			Debug.DrawLine (self.transform.position, target.transform.position, lineC, .05f);
			if (ForceMagnitude > 0f) {
				target.shipClass.PhysicsDamage (Damage, self.transform.position, ForceMagnitude, self.transform);
			} else {
				target.shipClass.Damage (Damage, self.transform.position, self.transform);
			}
			StartCoroutine ("Reload");
			CanFire = false;
			if(light)
			StartCoroutine ("MuzzleFlash");
		}
		if (tType == TurretType.PD) {
			if (!CanFire || pdTarget == null || !self.shipClass.Power.UsePower (powerCost))
				return;
			Debug.DrawLine (self.transform.position, pdTarget.GetGameObject().transform.position, lineC, .05f);
			pdTarget.HitByPD((int)Damage);
			StartCoroutine ("Reload");
			CanFire = false;
		}
	}

	IEnumerator MuzzleFlash(){
		light.enabled = true;
		float time = .1f;
		while (time > 0f) {
			time -= Time.deltaTime;
			yield return null;
		}
		light.enabled = false;
	}

	public void SortList(){
		targets.OrderBy(
			targ => Vector3.Distance(this.transform.position,targ.transform.position)).ToList();

		pdTargets.OrderBy(
			targ => Vector3.Distance(this.transform.position,targ.GetGameObject().transform.position)).ToList();
		ResetTarget ();
	}

	public void ResetTarget(){
		if ( !target && targets.Count != 0)
			target = targets [0];

		if ( pdTargets.Count != 0)
			pdTarget = pdTargets [0];
	}

	public void AssignTarget(Ship s){
		if (targets.Contains (s)) {
			target = s;
		}
	}

	public void AssignTarget(IPDTarget s){
		if (pdTargets.Contains (s)) {
			pdTarget = s;
		}
	}


	public IEnumerator Reload(){
		float timer = ReloadTime;
		while (timer > 0) {
			timer -= Time.deltaTime;
			yield return null;
		}
		CanFire = true;
	}
}
