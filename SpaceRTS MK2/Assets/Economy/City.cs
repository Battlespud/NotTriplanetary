using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class City : MonoBehaviour {

	public Text rSummary;
	public Text pSummary;

	GameObject controller;

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
		controller = GameObject.FindGameObjectWithTag ("Controller");
		controller.GetComponent<Clock> ().TurnEvent.AddListener (HandleEconomy);
		foreach(Factory f in GetComponentsInChildren<Factory>()){
			Factories.Add (f);
			f.city = this;
		}
	}
	

	public float processTimer;
	public float processInterval = 2f;


	void Update () {

	}

	void UpdateText(){
		rSummary.text = "Resources||\n";
		foreach (RawResource r in ResourceStockpile.Values) {
			rSummary.text += string.Format("{0}: {1} \n",r.resource.ToString(),r.GetAmount());
		}
		pSummary.text = "Products||\n";
		foreach (Product r in ProductStockpile.Values) {
			pSummary.text += string.Format("{0}: {1} \n",r.product.ToString(),r.GetAmount());
		}
	}

	void HandleEconomy(){
		foreach (Product p in ProductStockpile.Values) {
			Debug.Log (p.product.ToString () + " " + p.GetAmount());
		}
		UpdateText ();
	}

}
