using System;
using System.Reflection;
using Bottles.BottleLoaders.Assemblies;
using Bottles.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace Bottles.Tests
{
    [TestFixture]
    public class when_loading_a_single_package : InteractionContext<AssemblyLoader>
    {
        protected override void beforeEach()
        {
            
        }

        [Test]
        public void uses_double_dispatch_to_let_a_package_use_itself_to_load_assemblies()
        {
            ClassUnderTest.LoadAssembliesFromBottle(MockFor<IBottleInfo>());
            MockFor<IBottleInfo>().AssertWasCalled(x => x.LoadAssemblies(ClassUnderTest));
        }

        [Test]
        public void log_successful_registration_of_an_assembly()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var package = new StubBottle("something"){
                LoadingAssemblies = x => x.Use(assembly)
            };

            ClassUnderTest.LoadAssembliesFromBottle(package);

            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogAssembly(package, assembly, AssemblyLoader.DIRECTLY_REGISTERED_MESSAGE));

        
            ClassUnderTest.Assemblies.Contains(assembly).ShouldBeTrue();
        }

        [Test]
        public void log_duplicate_registration_of_an_assembly()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var package1 = new StubBottle("something")
            {
                LoadingAssemblies = x => x.Use(assembly)
            };

            var package2 = new StubBottle("something")
            {
                LoadingAssemblies = x => x.Use(assembly)
            };

            ClassUnderTest.LoadAssembliesFromBottle(package1);
            ClassUnderTest.LoadAssembliesFromBottle(package2);

            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogDuplicateAssembly(package2, assembly.FullName));
        }

        [Test]
        public void load_successfully_from_file_for_a_new_assembly()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var assemblyFileLoader = MockFor<Func<string, Assembly>>();

            ClassUnderTest.AssemblyFileLoader = assemblyFileLoader;

            var package = new StubBottle("something")
            {
                LoadingAssemblies = x => x.LoadFromFile("filename.dll", "AssemblyName")
            };

            assemblyFileLoader.Expect(x => x.Invoke("filename.dll")).Return(assembly);

            ClassUnderTest.LoadAssembliesFromBottle(package);

            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogAssembly(package, assembly, "Loaded from filename.dll"));
            assemblyFileLoader.VerifyAllExpectations();

            ClassUnderTest.Assemblies.Contains(assembly).ShouldBeTrue();
        }



        [Test]
        public void load_unsuccessfully_from_file_for_a_new_assembly()
        {
            var assemblyFileLoader = MockFor<Func<string, Assembly>>();

            ClassUnderTest.AssemblyFileLoader = assemblyFileLoader;

            var package = new StubBottle("something")
            {
                LoadingAssemblies = x => x.LoadFromFile("filename.dll", "AssemblyName")
            };

            var theExceptionFromAssemblyLoading = new ApplicationException("You shall not pass!");
            assemblyFileLoader.Expect(x => x.Invoke("filename.dll")).Throw(theExceptionFromAssemblyLoading);

            ClassUnderTest.LoadAssembliesFromBottle(package);

            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogAssemblyFailure(package, "filename.dll", theExceptionFromAssemblyLoading));

        }


        [Test]
        public void load_duplicate_assembly_attempt_from_file_for_a_new_assembly()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var package1 = new StubBottle("something")
            {
                LoadingAssemblies = x => x.Use(assembly)
            };
            ClassUnderTest.LoadAssembliesFromBottle(package1);

            var assemblyFileLoader = MockFor<Func<string, Assembly>>();

            ClassUnderTest.AssemblyFileLoader = assemblyFileLoader;

            var theAssemblyName = assembly.GetName().Name;
            var package = new StubBottle("something")
            {
                LoadingAssemblies = x => x.LoadFromFile("filename.dll", theAssemblyName)
            };

            assemblyFileLoader.Expect(x => x.Invoke("filename.dll")).Return(assembly);

            ClassUnderTest.LoadAssembliesFromBottle(package);

            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogDuplicateAssembly(package, theAssemblyName));


            ClassUnderTest.Assemblies.Count.ShouldEqual(1);
        }
    }

    
}