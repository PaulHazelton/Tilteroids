using SpaceshipArcade.MG.Engine.Utilities;

namespace Tilteroids.Core.Gameplay.Torus;

public interface IWrappable
{
	float Radius { get; }
	Vector2 WorldCenter { get; set; }
}

public static class WrappableExtensions
{
	private const float Epsilon = 0.001f;

	public static void Wrap(this IWrappable w, RectangleF bounds)
	{
		// Right
		var xDiff = w.WorldCenter.X - w.Radius - bounds.Right;
		if (xDiff > Epsilon)
			w.WorldCenter = new(bounds.Left - w.Radius + xDiff, w.WorldCenter.Y);

		// Left
		var xDiffNeg = w.WorldCenter.X + w.Radius - bounds.Left;
		if (xDiffNeg < -Epsilon)
			w.WorldCenter = new(bounds.Right + w.Radius + xDiffNeg, w.WorldCenter.Y);

		// Bottom
		var yDiff = w.WorldCenter.Y - w.Radius - bounds.Bottom;
		if (yDiff > Epsilon)
			w.WorldCenter = new(w.WorldCenter.X, bounds.Top - w.Radius + yDiff);

		// Top
		var yDiffNeg = w.WorldCenter.Y + w.Radius - bounds.Top;
		if (yDiffNeg < -Epsilon)
			w.WorldCenter = new(w.WorldCenter.X, bounds.Bottom + w.Radius + yDiffNeg);
	}
}