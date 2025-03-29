using System;

namespace Veedja.MG.Engine.Extensions
{
	public static class ServiceProviderExtensions
	{
		public static T GetService<T>(this IServiceProvider s)
		{
			return (T)s.GetService(typeof(T))!;
		}
	}
}