using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class ThemeManager : MonoBehaviour{
	static System.Random rnd = new System.Random();

	public static ThemeManager Manager;

	public static List<Theme> Themes = new List<Theme>();
	public static Dictionary<string,Theme> ThemesDict = new Dictionary<string, Theme>();

	public int ThemeCount;

	public void GenerateThemes(){
		List<string> paths = new List<string> ();
		paths.AddRange(Directory.GetDirectories (System.IO.Path.Combine (Application.streamingAssetsPath, "Themes")));
		Debug.Log (paths.Count + " Theme folders detected.");
		ThemeCount = paths.Count;
		foreach (string path in paths) {
			Theme t = new Theme (path);
		}
	}

	public static string GenerateCharName(Theme t, Sex s){
		if (s == Sex.Female) {
			return t.CharacterFirstNamesF [rnd.Next (0, t.CharacterFirstNamesF.Count )] + " " + t.CharacterLastNames [rnd.Next(0, t.CharacterLastNames.Count)];
		} else {
			return t.CharacterFirstNamesM [rnd.Next (0, t.CharacterFirstNamesF.Count)] + " " + t.CharacterLastNames [rnd.Next (0, t.CharacterLastNames.Count)];
		}
	}

	public static string GenerateCharName(Sex s){
		Theme t = Themes [rnd.Next (0, Themes.Count)];
		Debug.Log (t.ThemeName);
		if (s == Sex.Female) {
			return t.CharacterFirstNamesF [rnd.Next (0, t.CharacterFirstNamesF.Count)] + " " + t.CharacterLastNames [rnd.Next (0, t.CharacterLastNames.Count)];
		} else {
			try{
			return t.CharacterFirstNamesM [rnd.Next (0, t.CharacterFirstNamesF.Count)] + " " + t.CharacterLastNames [rnd.Next (0, t.CharacterLastNames.Count)];
			}
			catch{
				return NameManager.GenerateCharName();
			}
		}
	}

	void Awake(){
		if (Manager != null) {
			Debug.Log ("Error, multiple thememanagers present");
		}
		Trait.Load ();
		Manager = this;
		GenerateThemes ();
	}

}


public class Theme{
	//contains ship, character and planet names based on an overarching theme.
	const string MaleNamesFileName = "MFirst.txt";
	const string FemaleNamesFileName = "FFirst.txt";
	const string LastNamesFileName = "Last.txt";
	const string ShipNamesFileNames = "Ship.txt";


	public string ThemeName;


	public List<string> CharacterFirstNamesM = new List<string> ();
	public List<string> CharacterFirstNamesF = new List<string> ();
	public List<string> CharacterLastNames = new List<string> ();
	public List<string> ShipNames = new List<string> ();

	public string Summary;

	public Theme(string path){
		ThemeName = new DirectoryInfo(path).Name;
		try{
		CharacterFirstNamesM.AddRange(File.ReadAllLines(System.IO.Path.Combine(path,MaleNamesFileName)));
		CharacterFirstNamesF.AddRange(File.ReadAllLines(System.IO.Path.Combine(path,FemaleNamesFileName)));
		CharacterLastNames.AddRange(File.ReadAllLines(System.IO.Path.Combine(path,LastNamesFileName)));
		ShipNames.AddRange(File.ReadAllLines(System.IO.Path.Combine(path,ShipNamesFileNames)));

		Summary = string.Format ("Theme: {0}    Female: {1}    Male: {2}    Last: {3}    Ship: {4}", ThemeName, CharacterFirstNamesF.Count, CharacterFirstNamesM.Count, CharacterLastNames.Count, ShipNames.Count);

		Debug.Log (Summary);
		ThemeManager.Themes.Add (this);
		ThemeManager.ThemesDict.Add (ThemeName, this);
		}
		catch{
			Debug.Log (string.Format ("{0} is missing files or is set up incorrectly. It will not be loaded",ThemeName));
		}
	}
}