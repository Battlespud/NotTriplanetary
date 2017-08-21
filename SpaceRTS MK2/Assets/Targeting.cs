using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Targeting : MonoBehaviour {

	public List<Ship> targets = new List<Ship> ();

	public List<IPDTarget> pdTargets = new List<IPDTarget> ();

	public Ship target; //current target
	public IPDTarget pdtarget;
	public GameObject ipdTarget; //ui
	public int pdTargetsCount;

	public Ship self;
	public List<Turret> turrets = new List<Turret>();

	void Start(){
		self = GetComponentInParent<Ship> ();
		turrets.AddRange(GetComponentsInChildren<Turret>());
		Ship.OnDeath.AddListener (RemoveTargetS);
		PDTargetAbstract.pdDeath.AddListener (RemoveTargetT);
	}


	public void OnTriggerEnter(Collider col){
		IPDTarget t = col.GetComponent<IPDTarget> ();
		if (t != null) {
			if (t.GetFaction() != self.faction) {
				pdTargets.Add (t);
			//	ResetTarget ();
			}
		}
		Ship s = col.GetComponent<Ship> ();
		if (s) {
			if (s.faction != self.faction) {
				targets.Add (s);
			//	ResetTarget ();
			}
		}

	}

	public void RemoveTargetS(Ship s){
		if(targets.Contains(s))
			targets.Remove (s);
		if (target == s){
		//	ResetTarget ();
			target = null;
		}
	}

	public void RemoveTargetT(IPDTarget s){
		if(pdTargets.Contains(s))
			pdTargets.Remove (s);
		if (pdtarget == s){
			pdtarget = null;
	//		ResetTarget ();
		}
	}

	public void OnTriggerExit(Collider col){
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

		IPDTarget t = col.GetComponent<IPDTarget> ();
		if (t!= null) {
			if (pdTargets.Contains (t)) {
				pdTargets.Remove (t);
			}
			if (pdtarget == t) {
				pdtarget = null;
			}
			if (pdTargets.Count != 0)
				pdtarget = pdTargets [0];
		}
	}


	public void ResetTarget(){
		if ( !target && targets.Count != 0)
			target = targets [0];
		foreach (IPDTarget t in pdTargets) {
			if (t == null || t.GetGameObject() == null) {
				RemoveTargetT (t);
			}
		}
		if (pdTargets.Count != 0) {
			pdTargets.OrderBy(
				targ => Vector3.Distance(this.transform.position,targ.GetGameObject().transform.position)).ToList();		
			pdtarget = pdTargets [0];
			}
	}

	// Update is called once per frame
	public  void Update () {
		ResetTarget ();
		foreach (Turret t in turrets) {
			if(target && t.tType == TurretType.CAPITAL)
			t.Target = target.gameObject;
			if(pdtarget != null && t.tType == TurretType.PD)
				t.Target = pdtarget.GetGameObject();
		}
		if (pdtarget != null) {
			ipdTarget = pdtarget.GetGameObject();
		}
		pdTargetsCount = pdTargets.Count;

	}

}
