using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour {

	public float eRadius = 7.5f;

	public List<Ship> InBlastZone = new List<Ship>();
	Collider col;


	 float LaunchForce = 50f; //50
	public float Force = 450f;

	float fuseTimer = .5f;
	public bool armed = false;

	Renderer r;

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody> ().AddForce (transform.forward * LaunchForce);
		col = GetComponent<Collider> ();
		col.enabled = false;
		StartCoroutine ("ArmingTimer");
		r = GetComponent<Renderer> ();
		r.material.color = new Color(255,140,0); //orange
		r.material.SetColor ("_EmissionColor", new Color(255,140,0));
		r.material.EnableKeyword ("_EMISSION");
	}

	void OnTriggerEnter(Collider col){
		if(col.GetComponent<Ship>())
			InBlastZone.Add(col.GetComponent<Ship>());
	}

	void OnTriggerExit(Collider col){
		if(col.GetComponent<Ship>())
			InBlastZone.Remove(col.GetComponent<Ship>());
	}

	void OnCollisionEnter(Collision col){
		if (!armed)
			return;
		if (col.collider.GetComponent<Ship> ())
			Detonate ();
	}

	public void Detonate(){
		StartCoroutine ("ExplosionRadius");
		StartCoroutine ("ExplosionExpansion");

		foreach (Ship s in InBlastZone) {
			s.shipClass.Damage (45f, transform.position,transform);


		}
		Collider[] col = Physics.OverlapSphere (transform.position, eRadius);
		foreach (Collider hit in col)
		{
			Rigidbody rb = hit.GetComponent<Rigidbody>();

			if (rb)
				rb.AddExplosionForce(Force, transform.position, eRadius, 0f);
		}
		Destroy (GetComponent<Renderer>());

	}

	IEnumerator ExplosionRadius(){
		GameObject g = new GameObject();
		StartCoroutine ("ExplosionExpansion");
		g.name = "Explosion Radius";
		g.transform.position = this.transform.position;
		g.transform.parent = null;
		LineRenderer l = g.AddComponent<LineRenderer>();
		RenderCircle (l, eRadius);
		float a = 0f;
		Color c = new Color (l.startColor.r, l.startColor.g, l.startColor.b);
		while( c.a > .05f){
			c = new Color (l.startColor.r, l.startColor.g, l.startColor.b, Mathf.Lerp (l.startColor.a, 0f, 4f * Time.deltaTime));
			a += Time.deltaTime;
			l.SetColors (c, c);
			yield return null;
		}
		c = new Color (l.startColor.r, l.startColor.g, l.startColor.b, 0f);
		l.SetColors (c, c);

		float b = 0f;
		while( b < 3f){
			b += Time.deltaTime;
			yield return null;
		}
		Destroy (g);
		Destroy (gameObject);
	}

	IEnumerator ExplosionExpansion(){
		GameObject g = new GameObject();
		g.name = "Shockwave";
		g.transform.position = this.transform.position;
		g.transform.parent = null;
		LineRenderer l = g.AddComponent<LineRenderer>();
		l.SetColors (Color.yellow, Color.yellow);
		l.material = new Material (Shader.Find ("Particles/Additive"));
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

	// Update is called once per frame
	void Update () {

	}

	IEnumerator ArmingTimer(){
		float a = 0f;
		while (a < fuseTimer) {
			a += Time.deltaTime;
			yield return null;
		}
		col.enabled = true;
		armed = true;
	}
}
