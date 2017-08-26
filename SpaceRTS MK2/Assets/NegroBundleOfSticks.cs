using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

    //construction request shiup, make sure to add using UnityEngine.AI;
    public class NegroBundleOfSticks : MonoBehaviour
    {
        public ConstructionRequests mission;
        public NavMeshAgent agent;

        void Awake()
        {
		agent = gameObject.GetComponent<NavMeshAgent>();
		agent.Warp(new Vector3(transform.position.x, .59f, transform.position.y));
		ConstructionManager.availableConShips.Add (this);
        }

	IEnumerator Building(Constructable con)
        {
            bool isOccupied = true;
		Debug.Log ("Beginning construction");
            while (con && con.currTime < con.TimeToBuild)
            {
                con.currTime += Time.deltaTime;
                yield return null;
            }
		EndMission ();
        }

        public void AssignRequest(ConstructionRequests req)
        {
            StopAllCoroutines();
            mission = req;
		agent.SetDestination(mission.constr.transform.position);
        }

        void OnTriggerEnter(Collider col)
        {
		if (mission != null) {
			if (col.GetComponent<Constructable> () == mission.constr)
				StartCoroutine ("Building", mission.constr);
			agent.Stop();
		}
        }

        void OnTriggerExit(Collider col)
        {
		if(mission != null){
			if (col.GetComponent<Constructable>() == mission.constr)
                StopAllCoroutines();
			}	
        }

        void EndMission()
        {
            ConstructionManager.Finished(mission, this);
        }

        //ship class
    }
