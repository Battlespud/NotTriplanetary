using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDBullet : MonoBehaviour {

	float timer = 1f;

	int damage = 10;
	public Rigidbody rb;
	Collider col;
	Renderer r;
	// Use this for initialization
	void Awake () {
		rb = GetComponent<Rigidbody> ();
		col = GetComponent<Collider> ();
		r = GetComponent<Renderer> ();
		Color silver = new Color (200, 200, 200);
		r.material.color = silver; //orange
		r.material.SetColor ("_EmissionColor", silver);
		r.material.EnableKeyword ("_EMISSION");
		gameObject.layer = 8;
	}
	
	// Update is called once per frame
	void Update () {
		timer -= Time.deltaTime;
		if (timer <= 0f)
			Destroy (gameObject);
	}




	void OnCollisionEnter(Collision col){
		if (col.collider.GetComponent<IPDTarget> () != null) {
			col.collider.GetComponent<IPDTarget> ().HitByPD (damage);
		}
	}
}
