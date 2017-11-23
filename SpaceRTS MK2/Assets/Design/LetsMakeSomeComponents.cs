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

		ShipComponents MilEngine = new ShipComponents ();
		MilEngine.Name = "MilEngineMk2";
		MilEngine.CrewRequired = 50;
		MilEngine.Mass = 200;
		MilEngine.AddAbility(AbilityCats.THRUST,(float)EngineTypes.MAGNETO,1f,1f);

		ShipComponents turnThruster = new ShipComponents ();
		turnThruster.Name = "Manuevering Thruster";
		turnThruster.AddAbility (AbilityCats.TURN, 35f);
		turnThruster.Mass = 25;
		turnThruster.CrewRequired = 10;
		turnThruster.Category = CompCategory.DEFAULT;

		ShipComponents berths = new ShipComponents();
		berths.Name = "Crew Quarters";
		berths.Mass = 100;
		berths.AddAbility (AbilityCats.CREW, 100);
		berths.Category = CompCategory.REQUIRED;

		ShipComponents smBerths = new ShipComponents();
		smBerths.Name = "Crew Quarters (S)";
		smBerths.Mass = 10;
		smBerths.AddAbility (AbilityCats.CREW, 10);
		smBerths.Category = CompCategory.REQUIRED;

		ShipComponents Engineering = new ShipComponents();
		Engineering.Name = "Engineering Section";
		Engineering.Mass = 35;
		Engineering.CrewRequired = 15;
		Engineering.AddAbility(AbilityCats.MAINT,Engineering.Mass,25f,25f);
		Engineering.Category = CompCategory.UTILITY;

		ShipComponents.AddComponentToPublicDomain (bridge);
		ShipComponents.AddComponentToPublicDomain(smBerths);
		ShipComponents.AddComponentToPublicDomain (Engineering);
		ShipComponents.AddComponentToPublicDomain(berths);
		ShipComponents.AddComponentToPublicDomain (turnThruster);


		ShipComponents.AddComponentToPublicDomain (MilEngine);
		ShipComponents.AddComponentToPublicDomain (c);
		ShipComponents.AddComponentToPublicDomain (b);

		DesignScreenManager.LoadAllComponents = true;


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
