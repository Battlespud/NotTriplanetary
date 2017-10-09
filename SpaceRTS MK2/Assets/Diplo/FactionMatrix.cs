using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Diplo{
	WAR=0,
	PEACE,
	FRIEND,
	ALLY=3
};

public static class FactionMatrix {
	//records diplomatic relations between all groups as a 2d array


	static Diplo[,] RelationshipMatrix = new Diplo[MaxFactions,MaxFactions];

	public static void SetRelationship(int a, int b, Diplo relationship)
	{
		if (a > MaxFactions - 1 || a < 0 || b < 0 || b > MaxFactions - 1) {
			Debug.Log ("Invalid faction ids were entered, no action taken");
		} else {
			RelationshipMatrix [a, b] = relationship;
			RelationshipMatrix [b, a] = relationship;
		}
	}

	public static Diplo GetRelationship(int a, int b)
	{
		Diplo e;
		if (a > MaxFactions - 1 || a < 0 || b < 0 || b > MaxFactions - 1) {
			Debug.Log ("Invalid faction ids were entered, result will be incorrect");
			e = Diplo.WAR;
		} else {
			Diplo d = RelationshipMatrix[a,b];
			Diplo r = RelationshipMatrix[b,a];
			e = d;
			if (d != r) {
				Debug.Log ("The relationships are different, something went wrong, result will be incorrect");
			}

		}
		return e;
	}

	public static bool IsHostile(FAC s, FAC o){
		int self = (int)s;
		int other = (int)o;
		Diplo e;
		if (self > MaxFactions - 1 || self < 0 || other < 0 || other > MaxFactions - 1) {
			Debug.Log ("Invalid faction ids were entered, result will be incorrect");
			e = Diplo.WAR;
		} else {
			Diplo d = RelationshipMatrix[self,other];
			Diplo r = RelationshipMatrix[other,self];
			e = d;
			if (d != r) {
				Debug.Log ("The relationships are different, something went wrong, result will be incorrect");
			}
		}
		return e == Diplo.WAR;

	}

	static FactionMatrix(){
		//initial setup
		FactionMatrix.SetRelationship (0, 1, Diplo.WAR);
		FactionMatrix.SetRelationship (0, 2, Diplo.ALLY);
		FactionMatrix.SetRelationship (0, 3, Diplo.PEACE);
		FactionMatrix.SetRelationship (0, 0, Diplo.ALLY);

		FactionMatrix.SetRelationship (1, 2, Diplo.WAR);
		FactionMatrix.SetRelationship (1, 3, Diplo.WAR);

		FactionMatrix.SetRelationship (2, 3, Diplo.PEACE);
	}

	//static stuff
	public const int MaxFactions = 4;

	static string faction0 = "Player";
	static string faction1 = "Pirates";
	static string faction2 = "SpaceSquids";
	static string faction3 = "KosmoSlavs";
	public static int[] FactionIDs = new int[MaxFactions];
	public static string[] FactionNames = new string[MaxFactions]{faction0,faction1,faction2,faction3 };
	public static Color[] FactionColors = new Color[MaxFactions]{ Color.cyan, Color.grey, Color.magenta, Color.red };

}

public enum FAC{
	PLAYER = 0,
	PIRATE = 1,
	SPACESQUID = 2,
	KOSMOSLAV = 3
};


