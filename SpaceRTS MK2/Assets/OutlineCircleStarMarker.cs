using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutlineCircleStarMarker : MonoBehaviour {

	// Use this for initialization
	void Start () {
		RenderCircle (GetComponent<LineRenderer> (), 20f);
	}



	// Update is called once per frame
	void Update () {
		
	}

	void RenderCircle (LineRenderer l, float r) {
		int numSegments = 255;
		float radius = r;
		l.material = new Material(Shader.Find("Particles/Additive"));
		l.SetColors(Color.yellow, Color.yellow);
		l.SetWidth(2f,2f);
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
}
