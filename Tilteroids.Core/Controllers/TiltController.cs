using SpaceshipArcade.MG.Engine.Input.Sensors;

namespace Tilteroids.Core.Controllers;

public class TiltController
{
	private readonly OrientationSensor _orientationSensor;

	private Matrix _currentOrientation = Matrix.Identity;
	private Matrix _calibrationOrientation = Matrix.Identity;

	public Vector2 AimVector { get; private set; } = Vector2.Zero;

	public TiltController(OrientationSensor orientationSensor)
	{
		_orientationSensor = orientationSensor;
	}

	public void Update()
	{
		_currentOrientation = _orientationSensor.CurrentValue;

		AimVector = ComputeAimVector();
	}

	public void Calibrate()
	{
		_calibrationOrientation = _currentOrientation;
	}

	private Vector2 ComputeAimVector()
	{
		// Keep in mind that orientation is already inverted by default

		// Rotate the current matrix by the inverse of the calibration matrix
		// Then look at where the z^ base vector lies, as projected on the the xy plane

		// Find matrix R such that when applied to calibration, results in current
		// [R] * [Calibration] = [Current]
		// [R] = [Calibration]^ * [Current]
		var relativeRotation = Matrix.Invert(_calibrationOrientation) * _currentOrientation;

		// Project the z basis vector onto the xy plane
		var zBasis = relativeRotation.Backward;
		Vector2 projection = new(zBasis.Y, zBasis.X);

		// Projection gives the sin of the angle, arcSin to get the angle.
		return new((float)(2 * Math.Asin(projection.X) / Math.PI), (float)(2 * Math.Asin(projection.Y) / Math.PI));
	}
}