using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceExtensions.Discovery.Mef.Tests
{
    public static class ConfigurationValues
    {
        public static MefSettings DefaultImplSettings
            => new MefSettings
            {
                Directories = DirectoryConfig,
                Contracts = DefaultContractConfig
            };

        public static MefSettings NamespaceFilterSettings
            => new MefSettings
            {
                Directories = DirectoryConfig,
                Contracts = NamespaceContractConfig
            };

        public static MefSettings FirstSettings
            => new MefSettings
            {
                Directories = DirectoryConfig,
                Contracts = GlobalFirstConfig
            };


        public static MefSettings SecondSettings
            => new MefSettings
            {
                Directories = DirectoryConfig,
                Contracts = GlobalSecondConfig
            };

        public static MefSettings MultiSettings
            => new MefSettings
            {
                Directories = DirectoryConfig,
                Contracts = GlobalMultiConfig
            };

        private static List<MefDirectorySettings> DirectoryConfig
            => new List<MefDirectorySettings>
            {
                new MefDirectorySettings
                {
                    SearchPattern = "ServiceExtensions.*.dll"
                }
            };

        private static Dictionary<string, string> DefaultContractConfig
            => new Dictionary<string, string>();

        private static Dictionary<string, string> NamespaceContractConfig
            => new Dictionary<string, string>
            {
                ["IMySampleContract"] = "first",
                ["ServiceExtensions.Discovery.Mef.MockImports"] = "second"
            };

        private static Dictionary<string, string> GlobalFirstConfig
            => new Dictionary<string, string>
            {
                ["ServiceExtensions.Discovery.Mef.MockImports"] = "first"
            };

        private static Dictionary<string, string> GlobalSecondConfig
            => new Dictionary<string, string>
            {
                ["ServiceExtensions.Discovery.Mef.MockImports"] = "second"
            };

        private static Dictionary<string, string> GlobalMultiConfig
            => new Dictionary<string, string>
            {
                ["ServiceExtensions.Discovery.Mef.MockImports"] = "multi"
            };
    }
}
