using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.Devices.Sensors;
using SpaceshipArcade.MG.Engine.Utilities;
using Tilteroids.Core.Data;
using Tilteroids.Core.Gameplay;
using Tilteroids.Core.Graphics;

namespace Tilteroids.Core.Debugging;

public class AccelerometerDisplay(ContentBucket contentBucket, Accelerometer accelerometer) : IGameObject
{
	private readonly Accelerometer _accelerometer = accelerometer;
	private readonly TextPanel _textPanel = new(contentBucket.Fonts.FallbackFont, position: new Vector2(200, 800), TextPanel.AnchorCorner.TopLeft);
	private readonly Vector3 _targetVector = new(0, 0, 1);

	private Vector3 _currentVector = new();
	private Vector3 _calibrationVector = new();
	private Matrix _transformationMatrix = Matrix.Identity;

	public void Calibrate()
	{
		_calibrationVector = _currentVector;

		_transformationMatrix = GetRotationMatrix(_calibrationVector, _targetVector);
	}

	private Matrix GetRotationMatrix(Vector3 n, Vector3 target)
	{
		float dot = Vector3.Dot(_calibrationVector, _targetVector);

		if (Math.Abs(dot - 1.0f) < 1e-6f)
			return Matrix.Identity;

		if (Math.Abs(dot + 1.0f) < 1e-6)
			return Matrix.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi);

		Vector3 axis = Vector3.Normalize(Vector3.Cross(_calibrationVector, _targetVector));
		float angle = (float)Math.Acos(dot);
		return Matrix.CreateFromAxisAngle(axis, angle);
	}

	public void Update(GameTime gameTime)
	{
		_currentVector = Vector3.Normalize(_accelerometer.CurrentValue.Acceleration);

		// Text
		_textPanel.ClearLines();
		_textPanel.AddLine($"Valid: {_accelerometer.IsDataValid}");
		_textPanel.AddLine($"X: {_currentVector.X}");
		_textPanel.AddLine($"Y: {_currentVector.Y}");
		_textPanel.AddLine($"Z: {_currentVector.Z}");
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		spriteBatch.Begin();

		// XYZ Bars
		Primitives.DrawRectangleOutline(new Rectangle(200, 200, 500, 100), Color.Magenta, 2.0f, 1.0f);
		Primitives.DrawRectangle(Bar(_currentVector.X, 200, 200, 500, 100, 10), Color.Magenta);
		Primitives.DrawRectangle(Tick(_calibrationVector.X, 200, 200, 500, 100, 10, 2), Color.Yellow);

		Primitives.DrawRectangleOutline(new Rectangle(200, 400, 500, 100), Color.Lime, 2.0f, 1.0f);
		Primitives.DrawRectangle(Bar(_currentVector.Y, 200, 400, 500, 100, 10), Color.Lime);
		Primitives.DrawRectangle(Tick(_calibrationVector.Y, 200, 400, 500, 100, 10, 2), Color.Yellow);

		Primitives.DrawRectangleOutline(new Rectangle(200, 600, 500, 100), Color.Cyan, 2.0f, 1.0f);
		Primitives.DrawRectangle(Bar(_currentVector.Z, 200, 600, 500, 100, 10), Color.Cyan);
		Primitives.DrawRectangle(Tick(_calibrationVector.Z, 200, 600, 500, 100, 10, 2), Color.Yellow);

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

		float r = 100;
		Vector2 circleCenter = new(800 + r, 450 - r);
		Primitives.DrawCircle(circleCenter, r, new Color(50, 50, 50), 0.9f);

		// aimAngle.Normalize();
		Primitives.DrawLine(circleCenter, circleCenter + (aimAngle * r * 0.9f), 4.0f, Color.Yellow, 1.0f);

		// Text Panel
		_textPanel.Draw(spriteBatch);

		spriteBatch.End();
	}
}