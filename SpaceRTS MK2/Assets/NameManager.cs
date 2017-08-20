﻿using System.Collections.Generic;
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
    static readonly string[] RomanNumerals = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X" };
    static string dir = Directory.GetCurrentDirectory() + "\\Text Files";
    static string file = dir + "\\Ship Names.txt";
    static List<string> names;
    static List<string> usedNames;
	static Dictionary <string,int> Frequency = new Dictionary<string,int>();

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
			Frequency.Add (n, 0);
		}
    }

    static public string AssignName()
    {
		
        System.Random rand = new System.Random();
        //string name = names[rand.Next(0, names.Count)];
        string name = AssignNumeral(names[rand.Next(0, names.Count)]) ?? names[rand.Next(0, names.Count)];
        names.Remove(name);
        usedNames.Add(name);
		CheckDic (name);
        return name;
    }

	static void CheckDic(string name){
		Frequency [name]++;
	}

	static public void RecycleName(string name)
    {
		usedNames.Remove (name);
		names.Add (name);
	}

    static public string AssignNumeral(string name)
    {
        return Frequency[name] == 0 ? null : name += " " + RomanNumerals[Frequency[name]];
    }
}
