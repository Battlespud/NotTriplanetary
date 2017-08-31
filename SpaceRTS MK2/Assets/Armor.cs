using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour {

	static bool[,] penBlock = new bool[3,3];

	bool[,] armor = new bool[6,3];

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void DoDamage(bool[,] pen){
		int startX = Random.Range (0, armor.GetLength (0) + 1);
		int startY = 0;
		for(startY = armor.GetLength(1); startY >= 0; startY--){
			if (armor [startX, startY] == true) {
				break;
			}
		}
		int xx = 0; int yy = 0;

		for (int x = 0; x < pen.GetLength(0); x += 1) {
			for (int y = 0; y < pen.GetLength(1); y += 1) {

			}
		}

	}



}
