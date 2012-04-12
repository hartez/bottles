using System;
using NUnit.Framework;
using System.Linq;
using FubuTestingSupport;

namespace Bottles.Tests
{
    [TestFixture]
    public class PackageOrderingIntegratedTester
    {
        private void loadPackages(Action<StubBottleLoader> configuration)
        {
            BottlesRegistry.LoadBottles(facility =>
            {
                var loader = new StubBottleLoader();
                configuration(loader);

                facility.Loader(loader);
            });
        }

        private void thePackageNamesInOrderShouldBe(params string[] names)
        {
            BottlesRegistry.Bottles.Select(x => x.Name)
                .ShouldHaveTheSameElementsAs(names);
        }

        [Test]
        public void orders_by_name()
        {
            loadPackages(x =>
            {
                x.HasPackage("D");
                x.HasPackage("C");
                x.HasPackage("B");
            });

            thePackageNamesInOrderShouldBe("B", "C", "D");
        }

        [Test]
        public void order_with_a_dependency()
        {
            loadPackages(x =>
            {
                x.HasPackage("A1");
                x.PackageFor("A").MandatoryDependency("A1");
            });

            BottlesRegistry.AssertNoFailures();

            thePackageNamesInOrderShouldBe("A1", "A");
        }

        [Test]
        public void logs_failure_with_missing_dependency()
        {
            loadPackages(x =>
            {
                x.PackageFor("A").MandatoryDependency("B");
            });

            Exception<ApplicationException>.ShouldBeThrownBy(() =>
            {
                BottlesRegistry.AssertNoFailures();
            }).Message.ShouldContain("Missing required Bottle dependency named 'B'");
        }
    }
}