using Microsoft.Xna.Framework;
using SpaceshipArcade.MG.Engine.Extensions;
using Tilteroids.Core.Graphics;

namespace Tilteroids.Core.Debugging;

public class Vector3CircleDisplay(Vector2 position, float radius)
{
	// Required Params
	public Vector2 Position { get; set; } = position;
	public float Radius { get; set; } = radius;

	// Optional Params
	public Color BackgroundColor { get; set; } = new Color(50, 50, 50);
	public Color IndicatorColor { get; set; } = Color.Yellow;
	public Color CalibrationIndicatorColor { get; set; } = Color.Cyan;

	public void Draw(Vector3 currentVector, Vector3 calibrationVector)
	{
		currentVector.Normalize();
		calibrationVector.Normalize();

		if (currentVector.IsInvalid())
			currentVector = Vector3.Zero;

		if (calibrationVector.IsInvalid())
			calibrationVector = Vector3.Zero;

		// NOTE: Phone is intended to be landscape, User is looking at screen.
		// X is "North", Y is "West", Z is "Up" (towards the user's face)
		// In screen space, X is "East", and Y is "South".

		Vector2 currentTarget = new(-currentVector.Y, -currentVector.X);
		Vector2 calibrationTarget = new(-calibrationVector.Y, -calibrationVector.X);

		// Draw Backing Circle
		Primitives.DrawCircle(Position, Radius, BackgroundColor, 0.98f);

		// Draw Calibration Target Circle
		if (calibrationVector.Z >= 0)
			Primitives.DrawCircle(Position + calibrationTarget * Radius, Radius / 8, CalibrationIndicatorColor, 0.99f);
		else
			Primitives.DrawCircleOutline(Position + calibrationTarget * Radius, Radius / 8, CalibrationIndicatorColor, 0.99f);

		// Draw Target Circle
		if (currentVector.Z >= 0)
			Primitives.DrawCircle(Position + currentTarget * Radius, Radius / 6, IndicatorColor, 1.0f);
		else
			Primitives.DrawCircleOutline(Position + currentTarget * Radius, Radius / 6, IndicatorColor, 1.0f);
	}
}