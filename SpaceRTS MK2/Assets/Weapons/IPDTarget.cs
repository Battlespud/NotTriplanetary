using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPDTarget {

	void HitByPD(int dam);
	FAC GetFaction();
	bool isHostile (FAC attacker);
	GameObject GetGameObject ();

}
