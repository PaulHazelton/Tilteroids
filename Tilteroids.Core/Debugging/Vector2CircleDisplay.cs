using Microsoft.Xna.Framework;
using Tilteroids.Core.Graphics;

namespace Tilteroids.Core.Debugging;

public class Vector2CircleDisplay
{
	// Required Params
	public Vector2 Position { get; set; }
	public float Radius { get; set; }

	// Optional Params
	public float MaxMagnitude { get; set; } = 1.0f;
	public Color BackgroundColor { get; set; } = new Color(50, 50, 50);
	public Color IndicatorColor { get; set; } = Color.Yellow;

	public Vector2CircleDisplay(Vector2 position, float radius)
	{
		Position = position;
		Radius = radius;
	}

	public void Draw(Vector2 vector)
	{
		var magnitude = vector.Length();

		if (magnitude > MaxMagnitude)
			vector = (vector / magnitude) * MaxMagnitude;

		// Draw Backing Circle
		Primitives.DrawCircle(Position, Radius, BackgroundColor, 0.98f);

		// Draw Line
		Primitives.DrawLine(Position, Position + (vector * Radius), 2.0f, IndicatorColor, 1.0f);
	}
}