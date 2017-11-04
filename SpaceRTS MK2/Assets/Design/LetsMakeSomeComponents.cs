using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetsMakeSomeComponents : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Design test component
		ShipComponents.DesignedComponents.Clear ();

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


		ShipComponents.DesignedComponents.Add (bridge);
		ShipComponents.DesignedComponents.Add(smBerths);
		ShipComponents.DesignedComponents.Add(berths);
		ShipComponents.DesignedComponents.Add (turnThruster);


		ShipComponents.DesignedComponents.Add (MilEngine);
		ShipComponents.DesignedComponents.Add (c);
		ShipComponents.DesignedComponents.Add (b);

		DesignScreenManager.LoadAllComponents = true;
		foreach(ShipComponents ce in ShipComponents.DesignedComponents){
		//	ce.GetFields();
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
