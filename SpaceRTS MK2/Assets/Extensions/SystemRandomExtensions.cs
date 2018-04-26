using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using NUnit.Framework.Internal;
using UnityEngine.UI;

public static class SystemRandomExtensions {

	static System.Random RNG = new System.Random();
	
	//System.Random
	public static float Next(this System.Random random, float a, float b){
		return (float)(random.NextDouble () * (b - a) + a);
	}

	public static float NextFloat(this System.Random random, int a, int b){
		return (float)(random.NextDouble () * (b - a) + a);
	}

	public static float NextFloat(this System.Random random, float a, float b){
		return (float)(random.NextDouble () * (b - a) + a);
	}

	public static float NextFloat(this System.Random random, double a, double b){
		return (float)(random.NextDouble () * (b - a) + a);
	}
	
	public static bool NextBool(this System.Random random)
	{
		return random.Next(0, 2) == 0;
	}

	public static void Randomize(this int i, int ExclusiveMax)
	{
		i = RNG.Next(0, ExclusiveMax);
	}
	public static void Randomize(this float i, float ExclusiveMax)
	{
		i = RNG.NextFloat(0, ExclusiveMax);
	}
	public static void Randomize(this int i, int InclusiveMin, int ExclusiveMax)
	{
		i = RNG.Next(InclusiveMin, ExclusiveMax);
	}
	public static void Randomize(this float i, float InclusiveMin, float ExclusiveMax)
	{
		i = RNG.NextFloat(InclusiveMin, ExclusiveMax);
	}
	
	//Camera
	public static void ZoomTo(this Camera cam, Vector3 pos){
		Camera.main.gameObject.transform.position = new Vector3 (pos.x, 120f, pos.z - 50f);
	}
	public static void ZoomTo(this Camera cam, GameObject g)
	{
		Vector3 pos = g.transform.position;
		Camera.main.gameObject.transform.position = new Vector3 (pos.x, 120f, pos.z - 50f);
	}
	public static void ZoomTo(this Camera cam, MonoBehaviour g)
	{
		Vector3 pos = g.transform.position;
		Camera.main.gameObject.transform.position = new Vector3 (pos.x, 120f, pos.z - 50f);
	}
	
	//Dropdown
	public static void LoadEnum(this Dropdown drop, Type e)
	{
		List<string>Names = new List<string>();

		foreach (var v in Enum.GetValues(e))
		{
			Names.Add(v.ToString());
		}
		drop.ClearOptions();
		drop.AddOptions(Names);
	}

	//Collections
	public static void AddExclusive<T>(this List<T> list, T item){
		if (!list.Contains (item))
			list.Add (item);
	}

	public static T GetRandom<T>(this List<T> list)
	{
		return list[RNG.Next(0, list.Count)];
	}

	
	public static T RandomEnum<T>()
	{  
		Type type = typeof(T);
		{
			Array values = Enum.GetValues(type);
			lock (RNG)
			{
				object value = values.GetValue(RNG.Next(values.Length));
				return(T) Convert.ChangeType(value, type);
			}
		}
	}

	public static void RandomSex(this Sex sex)
	{
		sex = (Sex)RNG.Next(0, 2);
	}
	public static void RandomEitherSex(this Sex sex)
	{
		sex = (Sex)RNG.Next(0, 3);
	}
	/*
/// <summary>
/// Randomize an enum to another valid value.
/// </summary>
/// <param name="t"></param>
/// <typeparam name="T"></typeparam>
	public static void Randomize<T>(this T t)
	{
		Type type = typeof(T);
		if (type.IsEnum)
		{
			Array values = Enum.GetValues(type);
			lock (RNG)
			{
				object value = values.GetValue(RNG.Next(values.Length));
				t = (T) Convert.ChangeType(value, type);
			}
		}
	}
*/

	/*
	public static void Randomize<T>(this Enum enu)
	{
		Type type = typeof(T);
		List<T> EnumValues = new List<T>();
		EnumValues.Add(Enum.GetValues(type));
		object value= values.GetValue(RNG.Next(values.Length));
		enu = (type)value;
	}
	*/
}
