using System.IO;
using System.Linq;
using System.Reflection;
using AssemblyBottle;
using Bottles.BottleLoaders.Assemblies;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;
using FubuCore;

namespace Bottles.Tests
{
    [TestFixture]
    public class AssemblyPackageInfoTester
    {
        private Assembly assembly;
        private AssemblyBottleInfo bottle;

        [SetUp]
        public void SetUp()
        {
            assembly = Assembly.GetExecutingAssembly();
            bottle = AssemblyBottleInfo.CreateFor(assembly);
        }

        [Test]
        public void name_just_returns_the_assembly_name()
        {
            bottle.Name.ShouldEqual("Assembly:  " + assembly.GetName().FullName);
        }

        [Test]
        public void load_assemblies_just_tries_to_add_the_inner_assembly_directly()
        {
            var loader = MockRepository.GenerateMock<IAssemblyRegistration>();
            bottle.LoadAssemblies(loader);

            loader.AssertWasCalled(x => x.Use(assembly));
        }

        [Test]
        public void get_dependencies_is_empty_FOR_NOW()
        {
            bottle.GetDependencies().Any().ShouldBeFalse();
        }
    }

    [TestFixture]
    public class AssemblyPackageInfoIntegratedTester
    {
        private AssemblyBottleInfo _theBottle;

        [SetUp]
        public void SetUp()
        {
            new FileSystem().DeleteDirectory("content");
            _theBottle = AssemblyBottleInfo.CreateFor(typeof (AssemblyBottleMarker).Assembly);
        }


        [Test]
        public void can_retrieve_data_from_package()
        {
            var text = "not the right thing";
            _theBottle.ForData("1.txt", (name, data) =>
            {
                name.ShouldEqual("1.txt");
                text = new StreamReader(data).ReadToEnd();
            });

            // The text of this file in the AssemblyPackage data is just "1"
            text.ShouldEqual("1");
        }

        [Test]
        public void can_retrieve_web_content_folder_from_package()
        {
            var expected = "not this";
            _theBottle.ForFolder(CommonBottleFiles.WebContentFolder, folder =>
            {
                expected = folder;
            });

            expected.ShouldEqual(FileSystem.Combine("content", "AssemblyPackage", "WebContent").ToFullPath());
        }
    }
}