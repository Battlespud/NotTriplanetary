using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Traits modify a characters stats when they are added.  Any number can be added. Theres no protection against duplicates besides sheer numbers yet.
public struct Trait{
	public static List<Trait>Traits = new List<Trait>();
	static char delimiter = '+';
	public string Raw;
	public string Name;
	public string Description;
	public List<int> PersonalityModifiers;
	public string Summary;
	public Trait(string s, string d, string r, List<int> modifiers = null ){
		Name = s;
		Description = d;
		PersonalityModifiers = new List<int> ();
		PersonalityModifiers.AddRange (modifiers);
		string arraystring = "";
		foreach (int i in PersonalityModifiers) {
			arraystring += i.ToString() + ",";
		}
		Raw = r;
		Summary = string.Format ("{0}:(<color=navy>{1}</color>) {2}",Name,arraystring,Description);
		Traits.Add (this);

		//		Debug.Log (Summary);
	}
	public static void Load(){
		//Load traits
		Traits.Clear();
		string path = System.IO.Path.Combine (Application.streamingAssetsPath, "Traits/" ); 
		string[] traits = File.ReadAllLines(path+"Traits.txt");
		//string[] descr=File.ReadAllLines(path+"Descriptions.txt");
		int numTraits =0;

		for (int i = 0; i < traits.Length; i++) {
			List<int> mods = new List<int> ();
			List<string> cut = new List<string> ();
			cut.AddRange(traits [i].Split (delimiter));
			//Debug.Log (cut.Count);
			string traitName = cut [0];
			string modsString = cut [1];
			modsString = modsString.Replace ("(", "");
			modsString =modsString.Replace (")", "");
			modsString =modsString.Replace ("{", "");
			modsString =modsString.Replace ("}", "");
			modsString =modsString.Replace (" ", "");
			char modsDelim = ',';
			foreach (string s in modsString.Split (modsDelim)) {
				mods.Add (int.Parse (s));
			}
			string desc = cut [2];

			Trait t = new Trait (traitName, desc,traits[i], mods);
			numTraits = i + 1;
		}
		//		Debug.Log("Loaded " + numTraits + " traits.");
	}
}
