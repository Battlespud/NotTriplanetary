using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public enum Roles{
	Scientist,
	Leader,
	General,
	Captain,
	Admiral
}

public enum Sex{
	Female,
	Male
}

public struct Trait{
	public static List<Trait>Traits = new List<Trait>();
	public string Name;
	public string Description;
	public Trait(string s, string d){
		Name = s;
		Description = d;
		Traits.Add (this);
	}
	public static void Load(){
		//Load traits
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Traits/" ); 
		string[] traits = File.ReadAllLines(path+"Traits.txt");
		string[] descr=File.ReadAllLines(path+"Descriptions.txt");
		int numTraits =0;
		for (int i = 0; i < traits.Length; i++) {
			Trait t = new Trait (traits [i], descr [i]);
			numTraits = i + 1;
		}
		Debug.Log("Loaded " + numTraits + " traits.");
	}
}

public class Medal{
	public static Dictionary<int, Sprite> MedalImages = new Dictionary<int, Sprite> ();
	public string Name;
	public string Description;
	public int Points = 50;
	public string DateDesigned;
	public int ImageIndex;
	public static List<Medal> DesignedMedals = new List<Medal> ();

	public Medal(string na,string desc,int p, int i ){
		Name = na;
		Description = desc;
		Points = p;
		DateDesigned = ClockStatic.clock.GetDate ();
		ImageIndex = i;
		DesignedMedals.Add (this);
	}
	public Sprite GetImage(){
		return MedalImages [ImageIndex];
	}

	static Medal(){
		Medal Pioneer = new Medal ("Pioneer's Medallion", "An award commemorating this officer's role in early exploration efforts on the stellar frontier.", 50, 0);
	
	}
}

public class Character {
	static Clock clock = ClockStatic.clock;

	static int NextID = -1;
	public int GetNextID(){
		NextID++;
		return NextID;

	}
	public int ID;
	public static List<string>NavalRankNames = new List<string>(){"Lieutenant Commander","Commander","Captain","List Captain","Commodore", "Rear Admiral", "Admiral", "Admiral of the Fleet"};
	public int PromotionPoints;
	public List<Medal> Medals = new List<Medal>();
	public List<Trait> Traits = new List<Trait> ();

	public string CharName;
	public int Age;
	public Sex sex;
	public Ship shipPosting;
	public Empire empire;
	public int Rank;

	StringBuilder sb;

	public string History;

	public string GetNameString(){
		return string.Format ("{0} {1}", NavalRankNames [Rank], CharName);
	}

	public void AddHistory(string s){
		sb = new StringBuilder ();
		sb.Append(History);
		sb.AppendLine (s);
		History = sb.ToString ();
	}


	public void JoinsUp(){
		string st = string.Format("{0}: {1} enlists at the rank of {2}.",clock.GetDate(), CharName, NavalRankNames[Rank]);
		AddHistory (st);
	}

	public void NewCommand(ShipClass s){
		s.Captain = this;
		s.CaptainString = GetNameString ();
		empire.Unassigned.Remove (this);
		shipPosting = s.ship;
		string st = string.Format("{0}: {1} is placed in command of the {2}.",clock.GetDate(),GetNameString(), s.ShipName);
		AddHistory (st);
	}

	public void AwardMedal(Medal m){
		Medals.Add (m);
		PromotionPoints += m.Points;
		string st = string.Format("{0}: {1} is awarded the {2}.",clock.GetDate(),GetNameString(), m.Name);
		AddHistory (st);
	}

	public void AwardMedal(Medal m, string reason){
		Medals.Add (m);
		PromotionPoints += m.Points;
		string st = string.Format("{0}: {1} is awarded the {2} for '{3}'.",clock.GetDate(),GetNameString(), m.Name, reason);
		AddHistory (st);
	}

	public void Promote(){
		if (Rank == NavalRankNames.Count - 1)
			return;
		string st = string.Format("{0}: {1} is promoted to the rank of {2}.",clock.GetDate(),GetNameString(), NavalRankNames[Rank+1]);
		AddHistory (st);
		Rank++;
	}

	public void AddTrait(Trait t){
		Traits.Add (t);
	}

	public void Die(){
		string st = string.Format("{0}: {1} was killed in the destruction of the {2}.",clock.GetDate(),GetNameString(), shipPosting.ShipName);
		AddHistory (st);
		shipPosting = null;
		empire.Characters.Remove (this);
		empire.Unassigned.Remove (this);
		empire.Dead.Add (this);
		OutputDeath ();
	}
		
	public void Output(){
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Characters/" + CharName + ".txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			writer.WriteLine ("Name: " + CharName + "\t Age: " + Age);
			if(shipPosting != null)
			writer.WriteLine ("Posting: " + shipPosting.ShipName);
			writer.Write (History + "\n\nAwards:\n");
			foreach (Medal m in Medals) {
				writer.WriteLine (m.Name);
			}
			writer.Write ("\n\nTraits:\n");
			foreach (Trait m in Traits) {
				writer.WriteLine (m.Name);
			}
				}
	}
	public void OutputDeath(){
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Characters/Fallen" + CharName + ".txt"); 
		using (StreamWriter writer = new StreamWriter (path)) {
			writer.WriteLine ("Name: " + CharName + "\t Age: " + Age);
			if(shipPosting != null)
				writer.WriteLine ("Posting: " + shipPosting.ShipName);
			writer.Write (History + "\n\nAwards:\n");
			foreach (Medal m in Medals) {
				writer.WriteLine (m.Name);
			}
			writer.Write ("\n\nTraits:\n");
			foreach (Trait m in Traits) {
				writer.WriteLine (m.Name);
			}
		}
	}

	// Use this for initialization
	public Character(){
		CharName = NameManager.GenerateCharName (); //todo
		Rank = 0;
		ID = GetNextID();
		sex = (Sex)Random.Range (0, 2);
		JoinsUp ();
	}

	public Character(int i){
		CharName = NameManager.GenerateCharName (); //todo
		Rank = i;
		ID = GetNextID();
		sex = (Sex)Random.Range (0, 2);
		JoinsUp ();
	}
}
