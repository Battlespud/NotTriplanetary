//ConstructionManager
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


//data class to store reqst
//must contain gameObject target
//ship that is going to do the construction, starts off as null
public class ConstructionManager
{

    static public GameObject ConstructablePrefab;

    static public List<ConstructionRequests> opened;
    static public List<ConstructionRequests> closed;
    static public List<NegroBundleOfSticks> availableConShips;
    static public List<NegroBundleOfSticks> unavailableConShips;

    static ConstructionManager()
    {
        opened = new List<ConstructionRequests>();
        closed = new List<ConstructionRequests>();
        availableConShips = new List<NegroBundleOfSticks>();
        unavailableConShips = new List<NegroBundleOfSticks>();
    }

    public static void UpdateMissionsQuery()
    {
        //to assign open construction request to avaailable conShips
        while (availableConShips.Count > 0 && opened.Count > 0)
        {
			Debug.Log ("Looping");
            NegroBundleOfSticks conShipZero = availableConShips[0];
            List<ConstructionRequests> reqsts = new List<ConstructionRequests>();
            reqsts.AddRange(opened);
            reqsts.OrderBy(targ => Vector3.Distance(conShipZero.transform.position, targ.constr.transform.position)).ToList();
            //reqsts.AddRange(opened.OrderBy(targ => Vector3.Distance(conShipZero.transform.position, targ.constr.transform.position)).ToList());
            AssignMission(reqsts[0], conShipZero);
        }
    }

    static void AssignMission(ConstructionRequests conReq, NegroBundleOfSticks conShip)
    {
		opened.Remove (conReq);
        closed.Add(conReq);
		availableConShips.Remove (conShip);
        unavailableConShips.Add(conShip);
        conShip.AssignRequest(conReq);
    }

    static public void Finished(ConstructionRequests req, NegroBundleOfSticks ship)
    {
		closed.Remove(req);
		unavailableConShips.Remove(ship);
		availableConShips.Add(ship);
		UpdateMissionsQuery ();
    }
}