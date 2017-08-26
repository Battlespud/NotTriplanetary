using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemGenerator : MonoBehaviour {

	float SystemSize;
	static GameObject PlanetPrefab;
	static GameObject AsteroidPrefab;

	public int numPlanets = 3;
	 bool asteroidBelt = false;

	// Use this for initialization
	void Awake(){
		if (!PlanetPrefab || !AsteroidPrefab) {
			PlanetPrefab = Resources.Load <GameObject>("Planet") as GameObject;
			AsteroidPrefab = Resources.Load <GameObject>("Asteroid") as GameObject;
		}
	}

	void Start () {
		SystemSize = OutlineCircleStarMarker.SYSRADIUS;
		numPlanets = Random.Range (0, 3);



		for (int i = 0; i < numPlanets; i++) {
			ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync (this, GeneratePlanet ((i+1) * (SystemSize / numPlanets)));
			//GeneratePlanet ((i+1) * (SystemSize / numPlanets));
		}
		if (asteroidBelt)
			GenerateAsteroidRing(50f);


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void GenerateAsteroidRing(float r){
		List<Vector3> vecs = new List<Vector3> ();
		int numSegments = 255;
		float radius = r/12;
		float deltaTheta = (float) (2.0 * Mathf.PI) / 256;
		float theta = 0f;
		for (int i = 0 ; i < numSegments + 1 ; i++) {
			float x = radius * Mathf.Cos(theta);
			float z = radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, 0, z);
			vecs.Add(pos);
			theta += deltaTheta;
		}
		foreach (Vector3 v in vecs) {
			GameObject g = Instantiate (AsteroidPrefab);
			g.transform.parent = gameObject.transform;
			g.transform.localPosition = v;
		}
	}

	IEnumerator GeneratePlanet(float r){
		List<Vector3> vecs = new List<Vector3> ();
		int numSegments = 126;
		float radius = r/12;
		float deltaTheta = (float) (2.0 * Mathf.PI) / 256;
		float theta = 0f;
		for (int i = 0 ; i < numSegments + 1 ; i++) {
			float x = radius * Mathf.Cos(theta);
			float z = radius * Mathf.Sin(theta);
			Vector3 pos = new Vector3(x, 0, z);
			vecs.Add(pos);
			theta += deltaTheta;
		}
		yield return Ninja.JumpToUnity;
		Vector3 v = vecs [Random.Range (0, vecs.Count - 1)];
		GameObject g = Instantiate (PlanetPrefab);
		g.transform.parent = gameObject.transform;
		g.transform.localPosition = v;
	}

}
