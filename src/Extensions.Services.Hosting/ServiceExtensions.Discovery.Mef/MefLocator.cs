using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;
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
            : this(ConfigAccess.GetConfig<MefSettings>("Mef")) { }

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
            => new CompositionContainer(
                new AggregateCatalog(
                    settings.Directories.Select(CreateCatalog)));

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
                group.ActiveExports.Select(export => new ServiceDescriptor(group.Contract, export.Implementation, group.Lifetime)));

        #endregion IServiceLocator Implementation

        private IEnumerable<ExportedService> AllExports
            => Container.Catalog.SelectMany(exportingPart =>
                exportingPart.ExportDefinitions.Select(export => new ExportedService
                {
                    Contract = $"{export.Metadata["ExportTypeIdentity"]}",
                    Implementation = ReflectionModelServices.GetPartType(exportingPart).Value,
                    ExportName = export.ContractName
                })).ToList();

        private IEnumerable<ExportGroup> ExportGroups
            => AllExports.GroupBy(service => service.Contract)
                    .Select(group => new ExportGroup
                    {
                        Contract = GetExportContract(group.First().Implementation, group.Key),
                        ContractName = GetContract(group.Key),
                        Lifetime = GetLifetime(group.Key),
                        AvailableExports = group
                    }).ToList();

        private static Type GetExportContract(Type impl, string contractType)
        {
            if (GetExports(impl).FirstOrDefault(export => IsMatch(export, contractType)) is ExportAttribute match)
                return match.ContractType;
            if (GetInheritedExports(impl).FirstOrDefault(export => IsMatch(export, contractType)) is InheritedExportAttribute inheritedMatch)
                return inheritedMatch.ContractType;
            return default;
        }

        private static bool IsMatch(ExportAttribute export, string typeName)
            => export.ContractType is Type exportType && exportType.FullName.Equals(typeName);


        private static IEnumerable<ExportAttribute> GetExports(Type type)
            => type.GetCustomAttributes<ExportAttribute>();

        private static IEnumerable<InheritedExportAttribute> GetInheritedExports(Type type)
            => type.GetCustomAttributes<InheritedExportAttribute>(inherit: true);

        private string GetContract<TService>() => GetContract(typeof(TService).FullName);

        private string GetContract(string serviceType)
        {
            var contract = "";
            if (Options.Contracts.Keys.FirstOrDefault(key => Regex.IsMatch(serviceType, key)) is string matchingKey)
                contract = Options.Contracts[matchingKey];
            return contract;
        }

        private ServiceLifetime GetLifetime(string serviceType)
        {
            var lifetime = ServiceLifetime.Singleton;
            if (Options.Lifetimes.Keys.FirstOrDefault(key => Regex.IsMatch(serviceType, key)) is string matchingKey)
                lifetime = Options.Lifetimes[matchingKey];
            return lifetime;
        }

        #region IDisposable Implementation

        private readonly IDisposable monitor;

        private bool disposedValue;

        /// <inheritdoc />
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

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Implementation
    }
}
