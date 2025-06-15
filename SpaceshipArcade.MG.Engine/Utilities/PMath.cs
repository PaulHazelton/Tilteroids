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

	#region Matrices

	/// <summary>
	/// Calculates the orientation of the device based on the current accelerometer and compass readings
	/// </summary>
	/// <param name="accelerometerVector">The raw vector reading of the accelerometer. Expected Unit: g. Expected to be pointing up when the phone is at rest.</param>
	/// <param name="magnetometerVector">The raw vector reading of the magnetometer (compass). Expected Unit: Micro Tesla</param>
	/// <param name="result">The rotation matrix</param>
	/// <returns>True if success, false if failed.</returns>
	public static bool GetOrientation(Vector3 accelerometerVector, Vector3 magnetometerVector, out Matrix result)
	{
		float g = 1.0f; // Expected units are in gs

		result = Matrix.Identity;

		// If less than 0.1 gs (note, 0.1 is squared)
		if (accelerometerVector.LengthSquared() < 0.01f * g * g)
			return false;

		// If less than 5 micro T (note, 5 is squared) (Expected range is 25 to 65)
		if (magnetometerVector.LengthSquared() < 25.0f)
			return false;

		Vector3 east = Vector3.Normalize(Vector3.Cross(magnetometerVector, accelerometerVector));
		Vector3 north = Vector3.Normalize(Vector3.Cross(accelerometerVector, east));
		Vector3 up = Vector3.Normalize(accelerometerVector);

		result = new Matrix(new Vector4(east, 0), new Vector4(north, 0), new Vector4(up, 0), Vector4.UnitW);

		return true;
	}

	/// <summary>
	/// Old function that was used for the tilt controller that was based exclusively on accelerometer data.
	/// </summary>
	/// <param name="calibrationVector"></param>
	/// <param name="targetVector">The "neutral value" Defaults to (0, 0, 1) (out of the screen)</param>
	/// <returns>Returns a matrix that would return the calibration vector to neutral. Compare the current vector to the calibration vector.</returns> <summary>
	public static Matrix GetRotationMatrix(Vector3 calibrationVector, Vector3? targetVector = null)
	{
		targetVector ??= new(0, 0, 1);

		float dot = Vector3.Dot(calibrationVector, targetVector.Value);

		if (Math.Abs(dot - 1.0f) < 1e-6f)
			return Matrix.Identity;

		if (Math.Abs(dot + 1.0f) < 1e-6)
			return Matrix.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi);

		Vector3 axis = Vector3.Normalize(Vector3.Cross(calibrationVector, targetVector.Value));
		float angle = (float)Math.Acos(dot);
		return Matrix.CreateFromAxisAngle(axis, angle);
	}

	#endregion
}