using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum ArmyUnitTypes{
	Militia =2,
	Infantry =3,
	Marine =4,
	Armored = 5

}

public class ArmyUnit{
	public string UnitName;
	public float xp;
	public float morale;
	public ArmyUnitTypes UnitType;
	float strength; //base
	float manpower;
	float mManpower;
	bool deployed; //when deployed in a fight


	public float LastBattleCasualties;
}

public class Army : MonoBehaviour {
	public string ArmyName;
	public List<ArmyUnit> Units;
	public Colony origin; //where it was formed.

}

public class Battle : MonoBehaviour{
	public static Dictionary<Army,Battle> Dict = new Dictionary<Army, Battle> ();

	public static void AddBattle(Battle b){
		Dict.Add (b.Attacker, b);
		Dict.Add (b.Defender, b);
	}

	public static void RemoveBattle(Battle b){
		Dict.Remove (b.Attacker);
		Dict.Remove (b.Defender);
	}

	static float PhaseLength = 20f; //in seconds

	public Army Attacker;
	public Army Defender;

	int counterA = 0;
	int counterD = 0;

	public int ADeaths;
	public int DDeaths;

	public void Start(){
		Attacker.ArmyName = "Attacker";
		Defender.ArmyName = "Defender";
		AddBattle (this);
		foreach (ArmyUnit a in Attacker.Units) {
			if (counterD < Defender.Units.Count) {
				StartCoroutine ("Conflict", a, Defender.Units[counterD]);
			}
		}
	}


	public void Update(){
		bool surrender = true;
		foreach (ArmyUnit a in Attack) {
			if (a.morale > .25f)
				surrender = false;
		}
		if (surrender)
			EndBattle (Defender);
		surrender = true;
		foreach (ArmyUnit a in Defender) {
			if (a.morale > .25f)
				surrender = false;
		}
		if (surrender)
			EndBattle (Attacker);
	}

	void EndBattle(Army victor){
		Debug.Log (victor.ArmyName + " is victor!");
		RemoveBattle (this);
		Destroy (gameObject);
	}

	void Attack(ArmyUnit a, ArmyUnit d){
		if (Random.Range (0, (int)a.UnitType) > 0) {
			d.morale -= .1;
			//TODO
		}
	}

	IEnumerator Conflict(ArmyUnit a, ArmyUnit d){
		bool done = false;
		int aLoss;
		int dLoss;
		bool AttackerAttacks = true;
		float timer=1f;
		while (a.morale > .25f && d.morale > .25f) {
			timer += Time.deltaTime;
			if (timer > 1f) {
				if (AttackerAttacks) {
					Attack (a, d);
				} else {
					Attack (d, a);
				}
				Time = 0f;
			}
			yield return null;
		}
		if (!d.morale > .25f) {
			Debug.Log ("Attacker wins conflict!");
		} else {
			Debug.Log ("Defender wins conflict!");
		}
		done = true;

	}

}