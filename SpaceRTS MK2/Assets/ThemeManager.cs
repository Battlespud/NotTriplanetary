﻿using System.Collections;
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

	public static bool Initialized = false;

	public bool DebugSuppressPortraitLoading = true;

	public void GenerateThemes(){
		List<string> paths = new List<string> ();
		paths.AddRange(Directory.GetDirectories (System.IO.Path.Combine (Application.streamingAssetsPath, "Themes")));
		Debug.Log (paths.Count + " Theme folders detected.");
		ThemeCount = paths.Count;
		foreach (string path in paths) {
			Theme t = new Theme (path);
		}
		foreach (Empire e in Empire.AllEmpires) {
			e.EmpireThemes.Add(ThemesDict["Alliance"],1f);
		}
		Initialized = true;
	}

	IEnumerator ImportPortraits(){
		List<string> paths = new List<string> ();
		string pathAppend = @"Images\CharacterSprites";
		yield return Ninja.JumpToUnity;
		paths.AddRange(Directory.GetDirectories (System.IO.Path.Combine (Application.streamingAssetsPath, pathAppend )));
		yield return Ninja.JumpBack;
	//	Debug.Log (paths.Count + " Parent Level Portrait folders detected.");
		int g = 0;
		foreach (string path in paths) {
			for (int i = 0; i < 2; i++)
			{
				string modPath = path + @"\" +((Sex)i).ToString()[0];
				List<string> splitPath = new List<string> ();
				splitPath.AddRange(modPath.Split ('\\'));
				string FieldName = string.Format ("{1}{0}",splitPath[splitPath.Count-2] , splitPath[splitPath.Count-1]);
				List<Sprite> reflected = (List<Sprite>)typeof(Character).GetField(FieldName).GetValue(null);
				string[] FilePaths = Directory.GetFiles (modPath);
					
				yield return Ninja.JumpToUnity;
				foreach (string p in FilePaths) {
					if (!p.Contains (".meta")) {
						StartCoroutine(LoadTexFromFile("file://"+p,reflected));
						yield return null;
						g++;
					}
				}
//				Debug.Log(string.Format("Loading {0} {1}",((Sex)i).ToString(),splitPath[splitPath.Count-2]));
				yield return Ninja.JumpBack;	
			}
			yield return Ninja.JumpToUnity;
			Debug.Log("Number of Sprites Loaded: " + g );
		}
	}

	//image dictionary via index, input when coroutine finishes and use initialization variable plus while loop to wait before loading;

	IEnumerator LoadTexFromFile(string filename, List<Sprite> reflected){
//		Debug.Log (filename);
		WWW laughsinchinese = new WWW (filename);
		yield return laughsinchinese;
		Texture2D preSprite = new Texture2D (4, 4, TextureFormat.DXT1, false);;
		laughsinchinese.LoadImageIntoTexture (preSprite);
		yield return null;
		Sprite s = Sprite.Create (preSprite, new Rect (0f, 0f, preSprite.width, preSprite.height), new Vector2 (.5f, .5f), 100f);
		reflected.Add (s);
	}

	#region CharacterNames
	public static void GenerateCharName(Character c, Theme t = null){
		if(t==null)
			t = Themes [rnd.Next (0, Themes.Count)];
		
		if (c.sex == Sex.Female) {
			try{
				c.FirstName = t.CharacterFirstNamesF [rnd.Next (0, t.CharacterFirstNamesF.Count )]; 
				c.LastName = t.CharacterLastNames [rnd.Next(0, t.CharacterLastNames.Count)];
			}
			catch{
				Debug.LogError ("Failed to Assign Name Properly.");
				c.FirstName = "Error";
				c.LastName = "Error";
				return;
			}
		} else {
			try{
				c.FirstName = t.CharacterFirstNamesM [rnd.Next (0, t.CharacterFirstNamesF.Count)];
				c.LastName = t.CharacterLastNames [rnd.Next (0, t.CharacterLastNames.Count)];
			}
			catch{
				Debug.LogError ("Failed to Assign Name Properly.");
				c.FirstName = "Error";
				c.LastName = "Error";
				return;
			}
		}

	}

	#endregion

	#region Naming
	public string GetName(StrategicShip sh, Theme the){
		return the.ShipNames [rnd.Next (0, the.ShipNames.Count)];
	}
	//TODO add companies,s pace stations, ground units, etc
	#endregion

	public static string StreamingPath;
	
	void Awake()
	{
		Species s = new Species("Poof");

		StreamingPath = Application.streamingAssetsPath;
		Debug.LogError("Streaming path loaded into string. Proceed.");
		if (Manager != null) {
			Debug.Log ("Error, multiple ThemeManagers present");
		}
		Trait.Load ();
		Manager = this;
		GenerateThemes ();
		if(!DebugSuppressPortraitLoading)
			ThreadNinjaMonoBehaviourExtensions.StartCoroutineAsync(this,ImportPortraits());
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