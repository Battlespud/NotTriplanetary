using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerEndpoint : MonoBehaviour, IRepairable {
	public const int PowerLayer = 10;

	public Generator gen;
	public List<PowerNode> Nodes = new List<PowerNode> ();
	LineRenderer lr;
	GameObject lrObj;


	public bool Functional = true;

	public bool Recieving = false;

	public bool IsFunctional(){
		return Functional;
	}

	public void Repair(){
		Functional = true;
	}

	public string GetName(){
		return gameObject.name;
	}

	// Use this for initialization
	void Start () {
		lrObj = new GameObject ();
		lrObj.layer = PowerLayer;
		lrObj.transform.position = this.transform.position;
		lrObj.transform.rotation = this.transform.rotation;
		lr = lrObj.AddComponent<LineRenderer> ();
		lr.positionCount = 2 + Nodes.Count;
		lr.SetPosition (0, transform.position);
		foreach (PowerNode n in Nodes) {
			lr.SetPosition (Nodes.IndexOf (n) + 1, n.transform.position);
		}
		lr.SetPosition (lr.positionCount - 1, gen.transform.position);
		lr.SetWidth (.05f, .05f);
		lr.material = new Material(Shader.Find("Particles/Additive"));
	}
	
	// Update is called once per frame
	void Update () {
		Recieving = true;
		foreach (PowerNode n in Nodes) {
			if (!n.Functional)
				Recieving = false;
		}
		if (Recieving) {
			lr.SetColors (Color.green, Color.green);
		} else {
			lr.SetColors (Color.red, Color.red);
		}
		lr.SetPositions (new Vector3[0]);
		lr.SetPosition (0, transform.position);
		foreach (PowerNode n in Nodes) {
			lr.SetPosition (Nodes.IndexOf (n) + 1, n.transform.position);
		}
		lr.SetPosition (lr.positionCount - 1, gen.transform.position);
	}
}
