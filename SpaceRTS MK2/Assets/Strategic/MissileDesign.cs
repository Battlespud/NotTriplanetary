using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileDesign {

	public string MissileDesignName;
	public int NumberBuilt=0;

	public MissilePropulsionDesign Propulsion;
	public MissileWarheadDesign Warhead;
	public MissileElectronicsDesign Electronics;

	public float MaxAcceleration;
	public float BurnTime;
	public float TotalAcceleration; //total delta V
	public float Mass;

	public MissileDesign MIRV;
	public int MIRVCount = 0;

	public MissileDesign(string n, MissilePropulsionDesign prop, MissileWarheadDesign war, MissileElectronicsDesign elec, MissileDesign M = null, int mCount = 0){
		MissileDesignName = n;

		Propulsion = prop;
		Warhead = war;
		Electronics = elec;

		MIRV = M;
		MIRVCount = mCount;
		 
		if (MIRVCount > 0) {
			Mass = prop.Mass + war.Mass + elec.Mass + MIRV.Mass*MIRVCount;
		} else {
			Mass = prop.Mass + war.Mass + elec.Mass;
		}
		CalculateAccel ();
	}

	void CalculateAccel(){
		MaxAcceleration = Propulsion.CalculateAccel() / ((float)Mass / 50f);
		BurnTime = Propulsion.MaxFuel / Propulsion.FuelBurn;
		TotalAcceleration = MaxAcceleration * BurnTime;
	}

}

public class MissilePropulsionDesign{
	public float MaxFuel;
	public float FuelBurn;
	EngineTypes EngineType;
	public float PowerMod = 5f;

	public float Signature;
	public float Mass;

	public float CalculateAccel(){
		return (Mass / 50) * (1 + PowerMod * (int)EngineType);
	}

}

public class MissileWarheadDesign{
	public float Damage;

	public float Signature;
	public float Mass;
}

public class MissileElectronicsDesign{
	//TODO
	public float Mass;
}
