using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCameraController : MonoBehaviour {

	//just stick it on the camera in the planet and space scenes.
	Camera cam;
	const float BaseSensitivity = 5;
	float Sensitivity;
	Vector3 offset;

	// Use this for initialization
	void Start () {
		cam = Camera.main;
		StartCoroutine(InputListener());

		Sensitivity = BaseSensitivity;
	}
	
	// Update is called once per frame
	void Update () {
		InputLoop ();

		transform.position = new Vector3 (transform.position.x + offset.x, transform.position.y, transform.position.z + offset.z);
	}

	void InputLoop(){
		offset = new Vector3 ();
		if(Input.GetKey (KeyCode.LeftShift)){
			Sensitivity = BaseSensitivity *2.5f *Time.deltaTime;
		}
		else{
			Sensitivity = BaseSensitivity*Time.deltaTime;
		}
		offset.z = Input.GetAxis ("Vertical") * Sensitivity;
		offset.x = Input.GetAxis ("Horizontal") * Sensitivity;

		float zoom = Input.GetAxis ("Mouse ScrollWheel");
		if (zoom > 0)
			cam.orthographicSize -= Sensitivity * 150;
		else if (zoom < 0)
			cam.orthographicSize += Sensitivity * 150;
		if (cam.orthographicSize < 3)
			cam.orthographicSize = 3;

	}


	private float doubleClickTimeLimit = 0.25f;


	private IEnumerator InputListener() 
	{
		while(enabled)
		{ //Run as long as this is activ

			if(Input.GetMouseButtonDown(0))
				yield return ClickEvent();

			yield return null;
		}
	}

	private IEnumerator ClickEvent()
	{
		//pause a frame so you don't pick up the same mouse down event.
		yield return new WaitForEndOfFrame();

		float count = 0f;
		while(count < doubleClickTimeLimit)
		{
			if(Input.GetMouseButtonDown(0))
			{
				DoubleClick();
				yield break;
			}
			count += Time.deltaTime;// increment counter by change in time between frames
			yield return null; // wait for the next frame
		}
		SingleClick();
	}

	void DoubleClick(){
		Vector3 mousePos = cam.ScreenToWorldPoint (Input.mousePosition);
		transform.position = new Vector3 ( mousePos.x, transform.position.y, mousePos.z);
	}

	void SingleClick(){

	}
	

}

