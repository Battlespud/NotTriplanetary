using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;



public class PatternDesigner : MonoBehaviour {

	public GameObject Prefab;

	public List<GameObject>Cubes = new List<GameObject>(); //all
	public List<ToggleBlock> Blocks = new List<ToggleBlock> (); //only selected

	public bool UseInt2List = true;

	public InputField Name;

	const int Columns = 15;  //MUST BE ODD
	const int Rows = 9;

	void SelectBlock(ToggleBlock b){
		b.Toggle ();
		if (Blocks.Contains (b))
			Blocks.Remove (b);
		if (b.Active) {
			Blocks.Add (b);
			b.SetOrder(Blocks.Count);
			Debug.Log ("Set order #" + Blocks.Count);
		}
	}

	// Use this for initialization
	void Start () {
	}

	public void Clear(){
		int count = Cubes.Count;
		Blocks.Clear ();
		for (int i = 0; i < count; i++) {
			GameObject toDestroy = Cubes [0];
			Cubes.Remove (toDestroy);
			Destroy (toDestroy);
		}
	}

	public void Build(){
		GameObject MiddleCube = Instantiate<GameObject> (Prefab) as GameObject;
		MiddleCube.GetComponent<Renderer> ().material.color = Color.black;
		MiddleCube.transform.position = new Vector3 (0, 2f, 88f);
		MiddleCube.GetComponent<ToggleBlock> ().OrderText.text = "M";
		MiddleCube.GetComponent<ToggleBlock> ().Delete ();
		Cubes.Add (MiddleCube);
		for(int y = 0; y > Rows*-1;y--){
		for (int x = (Columns - 1) / -2; x <= Columns / 2; x++) {
			GameObject g = Instantiate<GameObject> (Prefab) as GameObject;
			g.transform.position = new Vector3 ((float)x*2, (float)y*2, 88f);
				g.GetComponent<ToggleBlock> ().x = x;
				g.GetComponent<ToggleBlock> ().y = y;
			Cubes.Add (g);
			}
		}
	}


	public void Save(){
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Patterns/" + Name.text + ".txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			writer.Write ("public List<Int2> " + Name.text + "Pattern = new List<Int2>(){");
			foreach (ToggleBlock b in Blocks) {
				writer.Write(string.Format("new Int2 ({0}, {1}), ",b.x,b.y));
			}
			writer.Write ("};");
		}
	}

	public void SaveAlter(){
		List<float> percents = new List<float> ();
		List <int> count = new List<int> ();
		//TODO
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Patterns/" + Name.text + ".txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			writer.Write ("public List<Int2> " + Name.text + "Pattern = new List<Int2>(){");
			foreach (ToggleBlock b in Blocks) {
				writer.Write(string.Format("new Int2 ({0}, {1}), ",b.x,b.y));
			}
			writer.Write ("};");
		}
	}


	Vector3 mousePos;

	// Update is called once per frame
	void Update () {
		mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Ray clickRay = new Ray (mousePos, Vector3.forward);
		RaycastHit hit;
		if (Input.GetMouseButtonDown (0)) {
			if (Physics.Raycast (clickRay, out hit, 10000f)) {
				try{
				SelectBlock (hit.collider.GetComponent<ToggleBlock> ());
				}
				catch{
				}
			}
		}

	}
}
