using Bottles.Commands;
using FubuCore;
using NUnit.Framework;
using Rhino.Mocks;
using FubuTestingSupport;

namespace Bottles.Tests.Commands
{
    [TestFixture]
    public class AssembliesInputTester
    {
        private IFileSystem theFileSystem;
        private AssembliesInput theInput;
        private BottleManifest theBottleManifest;
        private BottleManifest theApplicationManifest;

        [SetUp]
        public void SetUp()
        {
            theFileSystem = MockRepository.GenerateMock<IFileSystem>();
            theInput = new AssembliesInput{
                Directory = "directory1",
                FileNameFlag = null
            };

            theBottleManifest = new BottleManifest(){
                Name = "the package"
            };
            theApplicationManifest = new BottleManifest(){
                Name = "the application"
            };
        }

        private void thePackageManifestFileExists()
        {
            theFileSystem.Stub(
                x => x.FileExists(theInput.Directory, BottleManifest.FILE))
                .Return(true);

            theFileSystem.Stub(
                x =>
                x.LoadFromFile<BottleManifest>(theInput.Directory, BottleManifest.FILE))
                .Return(theBottleManifest);
        }

        private void theApplicationManifestFileExists()
        {
            theFileSystem.Stub(
                x => x.FileExists(theInput.Directory, BottleManifest.FILE))
                .Return(true);

            theFileSystem.Stub(
                x =>
                x.LoadFromFile<BottleManifest>(theInput.Directory, BottleManifest.FILE))
                .Return(theApplicationManifest);
        }

        [Test]
        public void find_the_package_manifest_file()
        {
            thePackageManifestFileExists();
            theInput.FindManifestAndBinaryFolders(theFileSystem);

            theInput.Manifest.ShouldBeTheSameAs(theBottleManifest);

            theInput.Manifest.ManifestFileName.ShouldEqual(FileSystem.Combine(theInput.Directory, BottleManifest.FILE));
        }


        [Test]
        public void find_the_application_manifest_file()
        {
            theApplicationManifestFileExists();
            theInput.FindManifestAndBinaryFolders(theFileSystem);

            theInput.Manifest.ShouldBeTheSameAs(theApplicationManifest);

            theInput.Manifest.ManifestFileName.ShouldEqual(FileSystem.Combine(theInput.Directory, BottleManifest.FILE));
        }

        [Test]
        public void use_the_file_name_if_it_is_given_and_the_manifest_exists_at_that_file()
        {
            var manifest = new BottleManifest(){
                Name = "special"
            };

            theInput.FileNameFlag = "special.xml";

            theFileSystem.Stub(x => x.FileExists(theInput.Directory, theInput.FileNameFlag))
                .Return(true);

            theFileSystem.Stub(x => x.LoadFromFile<BottleManifest>(theInput.Directory, theInput.FileNameFlag))
                .Return(manifest);

            theInput.FindManifestAndBinaryFolders(theFileSystem);

            theInput.Manifest.ShouldBeTheSameAs(manifest);
        }
    }
}