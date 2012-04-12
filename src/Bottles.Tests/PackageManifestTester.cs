using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;
using System.Linq;

namespace Bottles.Tests
{
    [TestFixture]
    public class PackageManifestTester
    {
        [Test]
        public void fileset_for_searching()
        {
            var fileSet = BottleManifest.FileSetForSearching();
            fileSet.DeepSearch.ShouldBeTrue();
            fileSet.Include.ShouldEqual(BottleManifest.FILE);
            fileSet.Exclude.ShouldBeNull();
        }

        [Test]
        public void set_role_to_module()
        {
            var manifest = new BottleManifest();
            manifest.SetRole(BottleRoles.Module);


            manifest.Role.ShouldEqual(BottleRoles.Module);

            manifest.ContentFileSet.ShouldNotBeNull();
            manifest.ContentFileSet.Include.ShouldContain("*.as*x");
        }

        [Test]
        public void set_role_to_config()
        {
            var manifest = new BottleManifest();
            manifest.SetRole(BottleRoles.Config);

            manifest.Role.ShouldEqual(BottleRoles.Config);

            manifest.ContentFileSet.ShouldBeNull();
            manifest.DataFileSet.ShouldBeNull();
            manifest.ConfigFileSet.ShouldNotBeNull();

            manifest.ConfigFileSet.Include.ShouldEqual("*.*");

            manifest.Assemblies.Any().ShouldBeFalse();
            
        }


        [Test]
        public void read_config_manifest_from_file()
        {
            var manifest = new BottleManifest();
            manifest.SetRole(BottleRoles.Config);

            var system = new FileSystem();
            system.WriteObjectToFile("manifest.xml", manifest);

            var manifest2 = system.LoadFromFile<BottleManifest>("manifest.xml");
            manifest2.ContentFileSet.ShouldBeNull();
        }

        [Test]
        public void set_role_to_binaries()
        {
            var manifest = new BottleManifest();
            manifest.SetRole(BottleRoles.Binaries);

            manifest.ContentFileSet.ShouldBeNull();
            manifest.DataFileSet.ShouldBeNull();
            manifest.ConfigFileSet.ShouldBeNull();
        }

        [Test]
        public void set_role_to_data()
        {
            var manifest = new BottleManifest();
            manifest.SetRole(BottleRoles.Data);

            manifest.ContentFileSet.ShouldBeNull();
            manifest.ConfigFileSet.ShouldBeNull();

            manifest.DataFileSet.DeepSearch.ShouldBeTrue();
            manifest.DataFileSet.Include.ShouldEqual("*.*");
        }

    }
}