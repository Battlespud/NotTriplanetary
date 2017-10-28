using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathExtensions {

	public static void RefClamp01(ref float number){
		if (number > 1){
			number = 1;
		}  
		else if(number <0){
			number = 0;
		}
	}
	public static Vector3 QuickLerp3(Vector3 one, Vector3 two, float t){
		RefClamp01(ref t);

		return new Vector3(
			one.x + (two.x - one.x)*t,
			one.y + (two.y - one.y)*t,
			one.z + (two.z - one.z)*t
		);
	}

	public static float QuickLerp(float start, float end, float t){
		RefClamp01(ref t);
		return start + (end - start) * t;

	}
}
