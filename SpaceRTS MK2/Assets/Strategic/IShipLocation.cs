using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShipLocation : ISearchable, IPosition {
    string GetLocationName();
    object GetLocation();
    System.Type GetLocType();
}
