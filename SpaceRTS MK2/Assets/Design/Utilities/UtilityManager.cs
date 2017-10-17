using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UtilityManager : MonoBehaviour {

	public GameObject ComponentDesigner;
	public GameObject PatternDesigner;

	public void SwitchToComponentDesigner(){
		ComponentDesigner.SetActive (true);
		PatternDesigner.GetComponent<PatternDesigner> ().Clear ();
		PatternDesigner.SetActive (false);
	}

	public void SwitchToPatternDesigner(){
		ComponentDesigner.SetActive (false);
		PatternDesigner.SetActive (true);
		PatternDesigner.GetComponent<PatternDesigner> ().Build ();

	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
