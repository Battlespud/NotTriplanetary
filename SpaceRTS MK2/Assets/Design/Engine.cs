using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EngineTypes{
	SHIP,
	FIGHTER,
	MISSILE
};

public class Engine : ShipComponents {


	EngineTypes EngineType;


	public float Thrust;
	public float TurnRate;

	public float FuelConsumption; //per tick


	public Engine Clone(){
		Engine e = new Engine ();
		e.Thrust = Thrust;
		e.TurnRate = TurnRate;
		e.FuelConsumption = FuelConsumption;
		CloneProperties (this, e);
		return e;
	}
}
