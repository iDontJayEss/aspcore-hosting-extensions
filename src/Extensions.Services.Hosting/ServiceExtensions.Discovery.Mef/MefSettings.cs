using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace ServiceExtensions.Discovery.Mef
{
    /// <summary>
    /// Settings used by the <see cref="MefLocator"/>.
    /// </summary>
    public class MefSettings
    {
        /// <summary>
        /// A collection of directory configurations.
        /// Each configuration describes a directory with exporting assemblies.
        /// </summary>
        public List<MefDirectorySettings> Directories { get; set; }
            = new List<MefDirectorySettings>();

        /// <summary>
        /// Dictionary mapping an expression to match against the <see cref="Type.FullName"/> of an exported contract.
        /// The value is the contract name to use.
        /// </summary>
        public Dictionary<string, string> Contracts { get; set; }
            = new Dictionary<string, string>();

        /// <summary>
        /// Dictionary mapping an expression to match against the <see cref="Type.FullName"/> of an exported contract.
        /// Unmatched contracts use <see cref="ServiceLifetime.Singleton"/> by default.
        /// </summary>
        public Dictionary<string, ServiceLifetime> Lifetimes { get; set; }
            = new Dictionary<string, ServiceLifetime>();
    }

    /// <summary>
    /// Configuration describing a directory with exporting assemblies.
    /// </summary>
    public class MefDirectorySettings
    {
        /// <summary>
        /// The path to the directory containing exports.
        /// This must be an absolute path, or relative to <see cref="AppDomain.BaseDirectory"/>.
        /// </summary>
        public string DirectoryPath { get; set; } = ".";

        /// <summary>
        /// The search string. The format should be the same as is used by the <see cref="System.IO.Directory.GetFiles(string, string)"/> method.
        /// </summary>
        public string SearchPattern { get; set; } = "*.dll";
    }
}
