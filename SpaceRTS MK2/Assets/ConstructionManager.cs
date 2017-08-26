using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


//data class to store reqst
//must contain gameObject target
//ship that is going to do the construction, starts off as null
public class ConstructionManager
{
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

    static void NotTheUpdateLoop()
    {
        //to assign open construction request to avaailable conShips
        if (opened.Count == 0 || availableConShips.Count == 0)
            return;

        if (availableConShips.Count >= opened.Count)
        {
            while (availableConShips.Count > 0)
            {
                NegroBundleOfSticks conShipZero = new NegroBundleOfSticks();
                List<ConstructionRequests> reqsts = new List<ConstructionRequests>();
                reqsts.AddRange(opened);
                reqsts.OrderBy(targ => Vector3.Distance(conShipZero.transform.position, targ.transform.position)).ToList();
                AssignMission(reqsts[0], conShipZero);
            }
        }
        else
        {

        }
    }

    static void AssignMission(ConstructionRequests conReq, NegroBundleOfSticks conShip)
    {
        closed.Add(conReq);
        unavailableConShips.Add(conShip);
    }
}

public class ConstructionRequests : MonoBehaviour
{
    public GameObject construction;
    public NegroBundleOfSticks conShip;
}

public class NegroBundleOfSticks : MonoBehaviour
{
    //ship class
}
