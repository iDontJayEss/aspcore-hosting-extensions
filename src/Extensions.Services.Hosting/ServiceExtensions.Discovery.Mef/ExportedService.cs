using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceExtensions.Discovery.Mef
{
    internal class ExportedService
    {
        public Type Contract { get; set; }

        public Type Implementation { get; set; }

        public string ExportName { get; set; } = string.Empty;

        public bool IsDefaultContract => ExportName.Equals(Contract.FullName);
    }
}
