using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Places a character can go, replaces the old Icharacter interface
public interface ILocation  {

	string GetLocationName();
	object GetLocation();
	System.Type GetLocType();
	void MoveCharacterToThis(Character c);
	void MoveCharacterFromThis(Character c);

}
