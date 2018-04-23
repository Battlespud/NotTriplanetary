using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;


public class ShipHull {
    
    //fluff
    public const float MINIMUM_LENGTH = 10;
    public const float MINIMUM_BEAM = 10;
    public const float MINIMUM_RATIO = 5;
    public const float BATTLESHIP_CUTOFF = 25000; //Where we're comfortable defining a ship as a battleship.
    
    
    //All existing ship hulls organized by designer (usually empire)
    public static Dictionary<DesignerToken,List<ShipHull>> HullDict = new Dictionary<DesignerToken, List<ShipHull>>();

    public static void AddHull(ShipHull h, [CanBeNull] DesignerToken d = null)
    {
        if (d == null)
            d = h.Designer;
        if (HullDict.ContainsKey(d))
        {
            HullDict[d].AddExclusive(h);
        }
        else
        {
            HullDict.Add(d, new List<ShipHull>(){h});
        }
        Debug.Log(h.HullName + " added to Hull Design Dictionary for " + d.OwnerName);
    }


    public string HullName;
    public DesignerToken Designer;

    
    //Base Design Stats
    public int ArmorLayers;
    public float ArmorLength;
    public ArmorTypes ArmorType;
    public float ArmorSize;
    
    //Aspect
    public float Length; //Armor length * 10 Meters if whole ship is armored.
    public float Beam;   //
    public float Draft;
    
    
    public int Size;
    public float UsableSize;

    public float DesignLife;  //Quality of the materials etc.  How long ships of this hull are planned to be in use for.  Not the same as maintenance life.  Hull wear & tear causes permanent debuffs once this is exceeded. Huge impact on price.

    //Design Slots
    public Dictionary<HardpointTypes,List<Hardpoint>> Hardpoints = new Dictionary<HardpointTypes, List<Hardpoint>>();

    public Dictionary<HardpointTypes, List<Hardpoint>> CloneHardpoints()
    {
        Dictionary<HardpointTypes, List<Hardpoint>> cloned = new Dictionary<HardpointTypes, List<Hardpoint>>();
        foreach (HardpointTypes ht in Enum.GetValues(typeof(HardpointTypes)))
        {
            if (Hardpoints.ContainsKey(ht))
            {
                List<Hardpoint> points = new List<Hardpoint>();
                foreach (var hp in Hardpoints[ht])
                {
                    points.Add(hp.Clone());
                }
                cloned.Add(ht,points);
            }
        }
        return cloned;
    }
    
        
    void CalculateArmorWidth(){
        int hs = Size / 50;
        ArmorLength =(int)(1 + 1.1*(Mathf.Pow (hs, .55f))); //.66 works well
        ArmorSize = 50/(int)ArmorType * (ArmorLength * (ArmorLayers + (int)Mathf.Pow(ArmorLayers, .35f)));
    }

    public void Finalize()
    {
        CalculateArmorWidth();
        UsableSize = Size;
        UsableSize -= ArmorSize;
    }

}
