using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public  class SpaceGun : MonoBehaviour {

	public Ship self;
	public List<Ship> targets;
	public Ship target;
	public bool shooting = true;
	public bool CanFire = true;

	public float powerCost = 1f;

	public float ReloadTime = 1.5f;
	public float Damage = 1f;
	public float ForceMagnitude = 0f;
	public Color lineC;


	// Use this for initialization
	public virtual void Start () {
		lineC = Color.red;
		Initialize ();
	}

	public virtual void Initialize(){
		self = GetComponentInParent<Ship> ();
		targets = new List<Ship> ();
		Ship.OnDeath.AddListener (RemoveTarget);
	}

	public void RemoveTarget(Ship s){
		if(targets.Contains(s))
			targets.Remove (s);
		if (target == s)
			ResetTarget ();
	}

	public virtual void OnTriggerEnter(Collider col){
		Ship s = col.GetComponent<Ship> ();
		if (s) {
			if (s.faction != self.faction) {
				targets.Add (s);
			}
		}
	}

	public virtual void OnTriggerExit(Collider col){
		Ship s = col.GetComponent<Ship> ();
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
	}

	// Update is called once per frame
	public virtual void Update () {
		ResetTarget ();
		if (target) {
			Fire ();
		}
	}

	public virtual void Fire(){
		if (!CanFire || !target || !self.shipClass.Power.UsePower(powerCost))
			return;
		Debug.DrawLine (self.transform.position, target.transform.position, lineC, .05f);
		if (ForceMagnitude > 0f) {
			target.shipClass.PhysicsDamage (Damage, self.transform.position,ForceMagnitude, self.transform);
		} else {
			target.shipClass.Damage (Damage, self.transform.position, self.transform);
		}
		StartCoroutine ("Reload");
		CanFire = false;
	}

	public void SortList(){
		targets.OrderBy(
			targ => Vector3.Distance(this.transform.position,targ.transform.position)
		).ToList();
		ResetTarget ();
	}

	public void ResetTarget(){
		if ( !target && targets.Count != 0)
			target = targets [0];
	}

	public void AssignTarget(Ship s){
		if (targets.Contains (s)) {
			target = s;
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
