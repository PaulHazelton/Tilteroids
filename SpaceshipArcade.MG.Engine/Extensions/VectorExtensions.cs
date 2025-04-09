using Microsoft.Xna.Framework;

namespace SpaceshipArcade.MG.Engine.Extensions;

public static class VectorExtensions
{
	/// <returns>The angle between the x axis and the vector. Always between -pi and pi.</returns>
	public static float Angle(this Vector2 v) => (float)Math.Atan2(v.Y, v.X);

	public static float Distance(this Vector2 v) => MathHelper.Distance(v.X, v.Y);
}