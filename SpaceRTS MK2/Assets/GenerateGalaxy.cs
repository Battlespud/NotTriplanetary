using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class GenerateGalaxy : MonoBehaviour {

	 int NumStars = 400;

	 float minDist = 450;
	 float maxDist = 1100;

	float bound = 60*OutlineCircleStarMarker.SYSRADIUS;

	//10k max 250

	public float maxLaneDist = 650;

	GameObject star;

	public int count;

	public bool DoCalc = false;

	public List<Vector3> stars = new List<Vector3>();
	public List<GameObject> starsObj = new List<GameObject>();

	public GameObject parent;

	// Use this for initialization
	void Awake () {
		star = Resources.Load<GameObject> ("StarMarker") as GameObject;
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,Generate());
	}

	IEnumerator Generate(){
		while (stars.Count < NumStars) {
			List<Vector3> working = new List<Vector3> ();
			List<Vector3> W2 = new List<Vector3> ();
			yield return Ninja.JumpToUnity;
			for (int i = 0; i < NumStars/8f; i++) {
				Vector3 g = new Vector3(Random.Range (-bound, bound), 0f, Random.Range (-bound, bound));
		//		if(parent)
		//		g.transform.parent = parent.transform;
		//		star.transform.position = new Vector3 (Random.Range (-bound, bound), 0f, Random.Range (-bound, bound));
				working.Add (g);
			}
			yield return Ninja.JumpBack;
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
		}
		yield return Ninja.JumpToUnity;
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
			/*
			GameObject g = new GameObject();
			for (int i = 0; i < st.transform.childCount; i++) {
				st.transform.GetChild (i).transform.parent = g.transform;
			}
			st.transform.localScale = new Vector3 (35f, 35f, 35f);
			for (int i = 0; i < g.transform.childCount; i++) {
				st.transform.GetChild (i).transform.parent = st.transform;
			}
			*/
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


}
