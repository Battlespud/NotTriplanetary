using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using UnityEngine.Events;

public class City : MonoBehaviour, IResources, IContext {

	public List<UnityAction> ContextActions(){
		return new List<UnityAction>{new UnityAction(ToggleUI), new UnityAction(RequestResources)};
	}

	public void RequestResources(){
		MakeARequestBitch = true;
	}

	public GameObject getGameObject(){
		return gameObject;
	}

	public Text rSummary;
	public Text pSummary;

	public GameObject UI;

	public bool MakeARequestBitch = false;

	GameObject controller;

	public RawResources neededTest;

	public Dictionary<RawResources, RawResource> ResourceStockpile = new Dictionary<RawResources, RawResource> ();

	public List<ResourceRequest> Requests = new List<ResourceRequest>();

	public bool TakeResource(RawResources r, float amount){
		if (!ResourceStockpile.ContainsKey (r)) {
			ResourceStockpile.Add (r, new RawResource (r));
			return false;
		}
		return ResourceStockpile [r].Use (amount);
	}

	public bool HasResource(RawResources r, float amount){
		if (!ResourceStockpile.ContainsKey (r)) {
			ResourceStockpile.Add (r, new RawResource (r));
			return false;
		}
		return ResourceStockpile [r].GetAmount() >= amount;
	}

	public bool GiveResource(RawResources r, float amount){
		if (!ResourceStockpile.ContainsKey (r)) {
			ResourceStockpile.Add (r, new RawResource (r));
			Debug.Log (r.ToString () + " has been added");
		}
		ResourceStockpile [r].Add(amount);
		UpdateText ();
		return true;
	}

	public Dictionary<Products, Product> ProductStockpile = new Dictionary<Products, Product> ();

	public void AddProduct(Products r, float amount){
		if (!ProductStockpile.ContainsKey (r)) {
			ProductStockpile.Add (r, new Product (r));
			Debug.Log (r.ToString () + " has been added");
		}
		ProductStockpile [r].Add(amount);
		UpdateText ();
	}
		
	[SerializeField]

	public List<Factory> Factories = new List<Factory> ();

	string Name;
	//Factions Faction;

	// Use this for initialization
	void Start () {
		Name = "city";
		Collections.Cities.Add (this);
		Collections.ResourceSources.Add (this);
		controller = GameObject.FindGameObjectWithTag ("Controller");
//		controller.GetComponent<Clock> ().TurnEvent.AddListener (UpdateTextVoid);
		UI.SetActive(false);
		foreach(Factory f in GetComponentsInChildren<Factory>()){
			Factories.Add (f);
			f.city = this;
		}
		for (int i = 1; i < 6; i++) {
			GiveResource ((RawResources)i,0f);
			AddProduct ((Products)i, 0f);
		}

	}
	

	public float processTimer;
	public float processInterval = 2f;


	void Update () {
		if (MakeARequestBitch) {
			MakeARequestBitch = false;
			GenerateRequest (neededTest, 5, false);
		}
	}

	public GameObject GetGameObject(){
		return gameObject;
	}

	void GenerateRequest(RawResources r, int am, bool b){
		ResourceRequest req = new ResourceRequest (r,am,b);
		req.patron = this;
		List<Freighter> temp = new List<Freighter> ();
		temp.AddRange (Collections.Available);
		temp.OrderBy(
			targ => Vector3.Distance(this.transform.position,targ.transform.position)).ToList();
		try{
		temp [0].AssignMission (req);
		}
		catch{
			Debug.Log ("No valid ships");
			return;
		}
	}

	public float ResourceAmount(RawResources r){
		if(ResourceStockpile.ContainsKey(r))
		{
			return ResourceStockpile[r].GetAmount();
		}
			return 0f;
	}

	void UpdateTextVoid(){
		StopAllCoroutines ();
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync (this, UpdateText ());
	}

	//async
	IEnumerator UpdateText(){
		StringBuilder sb = new StringBuilder();
		sb.AppendLine("Resources||");
		foreach (RawResource r in ResourceStockpile.Values)
		{
			sb.AppendLine(string.Format("{0}: {1}", r.resource.ToString(), r.GetAmount()));
		}
		sb.AppendLine("\nProducts||");
		foreach (Product r in ProductStockpile.Values)
		{
			sb.AppendLine(string.Format("{0}: {1}", r.product.ToString(), r.GetAmount()));
		}
		yield return Ninja.JumpToUnity;
		rSummary.text = sb.ToString();
	}

	public void ToggleUI(){
		UI.SetActive (!UI.active);
		if (UI.active) {
			controller.GetComponent<Clock> ().TurnEvent.AddListener (UpdateTextVoid);
			UpdateTextVoid ();
		} else {
			controller.GetComponent<Clock> ().TurnEvent.RemoveListener (UpdateTextVoid);
		}
	}

}
