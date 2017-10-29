using UnityEngine;
using System.Collections;

public class ScrollUV : MonoBehaviour {

	void Start(){
		iniRot = transform.rotation;
	}

	void Update () {

		MeshRenderer mr = GetComponent<MeshRenderer>();

		Material mat = mr.material;

		Vector2 offset = mat.mainTextureOffset;

		offset.x += Time.deltaTime / 10f;

		mat.mainTextureOffset = offset;

	}


	Quaternion iniRot;

	void LateUpdate(){
		transform.rotation = iniRot;
	}

}
