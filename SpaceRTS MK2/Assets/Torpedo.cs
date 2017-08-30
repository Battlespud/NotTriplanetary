using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour, IPDTarget {

	public float eRadius = 2.5f;

	public List<ICAPTarget> InBlastZone = new List<ICAPTarget>();
	Collider col;

	int hp = 1;

	public FAC faction;

	 float LaunchForce = 40f; //50
	public float Force = 10f;

	float fuseTimer = 2f;
	public bool armed = false;

	float fuelTime = 15f;

	Renderer r;

	Color orange = new Color(255,140,0);

	// Use this for initialization
	void Start () {
		transform.position = new Vector3 (transform.position.x, .59f, transform.position.z);
		gameObject.layer = 9;
		GetComponent<Rigidbody> ().AddForce (transform.forward * LaunchForce);
		col = GetComponent<Collider> ();
		col.enabled = false;
		StartCoroutine ("ArmingTimer");
		r = GetComponent<Renderer> ();
		r.material.SetColor ("_EmissionColor", orange  );
		r.material.EnableKeyword ("_EMISSION");
		r.material.color = orange; //orange

	}

	//PD I
	public void HitByPD(int dam){
		hp -= dam;
	}

	public GameObject GetGameObject(){
		if(gameObject != null)
			return gameObject;
		return null;
	}

	public FAC GetFaction(){
		return faction;
	}

	public bool isHostile(FAC caller){
		return FactionMatrix.IsHostile (caller, faction);
	}


	void OnTriggerEnter(Collider col){
		if(col.GetComponent<ICAPTarget>() != null)
			InBlastZone.Add(col.GetComponent<ICAPTarget>());
	}

	void OnTriggerExit(Collider col){
		if(col.GetComponent<ICAPTarget>() != null)
			InBlastZone.Remove(col.GetComponent<ICAPTarget>());
	}

	void OnCollisionEnter(Collision col){
		if (!armed)
			return;
		if (col.collider.GetComponent<ICAPTarget> () != null || col.collider.GetComponentInChildren<ICAPTarget>() != null || col.collider.GetComponentInParent<ICAPTarget>() != null)
			Detonate ();
	}

	public void Detonate(){
		foreach (ICAPTarget s in InBlastZone) {
			s.DealDamage (12f-(2f*Vector3.Distance(s.GetGameObject().transform.position,transform.position)), transform.position,transform);
		}
		StartCoroutine ("ExplosionRadius");
		StartCoroutine ("ExplosionExpansion");


		Collider[] col = Physics.OverlapSphere (transform.position, eRadius);
		foreach (Collider hit in col)
		{
			if (!hit.gameObject.GetComponent<Torpedo> ()) { //wont knock away torpedoes
				Rigidbody rb = hit.GetComponent<Rigidbody> ();

				if (rb)
					rb.AddExplosionForce (Force, transform.position, eRadius, 0f);
			}
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
		hp = -1;
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
		fuelTime -= Time.deltaTime;
		if (hp <= 0 || fuelTime <= 0f) {
			PDTargetAbstract.pdDeath.Invoke (this);
			transform.position = new Vector3 (100f, 1000f, 100f); //to trigger OnTriggerExit and let guns reset.
			StartCoroutine("Destroy");
		}
	}

	IEnumerator Destroy(){
		float f = 1f;
		while (f > 0f) {
			f -= Time.deltaTime;
			yield return null;
		}
		Destroy (gameObject);
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
