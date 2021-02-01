using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace ServiceExtensions.Discovery
{
    /// <summary>
    /// Represents a facility for discovering components in external assemblies.
    /// </summary>
    public interface IServiceLocator
    {
        /// <summary>
        /// Retrieves an implementation of the provided type.
        /// </summary>
        /// <typeparam name="TService">The contract an implementation must satisfy.</typeparam>
        /// <returns>An implementation of the provided type, if it exists.</returns>
        TService Resolve<TService>();

        /// <summary>
        /// Retrieves an implementation type matching the provided <paramref name="contractName"/>.
        /// </summary>
        /// <typeparam name="TService">The contract an implementation must satisfy.</typeparam>
        /// <param name="contractName">The broadcasting name of the contract implementation.</param>
        /// <returns>An implementation matching the provided name, if it exists.</returns>
        TService Resolve<TService>(string contractName);

        /// <summary>
        /// Retrieves all implementations of the provided type.
        /// </summary>
        /// <typeparam name="TService">The contract an implementation must satisfy.</typeparam>
        /// <returns>A collection of implementations matching the provided type.</returns>
        IEnumerable<TService> ResolveAll<TService>();

        /// <summary>
        /// Retrieves all implementations of the provided type matching the provided <paramref name="contractName"/>.
        /// </summary>
        /// <typeparam name="TService">The contract an implementation must satisfy.</typeparam>
        /// <param name="contractName">The broadcasting name of the contract implementation.</param>
        /// <returns>A collection of matching implementations.</returns>
        IEnumerable<TService> ResolveAll<TService>(string contractName);

        /// <summary>
        /// A collection of exporting services. These can be registered in a DI container.
        /// </summary>
        IEnumerable<ServiceDescriptor> ExportingServices { get; }
    }
}
