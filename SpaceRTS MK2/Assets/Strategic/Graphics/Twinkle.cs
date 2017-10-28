using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Twinkle : MonoBehaviour {

	public Renderer render;
	float speed = .2f;
	float goal = 1f;
	float current;
	// Use this for initialization
	void Start () {
		if (render == null) {
			render = GetComponent<Renderer> ();
		}
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync (this,LightControl ());
	}
	
	IEnumerator LightControl(){
		//goal = MathExtensions.QuickLerp(current,goal,
		yield return null;
	}
}
