using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ServiceExtensions.Discovery.Mef.Tests
{
    [TestClass]
    public class MefSettingsTests
    {
        private const string AbsolutePath = @"C:\GIT\aspcore-hosting-extensions\src\Extensions.Services.Hosting\tests\ServiceExtensions.Discovery.Mef.Tests\bin\Debug\netcoreapp3.1";
        private static MefDirectorySettings AbsolutePathSettings
            => new MefDirectorySettings
            {
                DirectoryPath = AbsolutePath,
                DirectoryType = BaseDirectoryType.AppDomain // Arbitrary. Just checking that it gets ignored.
            };

        private static MefDirectorySettings DirectoryPathSettings
            => new MefDirectorySettings();

        private static MefDirectorySettings AssemblyPathSettings
            => new MefDirectorySettings
            {
                DirectoryType = BaseDirectoryType.Assembly
            };

        private static MefDirectorySettings AppDomainSettings
            => new MefDirectorySettings
            {
                DirectoryType = BaseDirectoryType.AppDomain
            };

        [TestMethod]
        public void FullPath_Test()
        {
            Assert.AreEqual(".", DirectoryPathSettings.FullPath);
        }

        [TestMethod]
        public void FullPath_Absolute_Test()
        {
            Assert.AreEqual(AbsolutePath, AbsolutePathSettings.FullPath);
        }

        [TestMethod]
        public void FullPath_Assembly_Test()
        {
            Assert.AreEqual(Path.Combine(Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path)),"."), AssemblyPathSettings.FullPath);
        }

        [TestMethod]
        public void FullPath_AppDomain_Test()
        {
            Assert.AreEqual(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"."), AppDomainSettings.FullPath);
        }
    }
}
