using System;
using Microsoft.Xna.Framework;
using MonoGame.Framework.Devices.Sensors;

namespace Tilteroids.Core.Controllers;

public class OldTiltController
{
	private readonly Accelerometer _accelerometer;
	
	private Vector3 _currentVector = new(0, 0, 1);
	private Vector3 _calibrationVector = new(0, 0, 1);
	private Matrix _transformationMatrix = Matrix.Identity;

	public Vector3 TargetVector { get; init; } = new(0, 0, 1);
	public Vector2 AimVector { get; private set; } = new(0, 0);

	public OldTiltController(Accelerometer accelerometer, Vector3? targetVector = null)
	{
		_accelerometer = accelerometer;
		TargetVector = targetVector ?? new(0, 0, 1);
	}

	public void Update()
	{
		_currentVector = Vector3.Normalize(_accelerometer.CurrentValue.Acceleration);

		var calibratedVector = Vector3.Transform(_currentVector, _transformationMatrix);

		// Swap x and y because it's landscape
		AimVector = new(calibratedVector.Y, calibratedVector.X);
	}

	public void Calibrate()
	{
		_calibrationVector = _currentVector;

		_transformationMatrix = GetRotationMatrix();
	}
	
	private Matrix GetRotationMatrix()
	{
		float dot = Vector3.Dot(_calibrationVector, TargetVector);

		if (Math.Abs(dot - 1.0f) < 1e-6f)
			return Matrix.Identity;

		if (Math.Abs(dot + 1.0f) < 1e-6)
			return Matrix.CreateFromAxisAngle(Vector3.UnitY, MathHelper.Pi);

		Vector3 axis = Vector3.Normalize(Vector3.Cross(_calibrationVector, TargetVector));
		float angle = (float)Math.Acos(dot);
		return Matrix.CreateFromAxisAngle(axis, angle);
	}
}