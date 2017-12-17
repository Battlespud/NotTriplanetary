using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cost
{
    public List<KeyValuePair<RawResource, float>> cost;
    public Cost(RawResource[] resources, float[] amounts) //the sizes of each array need to match eachother so normally id check for this, but im going to assume A. youre not retarded and B. you won't allow the player acces to this
    {
        cost = new List<KeyValuePair<RawResource, float>>();
        for (int i = 0; i < resources.Length; i++)
        {
            cost.Add(new KeyValuePair<RawResource, float>(resources[i], amounts[i]));
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