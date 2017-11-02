using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategicMissile {


	public StrategicMissile(MissileDesign des){
		Design = des;
		MaxAcceleration = des.MaxAcceleration;
		DeltaV = des.TotalAcceleration;

	}

	MissileDesign Design;
	public float DeltaV;


	public float MaxAcceleration = 10f;


	public void Accelerate(){
		if (MaxAcceleration <= DeltaV) {
			DeltaV -= MaxAcceleration;
		} else {
			MaxAcceleration = DeltaV;
			DeltaV -= MaxAcceleration;
		}
	}
}
