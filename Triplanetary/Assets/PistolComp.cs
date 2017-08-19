using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolComp : MonoBehaviour {

	bool CanFire = true;
	float CD = .35f;
	LineRenderer lr;
	Renderer ren;

	// Use this for initialization
	void Start () {
		ren = GetComponent<Renderer> ();
		lr = gameObject.AddComponent<LineRenderer> ();
		lr.material = new Material(Shader.Find("Particles/Additive"));
		lr.SetWidth (.1f, .1f);
		lr.SetColors (Color.green, Color.green);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			Fire ();
		}
		if (CanFire) {
			ren.material.color = Color.green;
		} else {
			ren.material.color = Color.red;
		}
	}


	void Fire(){
		if (!CanFire) {
			return;
		}
		CanFire = false;
		RaycastHit hit;
		Ray ray = new Ray (transform.position, transform.forward * 100f);
		if (Physics.Raycast (ray, out hit)) {
			Debug.DrawLine (transform.position, hit.point,Color.green);
			StartCoroutine ("Laser", hit.point);

		} else {
			Debug.DrawLine (transform.position, transform.forward*100f,Color.green);
			StartCoroutine ("Laser", transform.forward * 100f);
		}
		StartCoroutine ("Reload");
	}

	IEnumerator Reload(){
		float f = CD;
		while (f > 0f) {
			f -= Time.deltaTime;
			yield return null;
		}
		CanFire = true;
	}

	IEnumerator Laser(Vector3 hit){
		float f = .15f;
		lr.enabled = true;
		lr.SetWidth (.1f, .1f);
		float d = .1f;
		while (f > 0f) {
			lr.SetPositions (new Vector3[2]{ transform.position, hit });
			Mathf.Lerp (f, 0f, 1f * Time.deltaTime);
			lr.SetWidth (f, f);
			f -= Time.deltaTime;
			yield return null;
		}
		lr.enabled = false;
	}
}
