using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServiceExtensions.Discovery.Mef.MockImports;
using System;
using System.Collections.Generic;
using System.Linq;

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
            Assert.AreEqual(2, services.Count());
            Assert.AreEqual(1, GetMatches<IMySampleContract, DefaultImplementation>(services).Count());
            Assert.AreEqual(1, GetMatches<IAnotherSampleContract, AnotherDefaultImplementation>(services).Count());
        }

        [TestMethod]
        public void GetServices_Multi_Test()
        {
            var locator = MultiLocator;
            var services = locator.ExportingServices;
            Assert.AreEqual(4, services.Count());
            Assert.AreEqual(2, GetMatches<IMySampleContract>(services).Count());
            Assert.AreEqual(2, GetMatches<IAnotherSampleContract>(services).Count());
        }

        private IEnumerable<ServiceDescriptor> GetMatches<TService>(IEnumerable<ServiceDescriptor> services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            => services.Where(svc => svc.ServiceType == typeof(TService) && svc.Lifetime == lifetime);

        private IEnumerable<ServiceDescriptor> GetMatches<TService, TImpl>(IEnumerable<ServiceDescriptor> services, ServiceLifetime lifetime = ServiceLifetime.Singleton)
            => GetMatches<TService>(services, lifetime).Where(svc => svc.ImplementationType == typeof(TImpl));

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
