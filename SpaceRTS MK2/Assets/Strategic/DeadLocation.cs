using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadLocation : ILocation {

	public static DeadLocation Loc;

	public string GetLocationName(){
		return "Unknown";
	}
	public object GetLocation(){
		return (object)this;
	}
	public System.Type GetLocType(){
		return this.GetType ();
	}
	public void MoveCharacterToThis(Character c){
	}
	public void MoveCharacterFromThis(Character c){
	}

	public string GetSearchableString()
	{
		return "";
	}

	public Vector3 GetPosition()
	{
		return new Vector3();
	}
	
	public DeadLocation(){
		Loc = this;
	}

}
