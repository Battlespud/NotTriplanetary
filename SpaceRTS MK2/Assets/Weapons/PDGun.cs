using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PDGun : SpaceGun {

	static GameObject bullet;


	float force = 130f;

	public override void Fire(){
		if (pdTarget == null)
				return;
		GameObject g = Instantiate (bullet);
		PDBullet p = g.GetComponent<PDBullet> ();
		g.transform.position = this.transform.position;
		try{
		Vector3 targetPos = pdTarget.GetGameObject ().transform.position;
		}
		catch{
			Debug.Log ("<color=red>Avoiding Target Lockup Error, PDGun #22</color> This warning can be safely ignored.");
			pdTarget = null;
			return;
		}
		g.transform.rotation = transform.rotation;
//		g.transform.rotation = g.transform.rotation * Quaternion.LookRotation(new Vector3(targetPos.x + Random.Range(targetPos.x - .1f*targetPos.x,targetPos.x - .11f*targetPos.x),targetPos.y,targetPos.z + Random.Range(targetPos.z - .1f*targetPos.z,targetPos.z - .11f*targetPos.z)));
		g.transform.Rotate(new Vector3(0f,Random.Range(-7f,7f),0f));
		p.rb.AddForce (g.transform.forward * force);
		//	StartCoroutine ("Reload");
		//	CanFire = false;

	}

	public override void Update () {
		ResetTarget ();
		Fire ();
	}

	public override void CustomInitialize(){
		tType = TurretType.PD;
		ReloadTime = .0015f;
		powerCost = 0f;
		if(bullet == null)
			bullet = Resources.Load<GameObject> ("PDBullet") as GameObject;

	}
}
