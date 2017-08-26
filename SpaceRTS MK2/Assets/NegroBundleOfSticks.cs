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

        private void Awake()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
        }

	IEnumerator Building(Constructable con)
        {
            bool isOccupied = true;

            while (con && con.currTime < con.TimeToBuild)
            {
                con.currTime += Time.deltaTime;
                yield return null;
            }
        }

        public void AssignRequest(ConstructionRequests req)
        {
            StopAllCoroutines();
            mission = req;
            agent.SetDestination(req.constr.transform.position);
        }

        void OnTriggerEnter(Collider col)
        {
			if (mission.constr && col.GetComponent<Constructable>() == mission.constr)
				StartCoroutine("Building", mission.constr);
        }

        void OnTriggerExit(Collider col)
        {
			if (mission.constr && col.GetComponent<Constructable>() == mission.constr)
                StopAllCoroutines();
        }

        void EndMission()
        {
            ConstructionManager.Finished(mission, this);
        }

        //ship class
    }
