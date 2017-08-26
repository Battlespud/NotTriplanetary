using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ContextMenu : MonoBehaviour
{
	// Use this for initialization
	void Start () {

	}




	float ySize = 5f;
	float xSize = 5f;

	// Update is called once per frame
	void Update ()
	{
		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);
		Ray clickRay = new Ray (mousePos, Vector3.down);


		if (Input.GetMouseButtonDown (1)) {
			GameObject c = Instantiate(Resources.Load<GameObject>("Canvas") as GameObject);
			c.transform.position = mousePos;
			RaycastHit[] hits;
			hits = Physics.RaycastAll (mousePos, Vector3.down, 100f);    
			List<IContext> context = new List<IContext> ();
			foreach (RaycastHit h in hits) {
				if (h.collider.GetComponent<IContext> () != null) {
					context.Add (h.collider.GetComponent<IContext> ());
				}
			}

			int i = 0;
			//make the first menu
			foreach (IContext con in context) {

				GameObject gxt = new GameObject() ;
				Text txt = gxt.AddComponent<Text> ();
				gxt.transform.parent = c.transform;
				gxt.transform.position = new Vector3 (c.transform.position.x, c.transform.position.y, c.transform.position.z - ySize * i);
				txt.text = con.getGameObject ().name;
				int j = 0;
				foreach (UnityAction action in con.ContextActions()) {
					GameObject bgo = Instantiate(Resources.Load<GameObject>("Button") as GameObject);
					bgo.transform.parent = c.transform;
					bgo.transform.position = new Vector3 (c.transform.position.x + xSize * i, c.transform.position.y, c.transform.position.z - ySize * j);
					//button.OnClick(action);
					bgo.GetComponent<Button>().onClick.AddListener( action);
					bgo.GetComponentInChildren<Text>().text = action.ToString();
					j++;
				}
				i++;
			}
		}
	}
}







