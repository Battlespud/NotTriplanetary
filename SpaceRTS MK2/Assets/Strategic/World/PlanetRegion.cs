using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetRegion : ILocation, IRecieveCombatResult{
	public Empire Owner;
	public Colony RegionColony;

	public string RegionName;
	public int RegionSize;
	public RegionTypes RegionType;
	public bool Explored = false;

	public PlanetRegion(){
		RegionSize = Random.Range (2, 11);
		RegionType = (RegionTypes)Random.Range (0, 6);
		RegionName = "<Unexplored " + RegionType.ToString();
	}

	public void ChangeOwner(Empire e){
		if (Owner != null) {
			EmpireLogEntry log = new EmpireLogEntry (LogCategories.MILITARY, 2, Owner, "REGION LOST", string.Format ("{0} on {1} has fallen to the forces of {2}.", RegionName,Planet.RegionToPlanet[this].PlanetName,e.EmpireName));
			EmpireLogEntry log2 = new EmpireLogEntry(LogCategories.MILITARY,2,e,"REGION CONQUERED",string.Format ("{0} on {1} has fallen to our forces.", RegionName,Planet.RegionToPlanet[this].PlanetName));

		} else {
			EmpireLogEntry log = new EmpireLogEntry(LogCategories.MILITARY,4,e,"REGION CONTROLLED",string.Format("{0} on {1} is now under our control.",RegionName,Planet.RegionToPlanet[this].PlanetName));
		}
		Owner = e;
		if (RegionColony != null)
			RegionColony.ChangeOwner (e);
	}

	public string GetLocationName(){
		return RegionName;
	}
	public object GetLocation(){
		return (object)this;
	}
	public System.Type GetLocType(){
		return this.GetType ();
	}
	public void MoveCharacterToThis(Character c){
		if (!Explored) {
			Explored = true;
			RegionName = string.Format("{0}'s {1}",c.CharName,RegionType.ToString() );
			ChangeOwner(c.empire);
			EmpireLogEntry log2 = new EmpireLogEntry(LogCategories.EXPLORATION,3,Owner,"REGION EXPLORED",string.Format ("{0} on {1} has been explored.", RegionName,Planet.RegionToPlanet[this].PlanetName));
		}
	}
	public void MoveCharacterFromThis(Character c){
	}
}
