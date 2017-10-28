using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Empire : MonoBehaviour {

	//Essentially our version of a faction.  Multiple empires of the same FAC can exist, representing
	//political groups within the whole.   Each will have its own officer core, but can share everything else.
	static System.Random rnd = new System.Random();
	public FAC Faction;
	public string Name;

	public List<Character>Characters = new List<Character>();
	public List<Character> Unassigned = new List<Character>();
	public List<Character> Dead = new List<Character>();
	public List<Ship> Ships = new List<Ship> ();


	public void GenerateStartingOfficerCorps(int i){
		ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,GenerateCorps(i));
	}

	public void DistributeOfficers(){
		foreach (Ship s in Ships) {
			if (s.shipClass.Captain == null) {
				Unassigned [0].NewCommand (s.shipClass);
			}
		}
	}

	public static bool RandomTraits = true;
	IEnumerator GenerateCorps(int i){
		//Max rank starting  = 6
		int index = 0;
		List<float> Distribution = new List<float>(){.65f,.15f,.1f,.05f, .05f};
		foreach (float f in Distribution) {
			for (int d = (int)(i * f); d > 0; d--) {
				Character c = new Character(index);
				c.Age = (int)(rnd.Next (24, 29) + index*rnd.Next(1.65f,3f));
				Characters.Add (c);
				c.empire = this;
				c.AwardMedal(Medal.DesignedMedals[0]); //All starting characters recieve a pioneer medal to show their seniority.
				if (RandomTraits) {
					int b = rnd.Next (0, 4);
					for(b=b;b >= 0; b--){
						int ind = rnd.Next(0,Trait.Traits.Count);
						c.AddTrait (Trait.Traits [ind]);
					}
				}
				yield return Ninja.JumpToUnity;
				c.Output ();
				yield return Ninja.JumpBack;
			}
			index++;
		}
		Unassigned.AddRange (Characters);
		yield return Ninja.JumpToUnity;
	}

	public bool DistributeCaptains = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (DistributeCaptains) {
			DistributeOfficers ();
			DistributeCaptains = false;
		}
	}
}
