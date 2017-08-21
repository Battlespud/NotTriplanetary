using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPDTarget {

	void HitByPD(int dam);
	int GetFaction();
	GameObject GetGameObject ();

}
