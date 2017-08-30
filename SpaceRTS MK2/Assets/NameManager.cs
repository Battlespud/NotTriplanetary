using System.Collections.Generic;
//using UnityEngine;
using System.IO;
using System.Text;
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
public class NameManager// : MonoBehaviour
{
    static readonly string[] RomanNumerals = { "", "", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
    static string dir = Directory.GetCurrentDirectory() + "\\Text Files";
    static string file = dir + "\\Ship Names.txt";
 public   static List<string> names;
    static List<string> usedNames;
	static Dictionary <string,int> Frequency = new Dictionary<string,int>();
	public static Dictionary <ShipAbstract,string> usedNamesShip = new Dictionary<ShipAbstract, string>();

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

		foreach (string n in names) {
			if(!Frequency.ContainsKey(n))
			Frequency.Add (n, 0);
		}
    }

	static public string AssignName(ShipAbstract ship)
    {
		System.Random rand = new System.Random();
		int index = rand.Next (0, names.Count);
		string name = names[index];   //AssignNumeral(names[]) ?? names[rand.Next(0, names.Count)];
        names.Remove(name);
        usedNames.Add(name);
		Frequency [name]++;
		usedNamesShip.Add (ship, name);
		return ship.BaseType.ToString() + " " + name + AssignNumeral(name);
    }

	static void CheckDic(string name){
	}

	static public void RecycleName(ShipAbstract s)
    {
		string n = usedNamesShip[s];
		usedNames.Remove (n);
		names.Add (n);
	}

    static public string AssignNumeral(string name)
    {
        return   Frequency[name] == 0 ? null : " " + RomanNumerals[Frequency[name]];
    }
}
