using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NameManager : MonoBehaviour
{
    static string dir = Directory.GetCurrentDirectory() + "\\Text Files";
    static string file = dir + "\\Ship Names.txt";
    static List<string> names;
    static List<string> usedNames;

    private void Start()
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
    }

    static public string AssignName()
    {
        System.Random rand = new System.Random();
        string name = names.ToArray()[rand.Next(0, names.Count)];
        names.Remove(name);
        usedNames.Add(name);
        return name;
    }
}
