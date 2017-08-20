using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {

	public Dictionary<RawResources, RawResource> ResourceStockpile = new Dictionary<RawResources, RawResource> ();
	public bool UseResources(RawResources r, float amount){
		if (!ResourceStockpile.ContainsKey (r)) {
			ResourceStockpile.Add (r, new RawResource (r));
			return false;
		}
		return ResourceStockpile [r].Use (amount);
	}
	public Dictionary<Products, Product> ProductStockpile = new Dictionary<Products, Product> ();
	public void AddProduct(Products r, float amount){
		if (!ProductStockpile.ContainsKey (r)) {
			ProductStockpile.Add (r, new Product (r));
			Debug.Log (r.ToString () + " has been added");
		}
		ProductStockpile [r].Add(amount);
	}
		
	[SerializeField]

	public List<Factory> Factories = new List<Factory> ();

	string Name;
	//Factions Faction;

	//coroutine already running
	bool busy = false;


	// Use this for initialization
	void Start () {
		Name = "city";
	//	Faction = cmPOI.Faction;
		foreach(Factory f in GetComponentsInChildren<Factory>()){
			Factories.Add (f);
			f.city = this;
		}
	}
	

	public float processTimer;
	public float processInterval = 2f;
	void Update () {
		processTimer += Time.deltaTime;
		if (processTimer >= processInterval) {
			HandleEconomy();
			processTimer = 0f;
		}
	}

	void HandleEconomy(){
		foreach (Product p in ProductStockpile.Values) {
			Debug.Log (p.product.ToString () + " " + p.Amount);
		}
	}

}
