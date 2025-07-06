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
		if (w.WorldCenter.X - w.Radius > bounds.Right)
			w.WorldCenter = new(bounds.Left - w.Radius, w.WorldCenter.Y);
		if (w.WorldCenter.X + w.Radius < bounds.Left)
			w.WorldCenter = new(bounds.Right + w.Radius, w.WorldCenter.Y);

		if (w.WorldCenter.Y - w.Radius > bounds.Bottom)
			w.WorldCenter = new(w.WorldCenter.X, bounds.Top - w.Radius);
		if (w.WorldCenter.Y + w.Radius < bounds.Top)
			w.WorldCenter = new(w.WorldCenter.X, bounds.Bottom + w.Radius);
	}
}