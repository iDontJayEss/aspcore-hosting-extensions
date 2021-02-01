using Microsoft.Extensions.DependencyInjection;
using ServiceExtensions.Hosting;
using System;

namespace Microsoft.Extensions.Hosting
{
    public static class HostAccessExtensions
    {
        public static IServiceCollection ConfigureHosting(this IServiceCollection services)
        {
            HostAccess.AmbientProvider = services.BuildServiceProvider();
            return services;
        }

        public static IServiceProvider ConfigureHosting(this IServiceProvider provider)
        {
            HostAccess.AmbientProvider = provider;
            return provider;
        }
    }
}
