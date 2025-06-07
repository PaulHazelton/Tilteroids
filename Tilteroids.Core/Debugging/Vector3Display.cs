using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Data;
using Tilteroids.Core.Graphics;

namespace Tilteroids.Core.Debugging;

public class Vector3Display
{
	// Private Readonly
	private readonly TextPanel _textPanel;
	private readonly Vector3 _targetVector = new(0, 0, 1);

	// Private State
	private Queue<Vector3> _samples;
	private Vector3 _currentVector = new();
	private Vector3 _calibrationVector = new();

	// private Matrix _transformationMatrix = Matrix.Identity;

	// Public Settings

	public Rectangle BarDestinationRectangle { get; set; }
	public Vector2 CirclePosition { get; set; }
	public float CircleRadius { get; set; }
	public Color CircleBackground { get; set; } = new Color(50, 50, 50);
	public Vector2 TextPanelPosition { get; set; }
	public int RollingAverageCount { get; init; }

	public Vector3Display(ContentBucket contentBucket, Rectangle barDestinationRectangle, Vector2 circlePos, float circleRadius, Vector2 textPanelPosition, int rollingAverageCount = 1)
	{
		_textPanel = new(contentBucket.Fonts.FallbackFont, textPanelPosition, TextPanel.AnchorCorner.TopLeft);
		BarDestinationRectangle = barDestinationRectangle;
		CirclePosition = circlePos;
		CircleRadius = circleRadius;
		TextPanelPosition = textPanelPosition;
		RollingAverageCount = rollingAverageCount;

		_samples = new(rollingAverageCount);

		for (int i = 0; i < RollingAverageCount; i++)
			_samples.Enqueue(Vector3.Backward);
	}

	public void Calibrate()
	{
		_calibrationVector = _currentVector;

		// _transformationMatrix = GetRotationMatrix(_calibrationVector, _targetVector);
	}

	public void Update(Vector3 value)
	{
		if (RollingAverageCount == 1)
			_currentVector = Vector3.Normalize(value);
		else
		{
			_samples.Dequeue();
			_samples.Enqueue(Vector3.Normalize(value));

			_currentVector = GetAverage(_samples);
		}

		// Text
		_textPanel.ClearLines();
		_textPanel.AddLine($"X: {_currentVector.X,6:F3}");
		_textPanel.AddLine($"Y: {_currentVector.Y,6:F3}");
		_textPanel.AddLine($"Z: {_currentVector.Z,6:F3}");
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		DrawBars();

		DrawCircle();

		_textPanel.Draw(spriteBatch);
	}

	private void DrawBars()
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
	}

	private void DrawCircle()
	{
		// NOTE: Phone is intended to be landscape, User is looking at screen.
		// X is "North", Y is "West", Z is "Up" (towards the user's face)
		// In screen space, X is "East", and Y is "South".

		// Aim Angle
		// var calibratedVector = Vector3.Normalize(_currentVector);
		// var currentVectorCalibrated = Vector3.Transform(Vector3.Normalize(_currentVector), _transformationMatrix);

		// See note above
		Vector2 calibrationTarget = new(-_calibrationVector.Y, -_calibrationVector.X);
		Vector2 currentTarget = new(-_currentVector.Y, -_currentVector.X);

		// Draw Backing Circle
		Primitives.DrawCircle(CirclePosition, CircleRadius, CircleBackground, 0.98f);

		// Draw Calibration Target Circle
		if (_calibrationVector.Z >= 0)
			Primitives.DrawCircle(CirclePosition + calibrationTarget * CircleRadius, CircleRadius / 8, Color.Cyan, 0.99f);
		else
			Primitives.DrawCircleOutline(CirclePosition + calibrationTarget * CircleRadius, CircleRadius / 8, Color.Cyan, 0.99f);

		// Draw Target Circle
		if (_currentVector.Z >= 0)
			Primitives.DrawCircle(CirclePosition + currentTarget * CircleRadius, CircleRadius / 6, Color.Yellow, 1.0f);
		else
			Primitives.DrawCircleOutline(CirclePosition + currentTarget * CircleRadius, CircleRadius / 6, Color.Yellow, 1.0f);

		// Old line
		// Primitives.DrawLine(CirclePosition, CirclePosition + (targetCircleCenterNormalized * CircleRadius * 0.9f), 4.0f, Color.Yellow, 1.0f);
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

	private static Vector3 GetAverage(Queue<Vector3> samples)
	{
		if (samples.Count == 0)
			return Vector3.Zero;
		
		return samples.Aggregate((sum, v) => sum + v) / samples.Count;
	}
}