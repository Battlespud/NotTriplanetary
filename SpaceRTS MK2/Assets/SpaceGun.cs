using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceGun : MonoBehaviour {

	public Ship self;
	public Ship target;
	public bool shooting = false;
	public bool CanFire = true;

	// Use this for initialization
	void Start () {
		self = GetComponentInParent<Ship> ();
	}

	void OnTriggerEnter(Collider col){
		Ship s = col.GetComponent<Ship> ();
		if (s) {
			if (s.faction != self.faction) {
				target = s;
				shooting = true;
			}
		}
	}

	void OnTriggerExit(Collider col){
		Ship s = col.GetComponent<Ship> ();
		if (s) {
			if (target = s) {
				target = null;
			}
		}

	}

	// Update is called once per frame
	void Update () {
		if (target && shooting) {
			Fire ();
		}
	}

	void Fire(){
		if (!CanFire || !target)
			return;
		Debug.DrawLine (self.transform.position, target.transform.position, Color.red, .05f);
		Debug.Log (gameObject.name + " firing");
		StartCoroutine ("Reload");
		CanFire = false;
	}

	IEnumerator Reload(){
		float timer = .05f;
		while (timer > 0) {
			timer -= Time.deltaTime;
			yield return null;
		}
		CanFire = true;
	}
}
