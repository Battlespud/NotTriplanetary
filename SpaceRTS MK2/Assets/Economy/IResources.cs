using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResources  {

	bool TakeResource(RawResources r, float amount);
	bool GiveResource(RawResources r, float amount);
	bool  HasResource(RawResources r, float amount);
	float ResourceAmount (RawResources r);
	GameObject GetGameObject();

}
