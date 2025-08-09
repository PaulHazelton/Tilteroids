using SpaceshipArcade.MG.Engine.Utilities;

namespace Tilteroids.Core.Gameplay.Torus;

public interface IWrappable
{
	float Radius { get; }
	Vector2 WorldCenter { get; set; }
}

public static class WrappableExtensions
{
	public static void Wrap(this IWrappable w, RectangleF bounds)
	{
		// Right
		var xDiff = w.WorldCenter.X - w.Radius - bounds.Right;
		if (xDiff > 0.001f)
			w.WorldCenter = new(bounds.Left - w.Radius + xDiff, w.WorldCenter.Y);

		// Left
		var xDiffNeg = w.WorldCenter.X + w.Radius - bounds.Left;
		if (xDiffNeg < -0.001f)
			w.WorldCenter = new(bounds.Right + w.Radius + xDiffNeg, w.WorldCenter.Y);

		// Bottom
		var yDiff = w.WorldCenter.Y - w.Radius - bounds.Bottom;
		if (yDiff > 0.001f)
			w.WorldCenter = new(w.WorldCenter.X, bounds.Top - w.Radius + yDiff);

		// Top
		var yDiffNeg = w.WorldCenter.Y + w.Radius - bounds.Top;
		if (yDiffNeg < -0.001f)
			w.WorldCenter = new(w.WorldCenter.X, bounds.Bottom + w.Radius + yDiffNeg);
	}
}