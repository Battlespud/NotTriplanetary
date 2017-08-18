using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatorButton : MonoBehaviour, IActivate {

	[SerializeField] //for some reason unity doesnt expose interfaces in the inspector..?
	public IActivate target;

	public GameObject targetGO;


	public void Activate(Activator a){
		target.Activate (a);
	}

	public string GetToolTip(){
		return target.GetToolTip ();
	}

	void Awake(){
		target = targetGO.GetComponent<IActivate> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
