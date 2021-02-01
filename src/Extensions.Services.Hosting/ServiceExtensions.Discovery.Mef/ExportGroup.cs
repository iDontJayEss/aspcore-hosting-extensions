using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ServiceExtensions.Discovery.Mef
{
    internal class ExportGroup
    {
        public Type Contract { get; set; }

        public string ContractName { get; set; }
            = string.Empty;

        public ServiceLifetime Lifetime { get; set; }
            = ServiceLifetime.Singleton;

        public IEnumerable<ExportedService> AvailableExports { get; set; }
            = Enumerable.Empty<ExportedService>();

        private IEnumerable<ExportedService> DefaultExports
            => AvailableExports.Where(export => export.IsDefaultContract);

        public IEnumerable<ExportedService> ActiveExports
            => string.IsNullOrWhiteSpace(ContractName)
            ? DefaultExports
            : AvailableExports.Where(export => export.Contract.Equals(ContractName));
    }
}
