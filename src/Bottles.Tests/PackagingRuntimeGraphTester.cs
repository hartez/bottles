using System.Collections.Generic;
using Bottles.Diagnostics;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;
using Rhino.Mocks;

namespace Bottles.Tests
{
    [TestFixture]
    public class when_adding_an_bootstrapper : InteractionContext<BottleRuntimeGraph>
    {
        private StubBootstrapper bootstrapper1;
        private StubBootstrapper bootstrapper2;
        private StubBootstrapper bootstrapper3;

        protected override void beforeEach()
        {
            bootstrapper1 = new StubBootstrapper("a");
            bootstrapper2 = new StubBootstrapper("b");
            bootstrapper3 = new StubBootstrapper("c");

            ClassUnderTest.PushProvenance("A");
            ClassUnderTest.AddBootstrapper(bootstrapper1);

            ClassUnderTest.PushProvenance("B");
            ClassUnderTest.AddBootstrapper(bootstrapper2);

            ClassUnderTest.PopProvenance();
            ClassUnderTest.AddBootstrapper(bootstrapper3);
        }


        [Test]
        public void should_register_the_first_bootstrapper()
        {
            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogObject(bootstrapper1, "A"));
        }

        [Test]
        public void should_register_the_second_bootstrapper_with_nested_provenance()
        {
            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogObject(bootstrapper2, "B"));
        }

        [Test]
        public void should_register_the_third_bootstrapper_after_popping_the_provenance()
        {
            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogObject(bootstrapper3, "A"));
        }
    }


    [TestFixture]
    public class when_adding_an_activator : InteractionContext<BottleRuntimeGraph>
    {
        private StubActivator activator1;
        private StubActivator activator2;
        private StubActivator activator3;

        protected override void beforeEach()
        {
            activator1 = new StubActivator();
            activator2 = new StubActivator();
            activator3 = new StubActivator();

            ClassUnderTest.PushProvenance("A");
            ClassUnderTest.AddActivator(activator1);

            ClassUnderTest.PushProvenance("B");
            ClassUnderTest.AddActivator(activator2);

            ClassUnderTest.PopProvenance();
            ClassUnderTest.AddActivator(activator3);
        }


        [Test]
        public void should_register_the_first_activator()
        {
            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogObject(activator1, "A"));
        }

        [Test]
        public void should_register_the_second_activator_with_nested_provenance()
        {
            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogObject(activator2, "B"));
        }

        [Test]
        public void should_register_the_third_activator_after_popping_the_provenance()
        {
            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogObject(activator3, "A"));
        }
    }


    [TestFixture]
    public class when_adding_a_package_loader : InteractionContext<BottleRuntimeGraph>
    {
        private StubBottleLoader loader1;
        private StubBottleLoader loader2;
        private StubBottleLoader loader3;

        protected override void beforeEach()
        {
            loader1 = new StubBottleLoader("1a", "1b", "1c");
            loader2 = new StubBottleLoader("2a", "2b");
            loader3 = new StubBottleLoader("3a", "3b", "3c");
        
            ClassUnderTest.PushProvenance("A");
            ClassUnderTest.AddLoader(loader1);

            ClassUnderTest.PushProvenance("B");
            ClassUnderTest.AddLoader(loader2);

            ClassUnderTest.PopProvenance();
            ClassUnderTest.AddLoader(loader3);
        }

        [Test]
        public void should_register_the_first_loader()
        {
            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogObject(loader1, "A"));
        }

        [Test]
        public void should_register_the_second_loader_with_nested_provenance()
        {
            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogObject(loader2, "B"));
        }

        [Test] 
        public void should_register_the_third_loader_after_popping_the_provenance()
        {
            MockFor<IBottlingDiagnostics>().AssertWasCalled(x => x.LogObject(loader3, "A"));
        }
    }


    public class StubBootstrapper : IBootstrapper
    {
        private readonly string _name;
        private readonly IActivator[] _activators;

        public StubBootstrapper(string name, params IActivator[] activators)
        {
            _name = name;
            _activators = activators;
        }

        public IEnumerable<IActivator> Bootstrap(IBottleLog log)
        {
            return _activators;
        }

        public override string ToString()
        {
            return string.Format("Bootstrapper: {0}", _name);
        }
    }

    public class StubActivator : IActivator
    {
        private IEnumerable<IBottleInfo> _packages;
        private IBottleLog _log;

        public void Activate(IEnumerable<IBottleInfo> packages, IBottleLog log)
        {
            _packages = packages;
            _log = log;

            
        }

        public IEnumerable<IBottleInfo> Packages
        {
            get { return _packages; }
        }

        public IBottleLog Log
        {
            get { return _log; }
        }
    }
}