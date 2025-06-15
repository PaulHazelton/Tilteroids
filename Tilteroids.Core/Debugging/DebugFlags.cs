using System;

namespace Tilteroids.Core.Debugging;

[Flags]
public enum DebugFlags
{
	None = 0,
	Physics = 1,
	SensorData = 2,
	AimVector = 4,
}