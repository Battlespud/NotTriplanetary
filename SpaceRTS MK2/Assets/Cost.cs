using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cost
{
    public List<KeyValuePair<RawResourceTypes, float>> cost;
    public Cost(RawResourceTypes[] resources, float[] amounts) //the sizes of each array need to match eachother so normally id check for this, but im going to assume A. youre not retarded and B. you won't allow the player acces to this
    {
        cost = new List<KeyValuePair<RawResourceTypes, float>>();
        for (int i = 0; i < resources.Length; i++)
        {
            cost.Add(new KeyValuePair<RawResourceTypes, float>(resources[i], amounts[i]));
        }
    }

    /// <summary>
    /// Deduct cost of whatever holds a cost value from the colony's resource pool.
    /// </summary>
    /// <param name="colony"></param>
    public void DeductCost(Colony colony)
    {
        //implement deduction logic here; colony needs its implementation before we can do this
    }
}