using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRepairable {

	bool IsFunctional ();
	void Repair();
	string GetName();
}
