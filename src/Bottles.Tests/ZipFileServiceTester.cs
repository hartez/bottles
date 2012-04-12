using System.IO;
using AssemblyBottle;
using Bottles.Zipping;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace Bottles.Tests
{
    [TestFixture]
    public class ZipFileServiceTester
    {
        [SetUp]
        public void SetUp()
        {
            var assembly = typeof (AssemblyBottleMarker).Assembly;
            var stream = assembly.GetManifestResourceStream(typeof (AssemblyBottleMarker), "pak-data.zip");

            var service = new ZipFileService(new FileSystem());
            service.ExtractTo("description of this", stream, "package-data");

            // These 3 files should be in the zip file embedded within the AssemblyPackage assembly
            File.Exists(Path.Combine("package-data", "1.txt")).ShouldBeTrue();
            File.Exists(Path.Combine("package-data", "2.txt")).ShouldBeTrue(); ;
            File.Exists(Path.Combine("package-data", "3.txt")).ShouldBeTrue(); ;
        }
    }
}