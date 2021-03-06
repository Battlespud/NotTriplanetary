﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;



public class ContextMenu : MonoBehaviour
{

    GameObject c;

    // Use this for initialization
    void Start()
    {

    }

    float ySize = 25f;
    float xSize = 100f;

    bool destroy = true;
    float dTime = 3f;

    // Update is called once per frame
    void Update()
    {

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);


        if (Input.GetMouseButtonDown(1))
        {
            Destroy(c);
            //c = new GameObject ();
            c = Instantiate(Resources.Load<GameObject>("Canvas") as GameObject);
            c.GetComponent<Canvas>().worldCamera = Camera.main;
            c.transform.position = mousePos;
            RaycastHit[] hits;
           // hits = Physics.RaycastAll(mousePos, Vector3.down, 100f);  //FOR ORTHOGRAPHIC
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			Debug.DrawLine (ray.origin, ray.direction*1000f,Color.green,20f);
			hits = Physics.RaycastAll (ray, 1000f);
            List<IContext> context = new List<IContext>();
            foreach (RaycastHit h in hits)
            {
                if (h.collider.GetComponent<IContext>() != null)
                {
                    context.Add(h.collider.GetComponent<IContext>());
                }
            }

            int i = 0;
            //make the first menu
            foreach (IContext con in context)
            {
                Debug.Log("Context detects " + con.getGameObject().name);
                GameObject gxt = new GameObject();
                Text txt = gxt.AddComponent<Text>();
                gxt.transform.parent = c.transform;
                gxt.transform.position = new Vector3(c.transform.position.x, c.transform.position.y - ySize  * i, c.transform.position.z);
                txt.text = con.getGameObject().name;
                int j = 0;
                foreach (UnityAction action in con.ContextActions())
                {
                    GameObject bgo = Instantiate(Resources.Load<GameObject>("Button") as GameObject);
                    bgo.transform.parent = c.transform;
                    bgo.transform.position = new Vector3(c.transform.position.x + xSize * i, c.transform.position.y - ySize * j, c.transform.position.z);
                    //button.OnClick(action);
                    bgo.GetComponent<Button>().onClick.AddListener(action);


                    string name = action.Method.ToString().Substring(5);
                    bgo.GetComponentInChildren<Text>().text = "<color=black>"+ name.Substring(0, name.Length - 2) +"</color>";
                    j++;
                }
                i++;
            }
        }
        if (destroy)
        {
            dTime-= Time.deltaTime;
            if (dTime <= 0f)
            {
                dTime = 3f;
                Destroy(c);
            //    c = new GameObject();
         //       destroy = false;
            }
        }

    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
    //        destroy = true;
        }
    }
}