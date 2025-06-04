using Microsoft.Xna.Framework;
using SpaceshipArcade.MG.Engine.Extensions;

namespace SpaceshipArcade.MG.Engine.Utilities;

public class PMath
{
	#region Scalers

	// NOTE! This was changed to have amount be the first param instead of the last
	public static float Map(float amount, float lowIn, float highIn, float lowOut, float highOut)
	{
		float range1 = highIn - lowIn;
		float x1 = amount - lowIn;

		x1 /= range1;

		float range2 = highOut - lowOut;
		float x2 = x1 * range2;

		return x2 + lowOut;
	}

	public static float MapClamp(float amount, float lowIn, float highIn, float lowOut, float highOut)
	{
		return MathHelper.Clamp(Map(amount, lowIn, highIn, lowOut, highOut), lowOut, highOut);
	}

	public static float AngleDifference(float a, float b)
	{
		a = MathHelper.WrapAngle(a);
		b = MathHelper.WrapAngle(b);

		float x = a - b;
		return x + ((x > MathHelper.Pi) ? -MathHelper.TwoPi : (x < -MathHelper.Pi) ? MathHelper.TwoPi : 0);
	}

	#endregion

	#region Vectors

	public static Vector2 Midpoint(Vector2 a, Vector2 b) => new((a.X + b.X) / 2, (a.Y + b.Y) / 2);

	public static (float R, float Theta) CartesianToPolar(Vector2 vector) => (MathHelper.Distance(vector.X, vector.Y), vector.Angle());

	public static Vector2 PolarToCartesian(float r, float theta)
	{
		float x = (float)(r * Math.Cos(theta));
		float y = (float)(r * Math.Sin(theta));

		return new Vector2(x, y);
	}

	#endregion
}