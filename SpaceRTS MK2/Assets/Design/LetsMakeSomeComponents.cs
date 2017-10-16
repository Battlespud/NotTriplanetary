using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetsMakeSomeComponents : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//Design test component
		ShipComponents c = new ShipComponents();
		c.name = "Heavy Test";
		c.Mass = 500;
		c.CrewRequired = 200;
		c.SetHTK (3);

		ShipComponents b = new ShipComponents();
		b.name = "Light Test";
		b.Mass = 250;
		b.CrewRequired = 50;
		b.SetHTK (2);

		ShipComponents bridge = new ShipComponents();
		bridge.name = "Bridge";
		bridge.Mass = 50;
		bridge.CrewRequired = 15;
		bridge.control = true;
		bridge.Category = CompCategory.REQUIRED;

		ShipComponents flagBridge = new ShipComponents();
		flagBridge.name = "Flag Bridge";
		flagBridge.Mass = 250;
		flagBridge.CrewRequired = 100;
		flagBridge.control = true;
		flagBridge.flagControl = true;
		flagBridge.Category = CompCategory.REQUIRED;

		ShipComponents e = new ShipComponents ();
		e.name = "Engine E";
		e.Thrust = 50;
		e.Mass = 50;
		e.CrewRequired = 25;
		e.isEngine = true;
		e.Category = CompCategory.ENGINE;

		ShipComponents f = new ShipComponents ();
		f.name = "Engine F";
		f.Thrust = 150;
		f.Mass = 150;
		f.CrewRequired = 80;
		f.isEngine = true;
		f.Category = CompCategory.ENGINE;

		ShipComponents turnThruster = new ShipComponents ();
		turnThruster.name = "Manuevering Thruster";
		turnThruster.TurnThrust = 25;
		turnThruster.Mass = 25;
		turnThruster.CrewRequired = 10;
		turnThruster.isEngine = true;
		turnThruster.Category = CompCategory.DEFAULT;

		ShipComponents berths = new ShipComponents();
		berths.name = "Crew Quarters";
		berths.Mass = 100;
		berths.quarters = 250;
		berths.Category = CompCategory.REQUIRED;

		ShipComponents smBerths = new ShipComponents();
		smBerths.name = "Crew Quarters";
		smBerths.Mass = 10;
		smBerths.quarters = 25;
		smBerths.Category = CompCategory.REQUIRED;

		ShipComponents lifeSupport = new ShipComponents();
		lifeSupport.name = "Life Support";
		lifeSupport.Mass = 100;
		lifeSupport.lifeSupport = 500;
		lifeSupport.Category = CompCategory.REQUIRED;

		ShipComponents smLifeSupport = new ShipComponents();
		smLifeSupport.name = "Small Life Support";
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


		ShipComponents.DesignedComponents.Add (f);
		ShipComponents.DesignedComponents.Add (e);
		ShipComponents.DesignedComponents.Add (c);
		ShipComponents.DesignedComponents.Add (b);

		foreach(ShipComponents ce in ShipComponents.DesignedComponents){
			ShipComponents.Save(ce);
		}
		ShipComponents.DesignedComponents.Clear ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
