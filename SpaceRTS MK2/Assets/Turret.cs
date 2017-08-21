using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretType{
	CAPITAL,
	PD
}

public class Turret : MonoBehaviour {

	public TurretType tType = TurretType.CAPITAL;

	List<Renderer> rens = new List<Renderer> ();
	public GameObject Target;
	public float speed;
	Quaternion original;

	public bool functional = true;
	Color silver = new Color (200, 200, 200);

	// Use this for initialization
	void Start () {
		original = transform.localRotation;
		rens.AddRange(GetComponentsInChildren<Renderer>());
			foreach(Renderer r in rens){
			r.material.color = silver; //orange
			r.material.EnableKeyword ("_EMISSION");
			}
	}
	
	// Update is called once per frame
	void Update () {
		if (rens [0].material.GetColor ("_EmissionColor") != silver) {
			foreach (Renderer r in rens) {
				r.material.SetColor ("_EmissionColor", silver);
			}
		}
		if (tType == TurretType.CAPITAL) {
			speed = 60f;
		} else {
			speed = 165f;
		}
		Quaternion qTo;
		if (Target && functional) {
			//qTo = Quaternion.LookRotation (Target.transform.position - transform.position + Target.GetComponent<Rigidbody>().velocity*.15f, Vector3.up);
			qTo = Quaternion.LookRotation (Target.transform.position - transform.position, Vector3.up);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, qTo, speed * Time.deltaTime);
		} else {
			//go back to original position
		}
	}
}
