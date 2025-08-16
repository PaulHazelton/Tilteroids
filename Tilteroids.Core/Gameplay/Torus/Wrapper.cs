using nkast.Aether.Physics2D.Dynamics;
using SpaceshipArcade.MG.Engine.Graphics;
using SpaceshipArcade.MG.Engine.Utilities;

namespace Tilteroids.Core.Gameplay.Torus;

public class Wrapper(float radius, RectangleF bounds, Body body)
{
	private const float Epsilon = 0.001f;

	public float Radius { get; set; } = radius;
	public RectangleF Bounds { get; set; } = bounds;
	public Body Body { get; set; } = body;

	public Vector2 WorldCenter
	{
		get => Body.WorldCenter;
		set
		{
			var offset = value - Body.WorldCenter;
			Body.Position += offset;
		}
	}

	public void Wrap()
	{
		// Right
		var xDiff = WorldCenter.X - Radius - Bounds.Right;
		if (xDiff > Epsilon)
			WorldCenter = new(Bounds.Left - Radius + xDiff, WorldCenter.Y);

		// Left
		var xDiffNeg = WorldCenter.X + Radius - Bounds.Left;
		if (xDiffNeg < -Epsilon)
			WorldCenter = new(Bounds.Right + Radius + xDiffNeg, WorldCenter.Y);

		// Bottom
		var yDiff = WorldCenter.Y - Radius - Bounds.Bottom;
		if (yDiff > Epsilon)
			WorldCenter = new(WorldCenter.X, Bounds.Top - Radius + yDiff);

		// Top
		var yDiffNeg = WorldCenter.Y + Radius - Bounds.Top;
		if (yDiffNeg < -Epsilon)
			WorldCenter = new(WorldCenter.X, Bounds.Bottom + Radius + yDiffNeg);
	}

	public void Draw()
	{
		// if (_handler.DebugSettings.HasFlag(DebugFlags.WorldWrapView))
		Primitives.DrawCircleOutline(WorldCenter, Radius, Color.Red, 1.0f);
	}
}