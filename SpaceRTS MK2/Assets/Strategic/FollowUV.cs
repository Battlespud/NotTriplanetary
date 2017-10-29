using UnityEngine;
using System.Collections;

public class FollowUV : MonoBehaviour {

	public float parralax = 2f;
	public bool UseParallax;
	float y;


	void Start(){
		iniRot = transform.rotation;
		y = transform.position.y;
	}

	void Update () {
		if (UseParallax) {
			MeshRenderer mr = GetComponent<MeshRenderer> ();

			Material mat = mr.material;

			Vector2 offset = mat.mainTextureOffset;

			offset.x = transform.position.x / transform.localScale.x / parralax;
			offset.y = transform.position.y / transform.localScale.y / parralax;

			mat.mainTextureOffset = offset;
		}
	}
	Quaternion iniRot;

	void LateUpdate(){
		transform.rotation = iniRot;
	}

}
