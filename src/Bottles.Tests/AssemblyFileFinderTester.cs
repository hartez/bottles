using System;
using System.Linq;
using Bottles.BottleLoaders.Assemblies;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests
{
    [TestFixture]
    public class AssemblyFileFinderTester : InteractionContext<AssemblyFileFinder>
    {
        private string[] theAssemblyNames;
        private string[] theDebugFiles;
        private string theBinPath;
        private string[] theAssemblyFiles;

        protected override void beforeEach()
        {
            theAssemblyNames = new[]{"a", "b", "c", "d"};
            theDebugFiles = theAssemblyNames
                .Select(x => "{0}.pdb".ToFormat(x).ToFullPath())
                .ToArray();

            MockFor<IFileSystem>().Stub(x => x.FindFiles(theBinPath, FileSet.ForAssemblyDebugFiles(theAssemblyNames)))
                .Return(theDebugFiles);

            var root = System.IO.Path.GetPathRoot(AppDomain.CurrentDomain.BaseDirectory);
            theBinPath = root.AppendPath("bin");
        }

        private void theActualAssemblyFilesAre(params string[] files)
        {
            theAssemblyFiles = files;
            MockFor<IFileSystem>().Stub(x => x.FindFiles(theBinPath, FileSet.ForAssemblyNames(theAssemblyNames)))
                .Return(files);
            // The real FileSystem implemention will return all files in path if the Include specification is empty
            MockFor<IFileSystem>().Stub(x => x.FindFiles(theBinPath, new FileSet{DeepSearch = false, Include = ""}))
                .Return(files);
        }

        [Test]
        public void all_assembly_files_can_be_found()
        {
            theActualAssemblyFilesAre("a.dll", "b.exe", "c.dll", "D.exe");

            var files = ClassUnderTest.FindAssemblies(theBinPath, theAssemblyNames);
            files.Success.ShouldBeTrue();
            files.Files.ShouldBeTheSameAs(theAssemblyFiles);

            files.MissingAssemblies.Any().ShouldBeFalse();
        }

        [Test]
        public void not_all_assembly_files_can_be_found()
        {
            theActualAssemblyFilesAre("a.dll", "b.exe");

            var files = ClassUnderTest.FindAssemblies(theBinPath, theAssemblyNames);
            files.Success.ShouldBeFalse();
            files.Files.ShouldBeTheSameAs(theAssemblyFiles);

            files.MissingAssemblies.ShouldHaveTheSameElementsAs("c", "d");
        }

        [Test]
        public void should_have_the_pdb_files()
        {
            theActualAssemblyFilesAre("a.dll", "b.exe", "c.dll", "D.exe");

            var files = ClassUnderTest.FindAssemblies(theBinPath, theAssemblyNames);
            files.PdbFiles.ShouldBeTheSameAs(theDebugFiles);
        }

        [Test]
        public void should_not_find_any_assemblies_when_no_names_are_given()
        {
            theActualAssemblyFilesAre("a.dll", "b.exe", "c.dll", "D.exe");

            var files = ClassUnderTest.FindAssemblies(theBinPath, new string[0]);
            files.Success.ShouldBeTrue();
            files.Files.Any().ShouldBeFalse();
        }
    }
}