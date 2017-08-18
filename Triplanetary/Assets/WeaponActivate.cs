using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponActivate : MonoBehaviour, IActivate {

	string toolTip = "Man Cannon..";
	public GameObject Gunner;
	public Camera GunCamera;
	public GameObject Turret;
	public GameObject Muzzle;
	public GameObject Ship;

	public GameObject GunnerUI;

	PowerEndpoint Power;
	public bool Powered = false;


	//Movement

	public bool Active = false;

	public string GetToolTip(){
		return toolTip;
	}

	public void Activate(Activator a){
		if (!Active) {
			TurnOn (a);
		} else {
			TurnOff (a);

		}
	}

	void TurnOn(Activator a){
		if (!Powered)
			return;
		Gunner = a.gameObject;
		Gunner.transform.parent = Ship.transform;
		a.GetComponent<MovementController> ().busy = true;
		a.GetComponent<MovementController> ().cam.enabled = false;
		EnableGunCamera ();
	}

	void TurnOff(Activator a){
		Active = false;
		Gunner.transform.parent = null;
		a.GetComponent<MovementController> ().busy = false;
		a.GetComponent<MovementController> ().cam.enabled = true;
		GunCamera.enabled = false;
		GunCamera.transform.rotation = Muzzle.transform.rotation;

	}

	void EnableGunCamera(){
		GunCamera.enabled = true;
		Active = true;
		StartCoroutine ("GunControls");
	}


	IEnumerator GunCameraOff(){
		GunCamera.enabled = true;
		Active = false;
		TurnOff (Gunner.GetComponent<Activator>());
		yield return null;
	}


	IEnumerator GunControls(){
		Cursor.visible = false;
		Debug.Log ("Gun controls online.");
		while (Active) {
			if (Input.GetKey (KeyCode.Space)) {

			} else {
				Turret.transform.Rotate ( Vector3.up, 180f * Time.deltaTime * Input.GetAxis ("Mouse X"));
			//	transform.RotateAround (Turret.transform.position, Vector3.right, 60f * Time.deltaTime * Input.GetAxis ("Mouse Y"));


				yield return null;
			}
			Cursor.visible = true;
		}
	}


	Renderer ren;
	// Use this for initialization
	void Start () {
		ren = gameObject.GetComponent<Renderer> ();
		GunCamera.enabled = false;
		Power = GetComponent<PowerEndpoint> ();
	}

	// Update is called once per frame
	void Update () {
		Powered = Power.Recieving;
		if(Active&& !Powered){
			StopAllCoroutines ();
			StartCoroutine ("GunCameraOff");
		}
		if (Powered) {
			ren.material.color = Color.cyan;
			Debug.DrawRay (Muzzle.transform.position, Muzzle.transform.forward * 25f, Color.white);
		} else {
			ren.material.color = Color.red;
		}
			if(Active && Input.GetKey(KeyCode.LeftAlt))GunCamera.transform.RotateAround (GunCamera.transform.position, Vector3.up, 180f * Time.deltaTime * Input.GetAxis ("Mouse X"));
			if (Input.GetKeyUp (KeyCode.LeftAlt) && Active)	GunCamera.transform.rotation = Turret.transform.rotation;
		//if(Active && Input.GetKey(KeyCode.LeftAlt))shipCamera.transform.RotateAround (shipCamera.transform.position, Vector3.right, 180f * Time.deltaTime * Input.GetAxis ("Mouse Y"));

	}
}
