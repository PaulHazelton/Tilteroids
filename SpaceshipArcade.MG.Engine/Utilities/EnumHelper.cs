namespace SpaceshipArcade.MG.Engine.Utilities;

public static class EnumHelpers
{
	public static T[] GetValues<T>() => (T[])Enum.GetValues(typeof(T));
}