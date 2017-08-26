using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace Assets
{
    //construction request shiup, make sure to add using UnityEngine.AI;
    public class NegroBundleOfSticks : MonoBehaviour
    {
        public ConstructionRequests mission;
        public NavMeshAgent agent;

        private void Awake()
        {
            agent = gameObject.GetComponent<NavMeshAgent>();
        }

        public IEnumerator Building(Constructable con)
        {
            bool isOccupied = true;

            while (con && con.currTime < TimeToBuild)
            {
                con.currTime += time.deltaTime;
                yield return null;
            }
        }

        public void AssignRequest(ConstructionRequests req)
        {
            StopAllCoroutines();
            mission = req;
            agent.SetDestination(req.transform.position);
        }

        void OnTriggerEnter(Collider col)
        {
            if (mission.constr && col.getComponent<Constructable>() == const)
                StartCoroutine("Building", constr);
        }

        void OnTriggerExit(Collider col)
        {
            if (mission.constr && col.getComponent<Constructable>() == const)
                StopAllCoroutines();
        }

        void EndMission()
        {
            ConstructionManager.Finished(mission, this);
        }

        //ship class
    }
}