namespace SpaceshipArcade.MG.Engine.Extensions;

public static class VectorExtensions
{
	/// <returns>The angle between the x axis and the vector. Always between -pi and pi.</returns>
	public static float Angle(this Vector2 v) => (float)Math.Atan2(v.Y, v.X);

	public static float Distance(this Vector2 v) => MathHelper.Distance(v.X, v.Y);

	public static bool IsZero(this Vector2 v) => v.X == 0f && v.Y == 0f;
	public static bool IsZero(this Vector3 v) => v.X == 0f && v.Y == 0f && v.Z == 0f;

	public static bool IsInvalid(this Vector2 v) =>
		float.IsNaN(v.X) || float.IsNaN(v.Y) ||
		float.IsInfinity(v.X) || float.IsInfinity(v.Y);
	public static bool IsInvalid(this Vector3 v) =>
		float.IsNaN(v.X) || float.IsNaN(v.Y) || float.IsNaN(v.Z) ||
		float.IsInfinity(v.X) || float.IsInfinity(v.Y) || float.IsInfinity(v.Z);
}