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
		Pattern = new List<Vector2> (){new Vector2 (0, 0), new Vector2 (0, -1), new Vector2 (1, 0), new Vector2 (-1, 0)};
	}

}
