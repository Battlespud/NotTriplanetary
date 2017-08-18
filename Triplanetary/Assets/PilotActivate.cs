using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotActivate : MonoBehaviour, IActivate {

	string toolTip = "Pilot Ship..";
	public GameObject pilot;
	public Camera shipCamera;
	public GameObject Ship;
	public List<Renderer> ShipRenderers = new List<Renderer>();
	public Material Wireframe;
	public Material Default;

	public GameObject PilotUI;

	PowerEndpoint Power;
	public bool Powered = false;


	//Movement
	float Force = 2000f;
	Rigidbody rb;

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
		pilot = a.gameObject;
		pilot.transform.parent = Ship.transform;
		a.GetComponent<MovementController> ().busy = true;
		a.GetComponent<MovementController> ().cam.enabled = false;
		EnableShipCamera ();
	}

	void TurnOff(Activator a){
		Active = false;
		pilot.transform.parent = null;
		a.GetComponent<MovementController> ().busy = false;
		a.GetComponent<MovementController> ().cam.enabled = true;
		shipCamera.enabled = false;
		shipCamera.transform.rotation = this.transform.rotation;
		foreach (Renderer r in ShipRenderers) {
			r.material = Default;
		}
	}

	void EnableShipCamera(){
		StartCoroutine ("ShipCameraOn");
	}

	IEnumerator ShipCameraOn(){
		shipCamera.enabled = true;

		shipCamera.fieldOfView = 0f;
		foreach (Renderer r in ShipRenderers) {
			r.material = Wireframe;
		}
		while (shipCamera.fieldOfView < 180f) {
			shipCamera.fieldOfView = Mathf.Lerp (shipCamera.fieldOfView, 360f, 1f * Time.deltaTime);
			yield return null;
		}
		while (shipCamera.fieldOfView > 90f) {
			shipCamera.fieldOfView = Mathf.Lerp (shipCamera.fieldOfView, 90f, 1f * Time.deltaTime);
			if (shipCamera.fieldOfView > 60f) 
				Active = true;
			if (shipCamera.fieldOfView > 88f && shipCamera.fieldOfView < 92f)
				shipCamera.fieldOfView = 90f;
			yield return null;
		}
		StartCoroutine ("ShipControls");
	}

	IEnumerator ShipCameraOff(){
		shipCamera.enabled = true;
		Active = false;
		shipCamera.fieldOfView = 90f;
		foreach (Renderer r in ShipRenderers) {
			r.material = Wireframe;
		}
		while (shipCamera.fieldOfView < 180f) {
			shipCamera.fieldOfView = Mathf.Lerp (shipCamera.fieldOfView, 180f, 1f * Time.deltaTime);

			if (shipCamera.fieldOfView > 170f && shipCamera.fieldOfView < 200f)
				shipCamera.fieldOfView = 180f;
			yield return null;
		}
		TurnOff (pilot.GetComponent<Activator>());
			}


	private WaitForFixedUpdate waitF = new WaitForFixedUpdate();
	IEnumerator ShipControls(){
		Cursor.visible = false;
		Debug.Log ("Ship controls online.");
		while (Active) {
			if (Input.GetKey (KeyCode.Space)) {

			} else {
				
				rb.AddForce (Ship.transform.forward * Force * Input.GetAxis ("Vertical"));
				rb.AddForce (Ship.transform.right * Force * Input.GetAxis ("Horizontal"));
				if (Input.GetKey (KeyCode.Q)) {
					rb.AddTorque (transform.forward * 2500f);
				}
				if (Input.GetKey (KeyCode.E)) {
					rb.AddTorque (transform.forward * -2500f);
				}
				if (Mathf.Abs (Input.GetAxis ("Mouse Y"))  > .1f && !Input.GetKey(KeyCode.LeftAlt))
					rb.AddTorque (transform.right * 6000 * Input.GetAxis ("Mouse Y"));
				if (Mathf.Abs (Input.GetAxis ("Mouse X"))  > .1f && !Input.GetKey(KeyCode.LeftAlt))
					rb.AddTorque (transform.up * 6000 * Input.GetAxis ("Mouse X"));
			}

			yield return waitF;
		}
		Cursor.visible = true;
	}


	Renderer ren;
	// Use this for initialization
	void Start () {
		ren = gameObject.GetComponent<Renderer> ();
		shipCamera.enabled = false;
		ShipRenderers.AddRange(Ship.GetComponentsInChildren<Renderer> ());
		Default = ShipRenderers[0].material;
		rb = Ship.GetComponent<Rigidbody> ();
		Power = GetComponent<PowerEndpoint> ();
	}
	
	// Update is called once per frame
	void Update () {
		Powered = Power.Recieving;
		if(Active&& !Powered){
			StopAllCoroutines ();
			StartCoroutine ("ShipCameraOff");
		}
		if(Powered)
			ren.material.color = Color.cyan;
		else
			ren.material.color = Color.red;
		if(Active && Input.GetKey(KeyCode.LeftAlt))shipCamera.transform.RotateAround (shipCamera.transform.position, Vector3.up, 180f * Time.deltaTime * Input.GetAxis ("Mouse X"));
		if (Input.GetKeyUp (KeyCode.LeftAlt) && Active)	shipCamera.transform.rotation = Ship.transform.rotation;
		//if(Active && Input.GetKey(KeyCode.LeftAlt))shipCamera.transform.RotateAround (shipCamera.transform.position, Vector3.right, 180f * Time.deltaTime * Input.GetAxis ("Mouse Y"));

	}
}
