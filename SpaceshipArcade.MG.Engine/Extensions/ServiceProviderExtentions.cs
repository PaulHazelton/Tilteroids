namespace SpaceshipArcade.MG.Engine.Extensions;

public static class ServiceProviderExtensions
{
	public static T GetService<T>(this IServiceProvider s) => (T)s.GetService(typeof(T))!;
}