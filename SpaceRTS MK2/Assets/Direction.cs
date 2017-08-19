using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GeneralDirection {
	None,
	Forwards,
	Back,
	Left,
	Right,
	Up,
	Down
}



public static class Direction {

	//ToDo not quite accurate. Doesnt account for rotation for some reason. Probably an issue in inputs, not the formula itself.

	// update right now its only telling us where the other ship is in world space terms, without adjusting via our own rotation
	public static GeneralDirection GetDirection (Vector3 PositionShotFrom, Transform enemy, Vector3 ourPosition, Transform us) {

		GeneralDirection result = GeneralDirection.None;
		float shortestDistance = Mathf.Infinity;
		float distance = 0;

		Vector3 vectorPosition = ourPosition + PositionShotFrom;

		distance = Mathf.Abs (((ourPosition + us.forward) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Forwards;
		}
		distance = Mathf.Abs (((ourPosition  -us.forward) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Back;
		}
		distance = Mathf.Abs (((ourPosition + Vector3.up) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Up;
		}
		distance = Mathf.Abs (((ourPosition + -Vector3.up) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Down;
		}
		distance = Mathf.Abs (((ourPosition - us.right) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Left;
		}
		distance = Mathf.Abs (((ourPosition + us.right) - PositionShotFrom).magnitude);
		if (distance < shortestDistance)
		{
			shortestDistance = distance;
			result = GeneralDirection.Right;
		}

		return result;

	}
}
