using ServiceExtensions.Discovery;
using ServiceExtensions.Discovery.Mef;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for adding MEF support to ASP.NET Core hosted applications.
    /// </summary>
    public static class MefExtensions
    {
        /// <summary>
        /// Registers and configures the <see cref="MefLocator"/> implementation in the DI container. Optionally registers all exported services in the container.
        /// </summary>
        /// <param name="services">The Startup services.</param>
        /// <param name="register">If <c>true</c>, registers discovered services.</param>
        /// <param name="startupActions">Delegate granting access to the <see cref="IServiceLocator"/> during Startup.</param>
        /// <returns>The updated Startup services.</returns>
        public static IServiceCollection AddMefServices(this IServiceCollection services, bool register = false, Action<IServiceCollection, IServiceLocator> startupActions = default)
        {
            return services.AddMefServices(register ? inject : startupActions);

            void inject(IServiceCollection svc, IServiceLocator locator)
            {
                InjectExports(svc, locator);
                startupActions?.Invoke(svc, locator);
            }
        }

        /// <summary>
        /// Registers and configures the <see cref="MefLocator"/> implementation in the DI container, and exposes the <see cref="IServiceLocator"/> as a Scoped service during Startup.
        /// </summary>
        /// <param name="services">The Startup services.</param>
        /// <param name="startupActions">Delegate granting access to the <see cref="IServiceLocator"/> during Startup.</param>
        /// <returns>The updated Startup services.</returns>
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
