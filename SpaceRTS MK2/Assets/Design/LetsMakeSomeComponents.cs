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
		bridge.control = true;
		bridge.Category = CompCategory.REQUIRED;

		ShipComponents flagBridge = new ShipComponents();
		flagBridge.Name = "Flag Bridge";
		flagBridge.Mass = 250;
		flagBridge.CrewRequired = 100;
		flagBridge.control = true;
		flagBridge.flagControl = true;
		flagBridge.Category = CompCategory.REQUIRED;

		ShipComponents MilEngine = ShipComponents.DesignEngine (EngineTypes.MAGNETO, 300, 1f);
		MilEngine.Name = "MilEngine2";
		MilEngine.CrewRequired = 50;

		ShipComponents turnThruster = new ShipComponents ();
		turnThruster.Name = "Manuevering Thruster";
		turnThruster.TurnThrust = 25;
		turnThruster.Mass = 25;
		turnThruster.CrewRequired = 10;
		turnThruster.isEngine = true;
		turnThruster.Category = CompCategory.DEFAULT;

		ShipComponents berths = new ShipComponents();
		berths.Name = "Crew Quarters";
		berths.Mass = 100;
		berths.quarters = 250;
		berths.Category = CompCategory.REQUIRED;

		ShipComponents smBerths = new ShipComponents();
		smBerths.Name = "Crew Quarters";
		smBerths.Mass = 10;
		smBerths.quarters = 25;
		smBerths.Category = CompCategory.REQUIRED;

		ShipComponents lifeSupport = new ShipComponents();
		lifeSupport.Name = "Life Support";
		lifeSupport.Mass = 100;
		lifeSupport.lifeSupport = 500;
		lifeSupport.Category = CompCategory.REQUIRED;

		ShipComponents smLifeSupport = new ShipComponents();
		smLifeSupport.Name = "Small Life Support";
		smLifeSupport.Mass = 10;
		smLifeSupport.lifeSupport = 50;
		smLifeSupport.Category = CompCategory.REQUIRED;

		ShipComponents.DesignedComponents.Add (flagBridge);
		ShipComponents.DesignedComponents.Add (bridge);
		ShipComponents.DesignedComponents.Add (smLifeSupport);
		ShipComponents.DesignedComponents.Add (lifeSupport);
		ShipComponents.DesignedComponents.Add(smBerths);
		ShipComponents.DesignedComponents.Add(berths);
		ShipComponents.DesignedComponents.Add (turnThruster);


		ShipComponents.DesignedComponents.Add (MilEngine);
		ShipComponents.DesignedComponents.Add (c);
		ShipComponents.DesignedComponents.Add (b);

		foreach(ShipComponents ce in ShipComponents.DesignedComponents){
			ce.GetFields();
		}

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
