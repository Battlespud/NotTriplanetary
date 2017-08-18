using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElectricRepairToolComp : MonoBehaviour {

	Ray activationRay;
	public const float ActivationDistance = 2f;
	public Text ActivationToolTipText;
	string emptyString;
	[SerializeField]public GameObject activego;

	int LayMask = 1 << 11;

	// Use this for initialization
	void Start () {
		emptyString = ("");
	}
	// Update is called once per frame
	void Update () {
		//		activationRay = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		try{
			if (Physics.Raycast (transform.position, transform.forward, out hit, ActivationDistance, LayMask)) {
				if (hit.collider.GetComponent<IRepairable> () != null ) {
					IRepairable active = hit.collider.GetComponent<IRepairable> ();
					activego = hit.collider.gameObject;
					if (Input.GetKeyDown (KeyCode.R) && !active.IsFunctional()) {
						StartCoroutine("Repair", active);
					} else {
						if(active.IsFunctional())
							ActivationToolTipText.text = active.GetName() + " is fully functional.";
						else
							ActivationToolTipText.text = "Press R to Repair " + active.GetName();
					}
				} else {
					ActivationToolTipText.text = emptyString;
				}
			} else {
				ActivationToolTipText.text = emptyString;
			}
		}
		catch{
			//idc
		}
	}

	IEnumerator Repair(IRepairable active){
		float timeToRepair = 3f; //in seconds
		while (timeToRepair > 0f) {
			timeToRepair -= Time.deltaTime;
			RaycastHit hit;
			if (Physics.Raycast (transform.position, transform.forward, out hit, ActivationDistance, LayMask)) {
				if (hit.collider.GetComponent<IRepairable> () == null) {
					StopAllCoroutines ();
				}
			} else {
				StopAllCoroutines ();
			}
			if(ActivationToolTipText != null)
			ActivationToolTipText.text = timeToRepair.ToString();
			yield return null;
		}
		active.Repair ();
		float f = .35f;
		while (f > 0f) {
			ActivationToolTipText.text =  "Repaired " + active.GetName();
			yield return null;
		}
	}
}
