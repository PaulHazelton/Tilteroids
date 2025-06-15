using Microsoft.Xna.Framework;
using SpaceshipArcade.MG.Engine.Graphics;

namespace SpaceshipArcade.MG.Engine.Debugging;

public class Vector2CircleDisplay(Vector2 position, float radius)
{
	// Required Params
	public Vector2 Position { get; set; } = position;
	public float Radius { get; set; } = radius;

	// Optional Params
	public float MaxMagnitude { get; set; } = 1.0f;
	public Color BackgroundColor { get; set; } = new Color(50, 50, 50);
	public Color IndicatorColor { get; set; } = Color.Yellow;

	public void Draw(Vector2 vector)
	{
		var magnitude = vector.Length();

		if (magnitude > MaxMagnitude)
			vector = vector / magnitude * MaxMagnitude;

		// Draw Backing Circle
		Primitives.DrawCircle(Position, Radius, BackgroundColor, 0.98f);

		// Draw Line
		Primitives.DrawLine(Position, Position + vector * Radius, 2.0f, IndicatorColor, 1.0f);
	}
}