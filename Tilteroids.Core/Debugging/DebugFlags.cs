using Microsoft.Xna.Framework.Input;

namespace Tilteroids.Core.Debugging;

[Flags]
public enum DebugFlags
{
	None = 0,
	Physics = 1,
	SensorData = 2,
	AimVector = 4,
	WorldWrapView = 8,
	ManualStepping = 16,
}

public static class DebugFlagsExtensions
{
	public static Keys GetKey(this DebugFlags value) => value switch
	{
		DebugFlags.Physics => Keys.F1,
		DebugFlags.SensorData => Keys.F2,
		DebugFlags.AimVector => Keys.F3,
		DebugFlags.WorldWrapView => Keys.F4,
		DebugFlags.ManualStepping => Keys.F5,
		_ => Keys.None
	};
}