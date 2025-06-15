using Microsoft.Xna.Framework;
using SpaceshipArcade.MG.Engine.Extensions;
using SpaceshipArcade.MG.Engine.Graphics;
using SpaceshipArcade.MG.Engine.Utilities;

namespace SpaceshipArcade.MG.Engine.Debugging;

public class Vector3BarDisplay(Rectangle barDestinationRectangle)
{
	// Required Params
	public Rectangle BarDestinationRectangle { get; set; } = barDestinationRectangle;

	// Optional Params
	public int Padding { get; set; } = 10;
	public Color XColor { get; set; } = Color.Magenta;
	public Color YColor { get; set; } = Color.Lime;
	public Color ZColor { get; set; } = Color.Cyan;
	public Color TickColor { get; set; } = Color.Yellow;

	public void Draw(Vector3 currentVector, Vector3 calibrationVector)
	{
		currentVector.Normalize();
		calibrationVector.Normalize();

		if (currentVector.IsInvalid())
			currentVector = Vector3.Zero;

		if (calibrationVector.IsInvalid())
			calibrationVector = Vector3.Zero;

		// XYZ Bars
		var (x, y, width, height) = (BarDestinationRectangle.X, BarDestinationRectangle.Y, BarDestinationRectangle.Width, BarDestinationRectangle.Height);

		Primitives.DrawRectangleOutline(new Rectangle(x, y, width, height), XColor, 2.0f, 1.0f);
		Primitives.DrawRectangle(Bar(currentVector.X, x, y, width, height, Padding), XColor);
		Primitives.DrawRectangle(Tick(calibrationVector.X, x, y, width, height, Padding, 2), TickColor);

		y += height * 2;
		Primitives.DrawRectangleOutline(new Rectangle(x, y, width, height), YColor, 2.0f, 1.0f);
		Primitives.DrawRectangle(Bar(currentVector.Y, x, y, width, height, Padding), YColor);
		Primitives.DrawRectangle(Tick(calibrationVector.Y, x, y, width, height, Padding, 2), TickColor);

		y += height * 2;
		Primitives.DrawRectangleOutline(new Rectangle(x, y, width, height), ZColor, 2.0f, 1.0f);
		Primitives.DrawRectangle(Bar(currentVector.Z, x, y, width, height, Padding), ZColor);
		Primitives.DrawRectangle(Tick(calibrationVector.Z, x, y, width, height, Padding, 2), TickColor);

		static Rectangle Bar(float value, int x, int y, int width, int height, int padding)
		{
			int halfWidth = width / 2 - padding;
			int barWidth = (int)PMath.MapClamp(Math.Abs(value), 0, 1, 0, halfWidth);
			int centerX = x + width / 2;
			int barX = centerX - (value < 0 ? barWidth : 0);
			return new Rectangle(barX, y + padding, barWidth, height - 2 * padding);
		}
		static Rectangle Tick(float value, int x, int y, int width, int height, int padding, int thickness)
		{
			int halfWidth = width / 2 - padding;
			int tickX = width / 2 + (int)PMath.MapClamp(value, -1, 1, x - halfWidth, x + halfWidth);
			return new Rectangle(tickX - thickness / 2, y + padding, thickness, height - padding * 2);
		}
	}
}