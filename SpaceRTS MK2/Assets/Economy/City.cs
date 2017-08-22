using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class City : MonoBehaviour {

	public Text rSummary;
	public Text pSummary;

	public bool MakeARequestBitch = false;

	GameObject controller;

	public Dictionary<RawResources, RawResource> ResourceStockpile = new Dictionary<RawResources, RawResource> ();

	public List<ResourceRequest> Requests = new List<ResourceRequest>();

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

	// Use this for initialization
	void Start () {
		Name = "city";
		Collections.Cities.Add (this);
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
		if (MakeARequestBitch) {
			MakeARequestBitch = false;
			GenerateRequest (RawResources.METAL, 5);
		}
	}

	void GenerateRequest(RawResources r, int am){
		ResourceRequest req = new ResourceRequest (r,am);
		req.patron = this;
		List<Freighter> temp = new List<Freighter> ();
		temp.AddRange (Collections.Available);
		temp.OrderBy(
			targ => Vector3.Distance(this.transform.position,targ.transform.position)).ToList();
		temp [0].AssignMission (req);
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
		//	Debug.Log (p.product.ToString () + " " + p.GetAmount());
		}
		UpdateText ();
	}

}
