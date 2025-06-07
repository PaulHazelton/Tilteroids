using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Data;
using Tilteroids.Core.Graphics;

namespace Tilteroids.Core.Debugging;

public class Vector3Display(ContentBucket contentBucket, Rectangle barDestinationRectangle, Vector2 circlePos, float circleRadius, Vector2 textPanelPosition)
{
	// Private Readonly
	private readonly TextPanel _textPanel = new(contentBucket.Fonts.FallbackFont, textPanelPosition, TextPanel.AnchorCorner.TopLeft);
	private readonly Vector3 _targetVector = new(0, 0, 1); // Consider Removing

	// Private State
	private Vector3 _currentVector = new();
	private Vector3 _calibrationVector = new();
	private Matrix _transformationMatrix = Matrix.Identity;

	// Public Settings
	public Rectangle BarDestinationRectangle { get; set; } = barDestinationRectangle;
	public Vector2 CirclePosition { get; set; } = circlePos;
	public float CircleRadius { get; set; } = circleRadius;
	public Vector2 TextPanelPosition { get; set; } = textPanelPosition;

	public void Calibrate()
	{
		_calibrationVector = _currentVector;

		_transformationMatrix = GetRotationMatrix(_calibrationVector, _targetVector);
	}

	public void Update(Vector3 value)
	{
		_currentVector = value;

		// Text
		_textPanel.ClearLines();
		_textPanel.AddLine($"X: {_currentVector.X,6:F3}");
		_textPanel.AddLine($"Y: {_currentVector.Y,6:F3}");
		_textPanel.AddLine($"Z: {_currentVector.Z,6:F3}");
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		// XYZ Bars
		var (x, y, width, height) = (BarDestinationRectangle.X, BarDestinationRectangle.Y, BarDestinationRectangle.Width, BarDestinationRectangle.Height);

		Primitives.DrawRectangleOutline(new Rectangle(x, y, width, height), Color.Magenta, 2.0f, 1.0f);
		Primitives.DrawRectangle(Bar(_currentVector.X, x, y, width, height, 10), Color.Magenta);
		Primitives.DrawRectangle(Tick(_calibrationVector.X, x, y, width, height, 10, 2), Color.Yellow);

		y += height * 2;
		Primitives.DrawRectangleOutline(new Rectangle(x, y, width, height), Color.Lime, 2.0f, 1.0f);
		Primitives.DrawRectangle(Bar(_currentVector.Y, x, y, width, height, 10), Color.Lime);
		Primitives.DrawRectangle(Tick(_calibrationVector.Y, x, y, width, height, 10, 2), Color.Yellow);

		y += height * 2;
		Primitives.DrawRectangleOutline(new Rectangle(x, y, width, height), Color.Cyan, 2.0f, 1.0f);
		Primitives.DrawRectangle(Bar(_currentVector.Z, x, y, width, height, 10), Color.Cyan);
		Primitives.DrawRectangle(Tick(_calibrationVector.Z, x, y, width, height, 10, 2), Color.Yellow);

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

		// Aim Angle
		var calibratedVector = Vector3.Transform(_currentVector, _transformationMatrix);
		// Swap x and y because it's landscape
		Vector2 aimAngle = new(calibratedVector.Y, calibratedVector.X);

		float r = CircleRadius;
		// Vector2 circleCenter = new(800 + r, 450 - r);;
		Primitives.DrawCircle(CirclePosition, r, new Color(50, 50, 50), 0.9f);

		// aimAngle.Normalize();
		Primitives.DrawLine(CirclePosition, CirclePosition + (aimAngle * r * 0.9f), 4.0f, Color.Yellow, 1.0f);

		// Text Panel
		_textPanel.Draw(spriteBatch);
	}

	private static Matrix GetRotationMatrix(Vector3 n, Vector3 target)
	{
		float dot = Vector3.Dot(n, target);

		if (Math.Abs(dot - 1.0f) < 1e-6f)
			return Matrix.Identity;

		if (Math.Abs(dot + 1.0f) < 1e-6)
			return Matrix.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi);

		Vector3 axis = Vector3.Normalize(Vector3.Cross(n, target));
		float angle = (float)Math.Acos(dot);
		return Matrix.CreateFromAxisAngle(axis, angle);
	}
}