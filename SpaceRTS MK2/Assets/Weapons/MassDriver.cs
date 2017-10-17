using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MassDriver : SpaceGun {

	public void Start(){
		Damage = 5f;
		ForceMagnitude = 100f;
		ReloadTime = .75f;
		powerCost = 1;
		Initialize ();
		lineC = Color.grey;
		Pattern = new List<Int2> (){new Int2 (0, 0), new Int2 (0, -1), new Int2 (1, 0), new Int2 (-1, 0)};
	}

}
