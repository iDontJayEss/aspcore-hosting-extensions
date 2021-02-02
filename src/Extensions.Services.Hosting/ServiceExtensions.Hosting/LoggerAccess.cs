using Microsoft.Extensions.DependencyInjection;
using ServiceExtensions.Hosting;
using System;

namespace Microsoft.Extensions.Logging
{
    /// <summary>
    /// Methods exposing global access to logging facilities.
    /// </summary>
    public static class LoggerAccess
    {
        private static ILoggerFactory LogFactory
            => HostAccess.ServiceProvider.GetRequiredService<ILoggerFactory>();

        /// <inheritdoc cref="LoggerFactoryExtensions.CreateLogger{T}(ILoggerFactory)" />
        public static ILogger<TCategory> GetLogger<TCategory>()
            => LogFactory.CreateLogger<TCategory>();

        /// <inheritdoc cref="LoggerFactoryExtensions.CreateLogger(ILoggerFactory, Type)" />
        public static ILogger GetLogger(Type consumer)
            => LogFactory.CreateLogger(consumer);
    }
}
