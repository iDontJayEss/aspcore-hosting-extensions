using ServiceExtensions.Discovery.Mef.MockImports;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceExtensions.Discovery.Mef.Tests
{
    using static ConfigurationValues;

    [TestClass]
    public class MefLocatorTests
    {

        private static MefLocator DefaultImplLocator => new MefLocator(DefaultImplSettings);

        private static MefLocator NamespaceLocator => new MefLocator(NamespaceFilterSettings);

        private static MefLocator FirstLocator => new MefLocator(FirstSettings);

        private static MefLocator SecondLocator => new MefLocator(SecondSettings);

        private static MefLocator MultiLocator => new MefLocator(MultiSettings);

        [TestMethod]
        public void Constructor_Default_Test()
        {
            Assert.IsInstanceOfType(new MefLocator(), typeof(MefLocator));
        }

        [TestMethod]
        public void Resolve_Test() 
            => Validate(DefaultImplLocator, typeof(DefaultImplementation), typeof(AnotherDefaultImplementation));

        [TestMethod]
        public void Resolve_Named_Test()
        {
            Validate(FirstLocator, typeof(FirstNamedImpl), typeof(AnotherFirstNamedImpl));
            Validate(SecondLocator, typeof(SecondNamedImpl), typeof(AnotherSecondNamedImpl));
        }

        [TestMethod]
        public void Resolve_Namespace_Test()
            => Validate(NamespaceLocator, typeof(FirstNamedImpl), typeof(AnotherSecondNamedImpl));

        [TestMethod]
        public void Resolve_NameOverride_Test()
        {
            Validate(DefaultImplLocator, "first", typeof(FirstNamedImpl), typeof(AnotherFirstNamedImpl));
            Validate(DefaultImplLocator, "second", typeof(SecondNamedImpl), typeof(AnotherSecondNamedImpl));
        }

        [TestMethod]
        public void GetServices_Test()
        {
            var locator = DefaultImplLocator;
            var services = locator.ExportingServices;
        }



        private static void Validate(MefLocator locator, string contractName, Type firstImpl, Type secondImpl)
        {
            Assert.IsInstanceOfType(locator.Resolve<IMySampleContract>(contractName), firstImpl);
            Assert.IsInstanceOfType(locator.Resolve<IAnotherSampleContract>(contractName), secondImpl);
        }

        private static void Validate(MefLocator locator, Type firstImpl, Type secondImpl)
        {
            Assert.IsInstanceOfType(locator.Resolve<IMySampleContract>(), firstImpl);
            Assert.IsInstanceOfType(locator.Resolve<IAnotherSampleContract>(), secondImpl);
        }

    }
}
