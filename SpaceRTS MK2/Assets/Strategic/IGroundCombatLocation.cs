using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//stick on anything that can have a ground combat
public interface IBattlefield : ILocation {
	Battlefield GetBattlefield ();
	void RecieveTroops (List<GroundUnit> Troops);
	void SendTroops (List<GroundUnit> Troops);
	List<GroundUnit> GetTroops (bool GetDefenderTroops = true);
}
