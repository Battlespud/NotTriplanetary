using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour {

	public bool busy;
	public Camera cam;

	public CharacterController controller;
	public Vector3 offset;
	float rOffset;
	Rigidbody rb;



	public float speed = 10.0f;
	public float gravity = 10.0f;
	public float maxVelocityChange = 10.0f;
	public bool canJump = true;
	public float jumpHeight = 2.0f;
	public bool grounded = false;

	// Use this for initialization
	void Awake () {
	//	controller = GetComponent<CharacterController> ();
		cam = GetComponentInChildren<Camera> ();
		rb = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//if (grounded) {
		// Calculate how fast we should be moving
		Vector3 targetVelocity = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
		targetVelocity = transform.TransformDirection (targetVelocity);
		targetVelocity *= speed;

		// Apply a force that attempts to reach our target velocity
		Vector3 velocity = rb.velocity;
		Vector3 velocityChange = (targetVelocity - velocity);
		velocityChange.x = Mathf.Clamp (velocityChange.x, -maxVelocityChange, maxVelocityChange);
		velocityChange.z = Mathf.Clamp (velocityChange.z, -maxVelocityChange, maxVelocityChange);
		velocityChange.y = 0;

		// Jump
		if (canJump && Input.GetButton ("Jump") && !busy) {
			rb.velocity = new Vector3 (velocity.x, CalculateJumpVerticalSpeed (), velocity.z);
		}
		//	}
		offset = new Vector3 ();
		rOffset = 0f;
		offset += Input.GetAxis ("Vertical") * transform.forward;
		offset += Input.GetAxis ("Horizontal") * transform.right;
		if (!busy) {
			rb.AddForce (velocityChange, ForceMode.VelocityChange);
			transform.RotateAround (transform.position, Vector3.up, 180f * Time.fixedDeltaTime * Input.GetAxis ("Mouse X"));
		}
	}

		float CalculateJumpVerticalSpeed () {
			// From the jump height and gravity we deduce the upwards speed 
			// for the character to reach at the apex.
			return Mathf.Sqrt(2 * jumpHeight * gravity);
		}
	}


