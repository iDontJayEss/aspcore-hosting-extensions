using ServiceExtensions.Discovery;
using ServiceExtensions.Discovery.Mef;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MefExtensions
    {

        public static IServiceCollection AddMefServices(this IServiceCollection services, bool register = false, Action<IServiceCollection, IServiceLocator> startupActions = default)
        {
            return services.AddMefServices(register ? inject : startupActions);

            void inject(IServiceCollection svc, IServiceLocator locator)
            {
                InjectExports(svc, locator);
                startupActions?.Invoke(svc, locator);
            }
        }

        public static IServiceCollection AddMefServices(this IServiceCollection services, Action<IServiceCollection, IServiceLocator> startupActions)
        {
            services.AddOptions<MefSettings>("Mef");
            services.AddSingleton<IServiceLocator, MefLocator>();
            services.AddMefServices();
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                var locator = scope.ServiceProvider.GetRequiredService<IServiceLocator>();
                startupActions?.Invoke(services, locator);
            }
            return services;
        }

        private static void InjectExports(IServiceCollection services, IServiceLocator locator)
        {
            foreach (var service in locator.ExportingServices)
                services.Add(service);
        }
    }
}
