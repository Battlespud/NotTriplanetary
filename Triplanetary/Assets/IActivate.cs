using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IActivate {

	//Stick this on anything y ou want to be activatible on key press, like a door, elevator, treasure chest whatev.

	void Activate (Activator a);

	string GetToolTip();

}
