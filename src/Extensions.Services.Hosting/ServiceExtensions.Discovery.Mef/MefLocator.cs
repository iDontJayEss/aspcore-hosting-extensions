using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.Composition.ReflectionModel;
using System.Text.RegularExpressions;

namespace ServiceExtensions.Discovery.Mef
{
    /// <summary>
    /// A facility for locating services build on top of .NET Managed Extensibility Framework (MEF).
    /// </summary>
    public class MefLocator : IServiceLocator, IDisposable
    {
        /// <summary>
        /// Creates a new instance of <see cref="MefLocator"/> using the default configuration.
        /// </summary>
        public MefLocator()
            : this(ConfigAccessExtensions.GetConfig<MefSettings>("Mef")) { }

        /// <summary>
        /// Creates a new instance of <see cref="MefLocator"/> using the provided <paramref name="options"/>.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public MefLocator(MefSettings options)
            => SetOptions(options ?? new MefSettings());

        /// <summary>
        /// Creates a new instance of <see cref="MefLocator"/> using <paramref name="options"/> provided by a DI Container.
        /// </summary>
        /// <param name="options">The configuration options.</param>
        public MefLocator(IOptionsMonitor<MefSettings> options)
            : this(options.CurrentValue)
            => monitor = options.OnChange(SetOptions);

        /// <summary>
        /// Configuration options used by this class.
        /// </summary>
        public MefSettings Options { get => options; set => SetOptions(value); }

        private MefSettings options = new MefSettings
        {
            Directories = new List<MefDirectorySettings>
                {
                    new MefDirectorySettings()
                }
        };

        private CompositionContainer Container { get; set; }

        private void SetOptions(MefSettings options)
        {
            this.options = options;
            Container = CreateContainer(options);
        }

        private static CompositionContainer CreateContainer(MefSettings settings)
        {
            var catalogs = settings.Directories.Select(CreateCatalog);
            var container = new CompositionContainer(new AggregateCatalog(catalogs));
            return container;
        }

        private static ComposablePartCatalog CreateCatalog(MefDirectorySettings settings)
            => new DirectoryCatalog(settings.DirectoryPath, settings.SearchPattern);

        #region IServiceLocator Implementation

        /// <inheritdoc />
        public TService Resolve<TService>()
            => Resolve<TService>(GetContract<TService>());

        /// <inheritdoc />
        /// <remarks>Overrides the <see cref="MefSettings.Contracts"/> section of the configuration.</remarks>
        public TService Resolve<TService>(string contractName)
            => Container.GetExportedValue<TService>(contractName);

        /// <inheritdoc />
        public IEnumerable<TService> ResolveAll<TService>()
            => ResolveAll<TService>(GetContract<TService>());

        /// <inheritdoc />
        /// <remarks>Overrides the <see cref="MefSettings.Contracts"/> section of the configuration.</remarks>
        public IEnumerable<TService> ResolveAll<TService>(string contractName)
            => Container.GetExportedValues<TService>(contractName);

        /// <inheritdoc />
        public IEnumerable<ServiceDescriptor> ExportingServices
            => ExportGroups.SelectMany(group =>
                group.ActiveExports.Select(export => new ServiceDescriptor(export.Contract, export.Implementation, group.Lifetime)));

        #endregion IServiceLocator Implementation

        private IEnumerable<ExportedService> AllExports
            => Container.Catalog.SelectMany(exportingPart =>
                exportingPart.ExportDefinitions.Select(export => new ExportedService
                {
                    Contract = Type.GetType($"{export.Metadata["ExportTypeIdentity"]}"),
                    Implementation = ReflectionModelServices.GetPartType(exportingPart).Value,
                    ExportName = export.ContractName
                }));

        private IEnumerable<ExportGroup> ExportGroups
            => AllExports.GroupBy(service => service.Contract)
                    .Select(group => new ExportGroup
                    {
                        Contract = group.Key,
                        ContractName = GetContract(group.Key),
                        Lifetime = GetLifetime(group.Key),
                        AvailableExports = group
                    });


        private string GetContract<TService>() => GetContract(typeof(TService));

        private string GetContract(Type serviceType)
        {
            var contract = "";
            if (Options.Contracts.Keys.FirstOrDefault(key => Regex.IsMatch(serviceType.FullName, key)) is string matchingKey)
                contract = Options.Contracts[matchingKey];
            return contract;
        }

        private ServiceLifetime GetLifetime(Type serviceType)
        {
            var lifetime = ServiceLifetime.Singleton;
            if (Options.Lifetimes.Keys.FirstOrDefault(key => Regex.IsMatch(serviceType.FullName, key)) is string matchingKey)
                lifetime = Options.Lifetimes[matchingKey];
            return lifetime;
        }

        #region IDisposable Implementation

        private readonly IDisposable monitor;

        private bool disposedValue;
        

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    monitor?.Dispose();
                    Container?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Implementation
    }
}
