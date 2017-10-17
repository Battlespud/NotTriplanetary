using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAC : SpaceGun {

	// Use this for initialization
	void Start () {
		Damage = 15f;
		ForceMagnitude = 25f;
		ReloadTime = 5f;
		powerCost = 1;
		Initialize ();
		lineC = Color.yellow;
		Pattern = new List<Int2> (){new Int2 (0, 0), new Int2 (0, -1),new Int2 (0, -2),new Int2 (0, -3),new Int2 (0, -4), new Int2 (1, 0),new Int2 (1, -1),new Int2 (-1, 0),new Int2 (-1, -1)};
	}
	

}
