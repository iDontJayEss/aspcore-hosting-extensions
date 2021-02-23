using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
        /// Determines how the base directory for a relative <see cref="DirectoryPath"/> is resolved.
        /// </summary>
        public BaseDirectoryType DirectoryType { get; set; } = BaseDirectoryType.Directory;

        /// <summary>
        /// The path to the directory containing exports.
        /// This must be an absolute path, or relative to <see cref="AppDomain.BaseDirectory"/>.
        /// </summary>
        public string DirectoryPath { get; set; } = ".";

        /// <summary>
        /// The search string. The format should be the same as is used by the <see cref="Directory.GetFiles(string, string)"/> method.
        /// </summary>
        public string SearchPattern { get; set; } = "*.dll";

        private bool IsAbsolutePath =>
            Path.IsPathRooted(DirectoryPath);

        /// <summary>
        /// The <see cref="DirectoryPath"/> with any alternate base directory settings applied.
        /// </summary>
        public string FullPath => IsAbsolutePath || DirectoryType == BaseDirectoryType.Directory
        ? DirectoryPath
        : DirectoryType == BaseDirectoryType.AppDomain
            ? Path.Combine(AppDomainPath, DirectoryPath)
            : Path.Combine(AssemblyPath, DirectoryPath);

        private static string AppDomainPath
            => AppDomain.CurrentDomain.BaseDirectory;

        private static string AssemblyPath
            => Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
    }

    /// <summary>
    /// Indicates a method for resolving an application's base directory.
    /// </summary>
    public enum BaseDirectoryType
    {
        /// <summary>
        /// An unspecified resolution type.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// Indicates the <see cref="Directory.GetCurrentDirectory"/> method should be used.
        /// </summary>
        Directory,

        /// <summary>
        /// Indicates the <see cref="Assembly.GetExecutingAssembly"/> method should be used.
        /// </summary>
        Assembly,

        /// <summary>
        /// Indicates the <see cref="AppDomain.BaseDirectory"/> property should be used.
        /// </summary>
        AppDomain
    }
}
