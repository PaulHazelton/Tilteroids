using System;

namespace SpaceshipArcade.MG.Engine.Utilities
{
	public static class EnumHelpers
	{
		public static T[] GetValues<T>()
		{
			return (T[])Enum.GetValues(typeof(T));
		}
	}
}