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

	public OrientationDisplay(Vector2 position, float radius)
	{
		Position = position;
		Radius = radius;
	}

	public void Draw(Matrix currentOrientation)
	{
		// NOTE: Phone is intended to be landscape, User is looking at screen.
		// X is towards right side of phone, Y is towards top of phone, Z is "Up" (towards the user's face)
		// In screen space, X is "right", and Y is "down".

		Vector3 x = currentOrientation.Right;
		Vector3 y = currentOrientation.Up;
		Vector3 z = currentOrientation.Backward;

		// Draw Backing Circle
		Primitives.DrawCircle(Position, Radius, BackgroundColor, 0.98f);

		DrawTargetCircle(new(-x.Y, -x.X), x.Z > 0, Radius / 8, XColor);
		DrawTargetCircle(new(-y.Y, -y.X), y.Z > 0, Radius / 8, YColor);
		DrawTargetCircle(new(-z.Y, -z.X), z.Z > 0, Radius / 8, ZColor);
	}
	private void DrawTargetCircle(Vector2 offset, bool filled, float radius, Color color)
	{
		if (filled)
			Primitives.DrawCircle(Position + offset * Radius, radius, color, 1.0f);
		else
			Primitives.DrawCircleOutline(Position + offset * Radius, radius, color, 1.0f);
	}
}