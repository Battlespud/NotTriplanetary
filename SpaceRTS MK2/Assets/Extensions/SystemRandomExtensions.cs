using System.Collections;
using System.Collections.Generic;

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
}
