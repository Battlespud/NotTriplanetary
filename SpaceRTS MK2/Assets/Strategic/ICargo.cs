using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICargo {

	string GetCargoName();
	float GetSize();
	object GetCargo();
	System.Type GetCargoType();
	void SetLocation(ILocation Loc);
	void DestroyCargo ();
	bool Load ();
}
