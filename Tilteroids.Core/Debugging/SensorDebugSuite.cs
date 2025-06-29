using MonoGame.Framework.Devices.Sensors;
using SpaceshipArcade.MG.Engine.Debugging;
using SpaceshipArcade.MG.Engine.Input.Sensors;

namespace Tilteroids.Core.Debugging;

public class SensorDebugSuite
{
	private readonly Accelerometer _accelerometer;
	private Vector3 _aCalibrationVector;
	private readonly Vector3BarDisplay _aBarDisplay;
	private readonly Vector3CircleDisplay _aCircleDisplay;

	private readonly Compass _compass;
	private Vector3 _cCalibrationVector;
	private readonly Vector3BarDisplay _cBarDisplay;
	private readonly Vector3CircleDisplay _cCircleDisplay;

	private readonly OrientationSensor _orientationSensor;
	private Matrix _calibrationMatrix = Matrix.Identity;
	private readonly OrientationDisplay _orientationDisplay;

	public SensorDebugSuite(Accelerometer accelerometer, Compass compass, OrientationSensor orientationSensor, int unit)
	{
		_accelerometer = accelerometer;
		_aBarDisplay = new(new Rectangle(2 * unit, 2 * unit, 5 * unit, 1 * unit));
		_aCircleDisplay = new(position: new(6 * unit, 9 * unit), radius: 1 * unit);

		_compass = compass;
		_cBarDisplay = new(new Rectangle(10 * unit, 2 * unit, 5 * unit, 1 * unit));
		_cCircleDisplay = new(position: new(14 * unit, 9 * unit), radius: 1 * unit);

		_orientationSensor = orientationSensor;
		_orientationDisplay = new(position: new(18 * unit, 4.5f * unit), radius: 2 * unit);
	}

	public void Calibrate()
	{
		_aCalibrationVector = _accelerometer.CurrentValue.Acceleration;
		_cCalibrationVector = _compass.CurrentValue.MagnetometerReading;
		_calibrationMatrix = _orientationSensor.CurrentValue;
	}

	public void Draw(SpriteBatch spriteBatch)
	{
		_aBarDisplay.Draw(_accelerometer.CurrentValue.Acceleration, _aCalibrationVector);
		_aCircleDisplay.Draw(_accelerometer.CurrentValue.Acceleration, _aCalibrationVector);
		_cBarDisplay.Draw(_compass.CurrentValue.MagnetometerReading, _cCalibrationVector);
		_cCircleDisplay.Draw(_compass.CurrentValue.MagnetometerReading, _cCalibrationVector);
		_orientationDisplay.Draw(_orientationSensor.CurrentValue, _calibrationMatrix);
	}
}