using Microsoft.Xna.Framework;
using Tilteroids.Core.Graphics;

namespace Tilteroids.Core.Debugging;

public class OrientationDisplay
{
	// Required Params
	public Vector2 Position { get; set; }
	public float Radius { get; set; }

	// Optional Params
	public Color BackgroundColor { get; set; } = new Color(50, 50, 50);
	public Color XColor { get; set; } = Color.Magenta;
	public Color YColor { get; set; } = Color.Lime;
	public Color ZColor { get; set; } = Color.Cyan;

	public Color CalibrationXColor { get; set; } = Color.Red;
	public Color CalibrationYColor { get; set; } = Color.Green;
	public Color CalibrationZColor { get; set; } = Color.Blue;

	public OrientationDisplay(Vector2 position, float radius)
	{
		Position = position;
		Radius = radius;
	}

	public void Draw(Matrix currentOrientation, Matrix calibrationOrientation)
	{
		// NOTE: Phone is intended to be landscape, User is looking at screen.
		// X is towards right side of phone, Y is towards top of phone, Z is "Up" (towards the user's face)
		// In screen space, X is "right", and Y is "down".

		// Draw Backing Circle
		Primitives.DrawCircle(Position, Radius, BackgroundColor, 0.98f);

		Vector3 xCal = calibrationOrientation.Right;
		Vector3 yCal = calibrationOrientation.Up;
		Vector3 zCal = calibrationOrientation.Backward;

		DrawTargetCircle(new(-xCal.Y, -xCal.X), xCal.Z >= 0, Radius / 10, CalibrationXColor);
		DrawTargetCircle(new(-yCal.Y, -yCal.X), yCal.Z >= 0, Radius / 10, CalibrationYColor);
		DrawTargetCircle(new(-zCal.Y, -zCal.X), zCal.Z >= 0, Radius / 10, CalibrationZColor);

		Vector3 x = currentOrientation.Right;
		Vector3 y = currentOrientation.Up;
		Vector3 z = currentOrientation.Backward;

		DrawTargetCircle(new(-x.Y, -x.X), x.Z >= 0, Radius / 8, XColor);
		DrawTargetCircle(new(-y.Y, -y.X), y.Z >= 0, Radius / 8, YColor);
		DrawTargetCircle(new(-z.Y, -z.X), z.Z >= 0, Radius / 8, ZColor);
	}
	private void DrawTargetCircle(Vector2 offset, bool filled, float radius, Color color)
	{
		if (filled)
			Primitives.DrawCircle(Position + offset * Radius, radius, color, 1.0f);
		else
			Primitives.DrawCircleOutline(Position + offset * Radius, radius, color, 1.0f);
	}
}