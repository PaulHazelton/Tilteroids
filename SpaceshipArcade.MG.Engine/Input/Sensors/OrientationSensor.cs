using MonoGame.Framework.Devices.Sensors;
using SpaceshipArcade.MG.Engine.Utilities;

namespace SpaceshipArcade.MG.Engine.Input.Sensors;

public class OrientationSensor
{
	private abstract record SampleMethod
	{
		public Vector3 CurrentValue { get; private set; }
		public abstract void Sample(Vector3 data);

		public record Direct : SampleMethod
		{
			public override void Sample(Vector3 data) => CurrentValue = data;
		}
		public record RollingAverage : SampleMethod
		{
			public int Count { get; private init; }
			private readonly Queue<Vector3> SampleQueue;

			public RollingAverage(int count, Vector3 initialValue)
			{
				Count = count;
				SampleQueue = new(capacity: count);

				for (int i = 0; i < Count; i++)
					SampleQueue.Enqueue(initialValue);
			}

			public override void Sample(Vector3 data)
			{
				SampleQueue.Dequeue();
				SampleQueue.Enqueue(data);

				// Vector average
				CurrentValue = SampleQueue.Aggregate((sum, v) => sum + v) / Count;
			}
		}
	}

	private readonly Accelerometer _accelerometer;
	private readonly Compass _compass;
	private readonly SampleMethod AccelerometerMethod = new SampleMethod.Direct();
	private readonly SampleMethod CompassMethod = new SampleMethod.Direct();

	public static bool IsSupported => Accelerometer.IsSupported && Compass.IsSupported;
	public Matrix CurrentValue { get; private set; } = Matrix.Identity;

	public OrientationSensor(byte accelerometerRollingAvgCount = 1, byte compassRollingAvgCount = 1)
	{
		_accelerometer = new();
		_compass = new();

		if (accelerometerRollingAvgCount <= 1)
			AccelerometerMethod = new SampleMethod.Direct();
		else
			AccelerometerMethod = new SampleMethod.RollingAverage(accelerometerRollingAvgCount, Vector3.Backward);

		if (compassRollingAvgCount <= 1)
			CompassMethod = new SampleMethod.Direct();
		else
			CompassMethod = new SampleMethod.RollingAverage(compassRollingAvgCount, Vector3.Up);
	}

	public void Sample()
	{
		AccelerometerMethod.Sample(_accelerometer.CurrentValue.Acceleration);
		CompassMethod.Sample(_compass.CurrentValue.MagnetometerReading);

		if (PMath.GetOrientation(AccelerometerMethod.CurrentValue, CompassMethod.CurrentValue, out Matrix result))
			CurrentValue = result;
		else
			CurrentValue = Matrix.Identity;
	}

	public void Start()
	{
		_accelerometer.Start();
		_compass.Start();
	}

	public void Stop()
	{
		_accelerometer.Stop();
		_compass.Stop();
	}

	public void Dispose()
	{
		_accelerometer.Dispose();
		_compass.Dispose();
	}
}
