using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
//public enum RomanNumeral
//{
//    I = 1,
//    II,
//    III,
//    IV,
//    V,
//    VI,
//    VII,
//    VIII,
//    IX,
//    X
//}






public static class NameManager
{	//TODO Use the streaming assets directory please, anything outside the asset management system wont be included in builds.

	static System.Random rand = new System.Random();

    static readonly string[] RomanNumerals = { "", "", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
    static string dir = Directory.GetCurrentDirectory() + "\\Text Files";
    static string file = dir + "\\Ship Names.txt";
 public   static List<string> names;
    static List<string> usedNames;
	static Dictionary <string,int> Frequency = new Dictionary<string,int>();
	public static Dictionary <ShipAbstract,string> usedNamesShip = new Dictionary<ShipAbstract, string>();
	public static Dictionary <StrategicShip,string> usedNamesStrategicShip = new Dictionary<StrategicShip, string>();

	//Characters

	static List<string> firstName = new List<string>();
	static string fnFile = dir + "\\FirstNames.txt";
	static List<string> lastName = new List<string>();
	static string lnFile = dir + "\\LastNames.txt";

    //private void Start()
    static NameManager()
    {
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        names = new List<string>();
        usedNames = new List<string>();
        List<string> temp = new List<string>();
        using (StreamReader sr = new StreamReader(File.OpenRead(file)))
        {
            do
                names.Add(sr.ReadLine());
            while (!sr.EndOfStream);
        }
		using (StreamReader sr = new StreamReader(File.OpenRead(fnFile)))
		{
			do
				firstName.Add(sr.ReadLine());
			while (!sr.EndOfStream);
		}
		using (StreamReader sr = new StreamReader(File.OpenRead(lnFile)))
		{
			do
				lastName.Add(sr.ReadLine());
			while (!sr.EndOfStream);
		}
		foreach (string n in names) {
			if(!Frequency.ContainsKey(n))
			Frequency.Add (n, 0);
		}
		Debug.Log(string.Format("{0} First Names Loaded. {1} Last Names Loaded.",firstName.Count,lastName.Count));
			
    }

	static public string AssignName(ShipAbstract ship)
    {
		int index = rand.Next (0, names.Count);
		string name = names[index];   //AssignNumeral(names[]) ?? names[rand.Next(0, names.Count)];
        names.Remove(name);
        usedNames.Add(name);
		Frequency [name]++;
		usedNamesShip.Add (ship, name);
		return ship.BaseType.ToString() + " " + name + AssignNumeral(name);
    }

	static public string AssignName(StrategicShip ship)
	{
		int index = rand.Next (0, names.Count);
		string name = names[index];   //AssignNumeral(names[]) ?? names[rand.Next(0, names.Count)];
		names.Remove(name);
		usedNames.Add(name);
		Frequency [name]++;
		usedNamesStrategicShip.Add (ship, name);
		return name + AssignNumeral(name);
	}

	static public string GenerateCharName(){
		return firstName [rand.Next(0, firstName.Count - 1)] + " " + lastName [rand.Next (0, lastName.Count - 1)];
	}

	static void CheckDic(string name){
	}

	static public void RecycleName(ShipAbstract s)
    {
		string n = usedNamesShip[s];
		usedNames.Remove (n);
		names.Add (n);
	}

	static public void RecycleName(StrategicShip s)
	{
		string n = usedNamesStrategicShip[s];
		usedNames.Remove (n);
		names.Add (n);
	}

    static public string AssignNumeral(string name)
    {
        return   Frequency[name] == 0 ? null : " " + RomanNumerals[Frequency[name]];
    }

	public static string GrabShipName(){
		int index = rand.Next (0, names.Count);
		string name = names[index]; 	
		return name;
	}
}
