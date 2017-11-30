using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetsMakeSomeComponents : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Design test component
		ShipComponents c = new ShipComponents();
		c.Name = "Heavy Test";
		c.Mass = 500;
		c.CrewRequired = 200;
		c.SetHTK (3);

		ShipComponents b = new ShipComponents();
		b.Name = "Light Test";
		b.Mass = 250;
		b.CrewRequired = 50;
		b.SetHTK (2);

		ShipComponents bridge = new ShipComponents();
		bridge.Name = "Bridge";
		bridge.Mass = 50;
		bridge.CrewRequired = 15;
		bridge.AddAbility(AbilityCats.CONTROL);
		bridge.Category = CompCategory.REQUIRED;
		bridge.Description = "A command and control center for the ship's officers.";

		ShipComponents MilEngine = new ShipComponents ();
		MilEngine.Category = CompCategory.ENGINE;
		MilEngine.Name = "MilEngine Mk1";
		MilEngine.CrewRequired = 50;
		MilEngine.Mass = 200;
		MilEngine.AddAbility(AbilityCats.THRUST,(float)EngineTypes.MAGNETO,2f,1f);
		MilEngine.AddAbility(AbilityCats.USEFUEL,(float)EngineTypes.MAGNETO*MilEngine.Mass*5f);
		MilEngine.Description = "An engine designed for maximum thrust with minimum weight.";

		ShipComponents CivEngine = new ShipComponents ();
		CivEngine.Name = "CivEngine Mk1";
		CivEngine.Category = CompCategory.ENGINE;
		CivEngine.CrewRequired = 125;
		CivEngine.Mass = 500;
		CivEngine.AddAbility(AbilityCats.THRUST,(float)EngineTypes.MAGNETO,.75f,1f);
		CivEngine.AddAbility(AbilityCats.USEFUEL,(float)EngineTypes.MAGNETO*CivEngine.Mass);
		CivEngine.Description = "An engine designed for efficiency.";

		ShipComponents turnThruster = new ShipComponents ();
		turnThruster.Name = "Manuevering Thruster";
		turnThruster.AddAbility (AbilityCats.TURN, 35f);
		turnThruster.Mass = 25;
		turnThruster.CrewRequired = 10;
		turnThruster.Category = CompCategory.DEFAULT;
		turnThruster.Description = "Small thruster mounted on the exterior in order to improve turn rates.";

		ShipComponents berths = new ShipComponents();
		berths.Name = "Crew Quarters";
		berths.Mass = 100;
		berths.AddAbility (AbilityCats.CREW, 100);
		berths.Category = CompCategory.REQUIRED;
		berths.Description = "Area for the crew to sleep, eat and pray.";

		ShipComponents smBerths = new ShipComponents();
		smBerths.Name = "Crew Quarters (S)";
		smBerths.Mass = 10;
		smBerths.AddAbility (AbilityCats.CREW, 10);
		smBerths.Category = CompCategory.REQUIRED;
		smBerths.Description = "Small area for the crew to sleep, eat and pray.";

		ShipComponents Engineering = new ShipComponents();
		Engineering.Name = "Engineering Section";
		Engineering.Mass = 35;
		Engineering.CrewRequired = 15;
		Engineering.AddAbility(AbilityCats.MAINT,Engineering.Mass,25f,25f);
		Engineering.Category = CompCategory.UTILITY;
		Engineering.Description = "Provides engineering personnel and equipment to keep the ship in functional condition.";

		ShipComponents SparePartsLocker = new ShipComponents();
		SparePartsLocker.Name = "Maintenance Locker";
		SparePartsLocker.Mass = 25;
		SparePartsLocker.CrewRequired = 15;
		SparePartsLocker.AddAbility(AbilityCats.MAINT,0,50f,50f);
		SparePartsLocker.Category = CompCategory.UTILITY;
		SparePartsLocker.Description = "Contains a reserve of maintenance supplies.";

		ShipComponents CargoHoldLarge = new ShipComponents();
		CargoHoldLarge.Name = "Cargo Hold";
		CargoHoldLarge.Mass = 250;
		CargoHoldLarge.CrewRequired = 0;
		CargoHoldLarge.AddAbility(AbilityCats.CARGO,100f,0f,0f);
		CargoHoldLarge.Category = CompCategory.UTILITY;
		CargoHoldLarge.Description = "Empty space and the infrastructure needed to service it.";


		ShipComponents CargoHoldSmall = new ShipComponents();
		CargoHoldSmall.Name = "Cargo Hold (S)";
		CargoHoldSmall.Mass = 25;
		CargoHoldSmall.CrewRequired = 0;
		CargoHoldSmall.AddAbility(AbilityCats.CARGO,10f,0f,0f);
		CargoHoldSmall.Category = CompCategory.UTILITY;
		CargoHoldSmall.Description = "Empty space and the infrastructure needed to service it.";

		ShipComponents FuelLarge = new ShipComponents();
		FuelLarge.Name = "Fuel Cell";
		FuelLarge.Mass = 150;
		FuelLarge.CrewRequired = 0;
		FuelLarge.AddAbility(AbilityCats.FUEL,10000f,0f,0f);
		FuelLarge.Category = CompCategory.UTILITY;
		FuelLarge.Description = "A refillable fuel cell used to power modern engines.";


		ShipComponents FuelSmall = new ShipComponents();
		FuelSmall.Name = "Fuel Cell (S)";
		FuelSmall.Mass = 15;
		FuelSmall.CrewRequired = 0;
		FuelSmall.AddAbility(AbilityCats.FUEL,1000f,0f,0f);
		FuelSmall.Category = CompCategory.UTILITY;
		FuelSmall.Description = "A small fuel cell for lighter craft.";

		ShipComponents.AddComponentToPublicDomain (FuelSmall);
		ShipComponents.AddComponentToPublicDomain (FuelLarge);
		ShipComponents.AddComponentToPublicDomain (CargoHoldSmall);
		ShipComponents.AddComponentToPublicDomain (CargoHoldLarge);
		ShipComponents.AddComponentToPublicDomain (SparePartsLocker);


		ShipComponents.AddComponentToPublicDomain (bridge);
		ShipComponents.AddComponentToPublicDomain(smBerths);
		ShipComponents.AddComponentToPublicDomain (Engineering);
		ShipComponents.AddComponentToPublicDomain(berths);
		ShipComponents.AddComponentToPublicDomain (turnThruster);
		ShipComponents.AddComponentToPublicDomain (CivEngine);


		ShipComponents.AddComponentToPublicDomain (MilEngine);
		ShipComponents.AddComponentToPublicDomain (c);
		ShipComponents.AddComponentToPublicDomain (b);

		DesignScreenManager.LoadAllComponents = true;


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
