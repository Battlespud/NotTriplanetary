using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StrategicExtensions {

	public const float yLayer = 20f;

	public static Vector3 OnLayer(Vector3 vec){
		return new Vector3 (vec.x, vec.y, yLayer);
	}
}
