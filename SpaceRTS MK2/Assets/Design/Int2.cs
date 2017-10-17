using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Int2{
	public int x;
	public int y;
	public Int2 VecToInt(Vector2 vec){
		Int2 g;
		g.x = (int)vec.x;
		g.y = (int)vec.y;
		return g;
	}
	public Int2(int a, int b){
		x=a;
		y=b;
	}
	public Int2 (float a, float b){
		x = (int)a;
		y = (int)b;
	}
}
