using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Team : ILocation{
	const int MaxTeamMembers = 5;

	public string TeamName = "Team";

	public TeamTypes TeamType;
	
	public ILocation Location;
	public Vector3 GetPosition()
	{
		return Location.GetPosition();
	}

	public string GetSearchableString()
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("Team" + TeamType.ToString());
		Members.ForEach(x =>
		{
			sb.Append(x.GetNameString());
		});
		return sb.ToString();
	}
	public Empire empire;

	public List<Character> Members = new List<Character>();

	public string GetLocationName(){
		return TeamName + " (" + Location.GetLocationName() + ")";
	}
	public object GetLocation(){
		return (object)this;
	}
	public System.Type GetLocType(){
		return this.GetType ();
	}

	public void MoveCharacterToThis(Character c){
		if (Members.Count + 1 <= MaxTeamMembers) {
			Members.Add (c);
			c.JoinsTeam (this);
		} else {
			//pass them off to wherever the team itself is located
			c.MoveTo (Location);
		}
	}

	public void MoveCharacterFromThis(Character c){
		RemoveMember (c);
		c.Unassign ();
	}

	void AddMember(Character C){
		if (!Members.Contains (C))
			Members.Add (C);
	}

	void RemoveMember(Character C){
		if (!Members.Contains (C)){
			//Debug.LogError (TeamName + " cannot remove because it does not contain " + C.GetNameString ());
		}else {
			Members.Remove (C);
			if (Members.Count == 0)
				Disband ();
		}
	}

	void Disband(){
		empire.Teams.Remove (this);
		Members.ForEach (x => {
			x.MoveTo (Location);
		});
	}
}
