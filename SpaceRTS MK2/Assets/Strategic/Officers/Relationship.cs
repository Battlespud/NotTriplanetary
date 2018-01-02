using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Each character has a relationship with every other it has encountered.
public class Relationship{

	public enum RelationshipTypes //TODO
	{
		HatedEnemy,
		Enemy,
		Unfriendly,
		Neutral,
		Friendly,
		VeryFriendly,
		Ally
	}
	public static Dictionary<RelationshipTypes, string> RelationshipTypeNames = new Dictionary<RelationshipTypes, string> ();
	public Character Other;

	List<RelationshipModifier> Modifiers = new List<RelationshipModifier>();

	//Sums all the modifiers to get a final result. 
	public int GetRelationshipValue(){
		int Total = 0;
		Modifiers.ForEach (x => {
			Total += x.Amount;
		});
		return Total;
	}

	public void AddMod(RelationshipModifier M){
		Modifiers.Add (M);
	}

	public List<RelationshipModifier> GetList(){
		return Modifiers;
	}

	public Relationship(Character o){
		Other = o;
		Modifiers.Add(new RelationshipModifier(o,0,"<color=red>-START-</color>"));
	}


}

//Relationship Modifiers generally exist to indicate a single event that changed a characters perception of another.  
public class RelationshipModifier{
	public string Date;				//automatically set.
	public string Description;	   //What the event was
	public int Amount;             //How much this affects relationship.   Negative is bad. Positive is good.
	public Character Other;			
	public RelationshipModifier(Character c, int a, string desc){
		Other = c;
		Amount = a;
		Description = desc;
		Date = StrategicClock.GetDate ();
	}
}