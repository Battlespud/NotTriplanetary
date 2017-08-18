using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum CurrentType{
	AC,
	DC,
	NA //if it doesnt matter 
};

public enum EMaterials{
	COPPER,
	SILVER,
	TUNGSTEN,


};

public struct MaterialStats{
	public float r;
	public float sHeat;
	public MaterialStats(float a, float b){
		r = a;
		sHeat = b;
	}
}
public static class Electricity {

	// current = i
	//voltage = v
	//resistance = r


	public static MaterialStats GetMaterialStats(EMaterials e){
		switch (e) {
		case EMaterials.COPPER:
			{
				return new MaterialStats (2f, 5f);
			}

		case EMaterials.SILVER:
			{
				return new MaterialStats(1f,2.5f);
			}

		case EMaterials.TUNGSTEN:
			{
				return new MaterialStats (5f, 50f);
			}

		}
		return new MaterialStats (0f, 0f); //this will crash the program 
	}


	public static float GetCurrent(float voltage, float resist){
		return voltage / resist;
	}

	public static float GetVoltage(float current, float resist){
		return current * resist;
	}



	//public static float GetHeat(

}
