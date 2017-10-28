using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fighter : MonoBehaviour, IPDTarget {

	public string Callsign;
	public int FormationNumber;
	public Ship Carrier;
	public ICAPTarget Target;
	public FAC Faction;
	public float Integrity = 1f;
	public int Crew = 1;
	public List<SpaceGun> Guns = new List<SpaceGun>();
	public float MaxRange = 5f;
	public float TorpedoReloadTimer = 5f;
	public int Torpedos = 6;
	public List<Renderer> Rens = new List<Renderer>();

	// Use this for initialization
	void Start () {
		if(Carrier != null)
		Faction = Carrier.faction;	
		RegenColors ();
	}

	public void RegenColors(){
		Color c = FactionMatrix.FactionColors [(int)Faction];
		foreach (Renderer r in Rens) {
			r.material.color = c;
			r.material.SetColor ("_EmissionColor", new Color (c.r, c.g, c.b, .35f));
			r.material.EnableKeyword ("_EMISSION");
		}
	}

	// Update is called once per frame
	void Update () {
		if (TorpedoReloadTimer > 0f)
			TorpedoReloadTimer -= Time.deltaTime;
		if (TargetLocked()) {
			Fire ();
		}
	}

	public void HitByPD(int dam){
		Integrity -= dam*Random.Range(0f,5f);
		OnChange();
	}

	void OnChange(){
		if (Crew <= 0 || Integrity <= 0) {
			Die ();
		}
	}

	void Die(){
		transform.position = new Vector3 (10f, 10000f, 0f);
		Destroy (gameObject);
	}

	public GameObject GetGameObject(){
		if(gameObject != null)
			return gameObject;
		return null;
	}

	public FAC GetFaction(){
		return Faction;
	}

	bool TargetLocked(){
		RaycastHit hit;
		Debug.DrawRay (transform.position, transform.forward * MaxRange, Color.cyan);
		if (Physics.Raycast (transform.position, transform.forward*MaxRange, out hit)) {
			Debug.DrawRay (transform.position, transform.forward * MaxRange, Color.red);
			if (hit.collider.GetComponent<ICAPTarget> () != null) {
				ICAPTarget i = hit.collider.GetComponent<ICAPTarget> ();
				if(i.isHostile(Faction))
					return true;
			}
		}
			return false;
	}

	bool CanFire(){
		return TorpedoReloadTimer <= 0f;
	}

	void Fire(){
		if (CanFire()) {
				if (Torpedos > 0) {
					Torpedos -= 1;
				TorpedoReloadTimer = 6f;
					GameObject t = Instantiate (Ship.Torpedo);
					t.transform.position = transform.position;
				t.GetComponent<Torpedo> ().faction = Faction;
				t.transform.rotation = transform.rotation;
			}
		}
	}

	public bool isHostile(FAC Caller){
		return FactionMatrix.IsHostile (Caller, Faction);
	}

}
