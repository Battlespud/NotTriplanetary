using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FacilityType
{
	city = 0, //economic/research/planetary, kinda misc i guess
	taxAgency,
	commercialPort,
	terraforming,
	environmental,
	research,
	mining = 100, //resource/construction
	processing,
	agriculture,
	componentFactory,
	fighterFactory,
	ordnanceFactory,
	militaryPort,
	officer = 200, // units recruitment/building
	troop,
	armor,
	crew,
	forcefield = 300, //planetary def
	bunker,
	antiAir,
	orbitalDefense,
	earlyWarning,
}

public static class Facilities
{
    public static Dictionary<FacilityType, string> FacilityNames;
    public static Dictionary<FacilityType, float> BaseCosts;
    public static Dictionary<FacilityType, float> BaseValues;
    /*
     	NONE = 0,
	    METAL=1,
	    LUMBER=2,
	    OIL=3,
	    CROPS=4,
	    GOLD=5
     */
    static Facilities()
    {
        int[] resourceCode;
        foreach (FacilityType type in Enum.GetValues(typeof(FacilityType)))
        {
            switch (type)
            {
                case FacilityType.city:
                    break;
                case FacilityType.taxAgency:
                    break;
                case FacilityType.commercialPort:
                    break;
                case FacilityType.terraforming:
                    break;
				case FacilityType.environmental:
					break;
                case FacilityType.research:
                    break;
                case FacilityType.mining:
                    break;
                case FacilityType.processing:
                    break;
                case FacilityType.agriculture:
                    break;
                case FacilityType.componentFactory:
                    break;
                case FacilityType.fighterFactory:
                    break;
				case FacilityType.ordnanceFactory:
                    break;
                case FacilityType.militaryPort:
                    break;
                case FacilityType.officer:
                    break;
                case FacilityType.troop:
                    break;
				case FacilityType.armor:
                    break;
                case FacilityType.crew:
                    break;
                case FacilityType.forcefield:
                    break;
                case FacilityType.bunker:
                    break;
                case FacilityType.antiAir:
                    break;
                case FacilityType.orbitalDefense:
                    break;
			case FacilityType.earlyWarning:
                    break;
                default:
                    break;
            }
        }
    }

    public static void ActivateFacility(Colony colony)
    {
		colony.Facilities.ForEach (x => {
			switch (x) { //make sure to actually add the facilities field to the colony class, afaik it only needs to be a list of FacilityType
			case FacilityType.city:
                //income/econmics, population
				break;
			case FacilityType.taxAgency:
                //new tax bill 2017
				break;
			case FacilityType.commercialPort:
                //io for commericial logistics
				break;
			case FacilityType.terraforming:
                //terraform planet
				break;
			case FacilityType.research:
                //advance percentage of research
				break;
			case FacilityType.mining:
                //mine resources
				break;
			case FacilityType.processing:
                //raw resources processing
				break;
			case FacilityType.agriculture:
                //feed the peeps
				break;
			case FacilityType.componentFactory:
                //build ship components
				break;
			case FacilityType.fighterFactory:
                //build fighters for the ships
				break;
			case FacilityType.ordnanceFactory:
                //build the explody things
				break;
			case FacilityType.militaryPort:
                //military related io, checkpoints
				break;
			case FacilityType.officer:
                //officer training
				break;
			case FacilityType.troop:
                //ground troop training
				break;
			case FacilityType.armor:
                //heavy armor unit training
				break;
			case FacilityType.crew:
                //space marines
				break;
			case FacilityType.forcefield:
                //planetary forcefield
				break;
			case FacilityType.bunker:
                //group units' defense augmentation
				break;
			case FacilityType.antiAir:
                //invasion defense
				break;
			case FacilityType.orbitalDefense:
                //orbital... defense
				break;
			case FacilityType.earlyWarning:
                //EA for inc ships
				break;
			default:
				break;
			}
		});
    }
}