using System;
using System.Collections.Generic;
using System.Linq;
using Bottles.BottleLoaders.Assemblies;
using Bottles.Diagnostics;
using FubuCore;

namespace Bottles
{
    /// <summary>
    /// A collection of data about a given runtime
    /// </summary>
    public class BottleRuntimeGraph : IDisposable
    {
        private readonly IList<IActivator> _activators = new List<IActivator>();
        private readonly IAssemblyLoader _assemblies;
        private readonly IList<IBootstrapper> _bootstrappers = new List<IBootstrapper>();
        private readonly IBottlingDiagnostics _diagnostics;
        private readonly IList<IBottleLoader> _bottleLoaders = new List<IBottleLoader>();
        private readonly Stack<string> _provenanceStack = new Stack<string>();
        private readonly IList<IBottleInfo> _bottles;

        public BottleRuntimeGraph(IBottlingDiagnostics diagnostics, IAssemblyLoader assemblies, IList<IBottleInfo> bottles)
        {
            _diagnostics = diagnostics;
            _assemblies = assemblies;
        	_bottles = bottles;
        }


        public IDisposable InProvenance(string provenance, Action<BottleRuntimeGraph> action)
        {
            PushProvenance(provenance);
            action(this);
            return this;
        }

        public void Dispose()
        {
            PopProvenance();
        }

        public void PushProvenance(string provenance)
        {
            _provenanceStack.Push(provenance);
        }


        public void PopProvenance()
        {
            _provenanceStack.Pop();
        }

        //I kinda want this method elsewhere
        public void DiscoverAndLoadBottles(Action onAssembliesScanned, bool runActivators = true)
        {
            var allBottles = findAllBottles();

            //orders _bottles
            analyzePackageDependenciesAndOrder(allBottles);

            loadAssemblies(_bottles, onAssembliesScanned);
            var discoveredActivators = collectAllActivatorsFromBootstrappers();

            if(runActivators)
            {
                activateBottles(_bottles, discoveredActivators);    
            }
        }

        private void analyzePackageDependenciesAndOrder(IEnumerable<IBottleInfo> bottles)
        {
            var dependencyProcessor = new BottleDependencyProcessor(bottles);
            dependencyProcessor.LogMissingBottleDependencies(_diagnostics);
            _bottles.AddRange(dependencyProcessor.OrderedBottles());
        }

        private void activateBottles(IList<IBottleInfo> bottles, IList<IActivator> discoveredActivators)
        {
            var discoveredPlusRegisteredActivators = discoveredActivators.Union(_activators);
            _diagnostics.LogExecutionOnEach(discoveredPlusRegisteredActivators, (activator, log) => activator.Activate(bottles, log));
        }

        public void AddBootstrapper(IBootstrapper bootstrapper)
        {
            _bootstrappers.Add(bootstrapper);
            _diagnostics.LogObject(bootstrapper, currentProvenance);
        }

        public void AddLoader(IBottleLoader loader)
        {
            _bottleLoaders.Add(loader);
            _diagnostics.LogObject(loader, currentProvenance);
        }

        public void AddActivator(IActivator activator)
        {
            _activators.Add(activator);
            _diagnostics.LogObject(activator, currentProvenance);
        }

        public void AddFacility(IBottleFacility facility)
        {
            _diagnostics.LogObject(facility, currentProvenance);

            PushProvenance(facility.ToString());
            
            facility.As<IBottleRuntimeGraphConfigurer>().Configure(this);
            PopProvenance();
        }

        private string currentProvenance
        {
            get { return _provenanceStack.Peek(); }
        }

        private List<IActivator> collectAllActivatorsFromBootstrappers()
        {
            var result = new List<IActivator>();
            
            _diagnostics.LogExecutionOnEach(_bootstrappers, (currentBootstrapper, log) =>
            {
                var bootstrapperActivators = currentBootstrapper.Bootstrap(log);
                result.AddRange(bootstrapperActivators);
                _diagnostics.LogBootstrapperRun(currentBootstrapper, bootstrapperActivators);
            });

            return result;
        }

        private void loadAssemblies(IEnumerable<IBottleInfo> bottles, Action onAssembliesScanned)
        {
            _diagnostics.LogExecutionOnEach(bottles, _assemblies.ReadBottle);

            onAssembliesScanned();
        }

        private IEnumerable<IBottleInfo> findAllBottles()
        {
            var result = new List<IBottleInfo>();

            _diagnostics.LogExecutionOnEach(_bottleLoaders, (currentLoader, log) =>
            {
                var packageInfos = currentLoader.Load(log).ToArray();
                _diagnostics.LogBottles(currentLoader, packageInfos);

                result.AddRange(packageInfos);
            });

            return result;
        }
    }
}