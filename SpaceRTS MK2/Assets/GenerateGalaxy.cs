using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateGalaxy : MonoBehaviour {

	public int NumStars = 1700;

	public float minDist = 150;
	public float maxDist = 450;

	public float bound = 10000;

	//10k max 250

	public float maxLaneDist = 450f;

	GameObject star;

	public int count;

	public bool DoCalc = false;

	public List<GameObject> stars = new List<GameObject>();

	public GameObject parent;

	// Use this for initialization
	void Awake () {
		star = Resources.Load<GameObject> ("StarMarker") as GameObject;
		StartCoroutine ("Generate");
	}

	IEnumerator Generate(){
		while (stars.Count < NumStars) {
			List<GameObject> working = new List<GameObject> ();
			for (int i = 0; i < NumStars/4; i++) {
				GameObject g = Instantiate (star);
				if(parent)
				g.transform.parent = parent.transform;
				star.transform.position = new Vector3 (Random.Range (-bound, bound), 0f, Random.Range (-bound, bound));
				working.Add (g);
			}
			List<GameObject> W2 = new List<GameObject> ();
			W2.AddRange (stars);
			stars.AddRange (working);
			working.AddRange (W2);
			Debug.Log ("Stars initial count");
			foreach (GameObject g in working) {
				bool tooFar = true;
				foreach (GameObject f in working) {
					if (Vector3.Distance (g.transform.position, f.transform.position) < minDist && g!=f) {
						if(stars.Contains(g))stars.Remove (g); break;
					}
					if (Vector3.Distance (g.transform.position, f.transform.position) <= maxDist && g!=f) {
						tooFar = false;
					}
				}
				if (tooFar && stars.Contains (g)) {
					stars.Remove (g);
				}
			}
			foreach (GameObject d in working) {
				if (!stars.Contains (d))
					Destroy (d);
			}
			Debug.Log ("End frame count: " + stars.Count);
			yield return null;
		}
		foreach (GameObject st in stars) {
			foreach (GameObject st2 in stars) {
				if (st != st2 && Vector3.Distance (st.transform.position, st2.transform.position) <= maxLaneDist) {
					GameObject go = new GameObject ();
					LineRenderer lr = go.AddComponent<LineRenderer> ();
					lr.SetPosition (0, st.transform.position);
					lr.SetPosition (1, st2.transform.position);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		count = stars.Count;
		if (Input.GetKeyDown (KeyCode.Space)) {
			foreach (GameObject g in stars){
				Destroy (g);
			}
			StartCoroutine ("Generate");
		}

		if (DoCalc) {
			DoCalc = false;
			GameObject j = stars [Random.Range (0, stars.Count + 1)];
			float dist = 9999f;
			foreach (GameObject g in stars) {
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
