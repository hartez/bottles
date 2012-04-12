using System.Linq;
using Bottles.BottleLoaders;
using Bottles.Diagnostics;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests
{
    [TestFixture]
    public class SolutionDirectoryPackageLoaderTester
    {
        private string thePathToScan = "solDirPackLoad";
        private SolutionDirectoryBottleLoader theLoader;

        [SetUp]
        public void BeforeEach()
        {
            var fs = new FileSystem();
            fs.DeleteDirectory(thePathToScan);
            fs.CreateDirectory(thePathToScan);
            fs.CreateDirectory(thePathToScan, "bin");

            theLoader = new SolutionDirectoryBottleLoader(thePathToScan.ToFullPath());
            var manifest = new BottleManifest{
                Name = "test-mani"
            };

            fs.PersistToFile(manifest, thePathToScan, BottleManifest.FILE);
        }

        [Test]
        public void there_are_7_manifests_that_are_modules_in_fubu()
        {
            var foundPackages = theLoader.Load(new BottleLog());
            foundPackages.Count().ShouldEqual(1);
            foundPackages.First().Name.ShouldEqual("test-mani");
        }
    }
}