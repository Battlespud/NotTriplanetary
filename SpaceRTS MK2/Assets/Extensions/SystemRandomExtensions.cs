using System.Collections;
using System.Collections.Generic;
using System;

public static class SystemRandomExtensions {

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

	public static void AddExclusive<T>(this List<T> list, T item){
		if (!list.Contains (item))
			list.Add (item);
	}

	static Random RNG = new Random();

	public static T RandomEnum<T>()
	{  
		Type type = typeof(T);
		Array values = Enum.GetValues(type);
		lock(RNG)
		{
			object value= values.GetValue(RNG.Next(values.Length));
			return (T)Convert.ChangeType(value, type);
		}
	}
}
