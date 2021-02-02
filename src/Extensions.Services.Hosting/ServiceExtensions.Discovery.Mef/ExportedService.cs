using System;
using System.ComponentModel.Composition;

namespace ServiceExtensions.Discovery.Mef
{
    /// <summary>
    /// Internal type used to summarize MEF exports.
    /// </summary>
    internal class ExportedService
    {
        /// <summary>
        /// The broadcasted export identity.
        /// </summary>
        public string Contract { get; set; }
            = string.Empty;

        /// <summary>
        /// The implementation Type: The Type of the class decorated with the <see cref="ExportAttribute"/>.
        /// </summary>
        public Type Implementation { get; set; }

        /// <summary>
        /// The contract name. If no name is provided in the <see cref="ExportAttribute"/>, this is the <see cref="Type.FullName"/> of the <see cref="Contract"/>.
        /// </summary>
        public string ExportName { get; set; } = string.Empty;

        /// <summary>
        /// Shows whether this export is named or default.
        /// </summary>
        public bool IsDefaultContract => ExportName.Equals(Contract);


    }
}
