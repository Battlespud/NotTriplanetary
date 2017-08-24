using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGalaxy : MonoBehaviour {

	 int NumStars = 500;

	 float minDist = 225f;
	 float maxDist = 550f;

	float bound = 60f*OutlineCircleStarMarker.SYSRADIUS;

	//10k max 250

	public float maxLaneDist = 400f;

	GameObject star;

	public int count;

	public bool DoCalc = false;

	public List<Vector3> stars = new List<Vector3>();
	public List<GameObject> starsObj = new List<GameObject>();

	public GameObject parent;

	// Use this for initialization
	void Awake () {
		star = Resources.Load<GameObject> ("StarMarker") as GameObject;
		StartCoroutine ("Generate");
	}

	IEnumerator Generate(){
		while (stars.Count < NumStars) {
			List<Vector3> working = new List<Vector3> ();
			List<Vector3> W2 = new List<Vector3> ();
			for (int i = 0; i < NumStars/8f; i++) {
				Vector3 g = new Vector3(Random.Range (-bound, bound), 0f, Random.Range (-bound, bound));
		//		if(parent)
		//		g.transform.parent = parent.transform;
		//		star.transform.position = new Vector3 (Random.Range (-bound, bound), 0f, Random.Range (-bound, bound));
				working.Add (g);
			}
			W2.AddRange (stars);
			stars.AddRange (working);
			working.AddRange (W2);
			Debug.Log ("Stars initial count");
			foreach (Vector3 g in working) {
				bool tooFar = true;
				foreach (Vector3 f in working) {
					if (Vector3.Distance (g, f) < minDist && g!=f) {
						if(stars.Contains(g))stars.Remove (g); break;
					}
			//		if (Vector3.Distance (g, f) <= maxDist && g!=f) {
						tooFar = false;
			//		}
				}
				if (tooFar && stars.Contains (g)) {
					stars.Remove (g);
				}
			}
			foreach (Vector3 d in working) {
				if (!stars.Contains (d)){
					//if gameobject then delete, dw about vectors
				}
				}
			Debug.Log ("End frame count: " + stars.Count);
			yield return null;
		}
		foreach(Vector3 vec in stars){
			GameObject f = Instantiate (star);
			f.transform.position = vec;
			starsObj.Add (f);
		}
		foreach (GameObject st in starsObj) {
			foreach (GameObject st2 in starsObj) {
				if (st != st2 && Vector3.Distance (st.transform.position, st2.transform.position) <= maxLaneDist) {
					GameObject go = new GameObject ();
					LineRenderer lr = go.AddComponent<LineRenderer> ();
					lr.SetPosition (0, st.transform.position);
					lr.SetPosition (1, st2.transform.position);
					st.GetComponent<OutlineCircleStarMarker> ().Connections.Add (st2);
					st2.GetComponent<OutlineCircleStarMarker> ().Connections.Add (st);

				}
			}
		}
		foreach (GameObject st in starsObj) {
			if (st.GetComponent<OutlineCircleStarMarker> ().Connections.Count < 1) {
				st.GetComponent<OutlineCircleStarMarker> ().Connections.Add (GetClosest (st));
				GameObject go = new GameObject ();
				LineRenderer lr = go.AddComponent<LineRenderer> ();
				lr.SetPosition (0, st.transform.position);
				lr.SetPosition (1, st.GetComponent<OutlineCircleStarMarker>().Connections[0].transform.position);
				st.GetComponent<OutlineCircleStarMarker>().Connections[0].GetComponent<OutlineCircleStarMarker> ().Connections.Add (st);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		count = stars.Count;
		if (Input.GetKeyDown (KeyCode.Space)) {
			foreach (GameObject g in starsObj){
				Destroy (g);
				stars.Clear ();
			}
			StartCoroutine ("Generate");
		}

		if (DoCalc) {
			DoCalc = false;
			GameObject j = starsObj [Random.Range (0, stars.Count + 1)];
			float dist = 9999f;
			foreach (GameObject g in starsObj) {
				if (g != j) {
					float d = Vector3.Distance (g.transform.position, j.transform.position);
					if (d < dist)
						dist = d;
				}
			}
			Debug.Log("Closest is " + dist);
		}
	}

	GameObject GetClosest(GameObject j){
		DoCalc = false;
		float dist = 9999f;
		GameObject closest = new GameObject ();
		foreach (GameObject g in starsObj) {
			if (g != j) {
				float d = Vector3.Distance (g.transform.position, j.transform.position);
				if (d < dist) {
					closest = g;
					dist = d;
				}
			}
		}
		return closest;
	}
}
