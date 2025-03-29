using System;
using Microsoft.Xna.Framework;
using SpaceshipArcade.MG.Engine.Utilities;

namespace SpaceshipArcade.MG.Engine.Extensions;

public static class RandomExtensions
{

	public static bool NextBool(this Random r)
	{
		return r.NextSingle() < 0.5f;
	}

	/// <summary>
	/// Returns a random float that is within a specified range.
	/// </summary>
	/// <param name="min">The inclusive lower bound of the random number returned.</param>
	/// <param name="max">The exclusive upper bound of the random number returned.
	/// max must be greater than or equal to min.</param>
	/// <returns></returns>
	public static float NextSingle(this Random r, float min, float max)
	{
		return min + r.NextSingle() * (max - min);
	}

	// Source: https://stackoverflow.com/a/218600
	public static float NextNormalDistributionSingle(this Random r, float mean, float stdDev)
	{
		float u1 = 1.0f - r.NextSingle(); //uniform(0,1] random doubles
		float u2 = 1.0f - r.NextSingle();
		float randStdNormal = (float)(Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2)); //random normal(0,1)
		return mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
	}

	#region Vectors

	public static Vector2 NextNormalVector(this Random r)
		=> PMath.PolarToCartesian(1, r.NextSingle(0, (float)Math.Tau));

	public static Vector2 NextVector(this Random r, float rMin, float rMax)
		=> PMath.PolarToCartesian(r.NextSingle(rMin, rMax), r.NextSingle(0, (float)Math.Tau));

	#endregion
}