using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Graphics;

namespace Tilteroids.Core.Debugging;

public class OrientationDisplay
{
	private Matrix orientationMatrix = Matrix.Identity;

	public Vector2 Position { get; set; }
	public float Radius { get; set; }
	public Color BackgroundColor { get; set; } = new Color(50, 50, 50);

	public OrientationDisplay(Vector2 position, float radius, Color? backgroundColor = null)
	{
		Position = position;
		Radius = radius;
		BackgroundColor = backgroundColor ?? new Color(50, 50, 50);
	}

	public void Update(Vector3 accelerometerVector, Vector3 magnetometerVector)
	{
		PMath.GetOrientation(accelerometerVector, magnetometerVector, out orientationMatrix);
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		DrawCircle();
	}

	private void DrawCircle()
	{
		// NOTE: Phone is intended to be landscape, User is looking at screen.
		// X is towards right side of phone, Y is towards top of phone, Z is "Up" (towards the user's face)
		// In screen space, X is "right", and Y is "down".

		// Draw Backing Circle
		Primitives.DrawCircle(Position, Radius, BackgroundColor, 0.98f);

		Vector3 x = orientationMatrix.Right;
		Vector3 y = orientationMatrix.Up;
		Vector3 z = orientationMatrix.Backward;

		Vector2 xScreen = new(-x.Y, -x.X);
		Vector2 yScreen = new(-y.Y, -y.X);
		Vector2 zScreen = new(-z.Y, -z.X);

		DrawTargetCircle(xScreen, x.Z > 0, Radius / 8, Color.Magenta);
		DrawTargetCircle(yScreen, y.Z > 0, Radius / 8, Color.Lime);
		DrawTargetCircle(zScreen, z.Z > 0, Radius / 8, Color.Cyan);
	}
	private void DrawTargetCircle(Vector2 offset, bool filled, float radius, Color color)
	{
		if (filled)
			Primitives.DrawCircle(Position + offset * Radius, radius, color, 1.0f);
		else
			Primitives.DrawCircleOutline(Position + offset * Radius, radius, color, 1.0f);
	}
}