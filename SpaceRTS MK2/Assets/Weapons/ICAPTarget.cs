using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICAPTarget  {
	FAC GetFaction();
	bool isHostile (FAC attacker);
	GameObject GetGameObject ();

	void DealDamage(float dam, Vector3 origin, Transform en, List<Int2> pattern);
	void DealPhysicsDamage(float f, Vector3 origin, float fMag, Transform s, List<Int2> pattern);
}
