using System;
using System.Collections.Generic;
using Bottles.Diagnostics;
using FubuCore.DependencyAnalysis;
using System.Linq;
using FubuCore;

namespace Bottles
{
    public class BottleDependencyProcessor
    {
        private readonly IEnumerable<IBottleInfo> _bottles;
        private readonly DependencyGraph<IBottleInfo> _graph;

        public BottleDependencyProcessor(IEnumerable<IBottleInfo> bottles)
        {
            _bottles = bottles;

            guardAgainstMalformedBottles();

            _graph = new DependencyGraph<IBottleInfo>(pak => pak.Name, pak => pak.GetDependencies().Select(x => x.Name));

            _bottles.OrderBy(p => p.Name).Each(p => _graph.RegisterItem(p));
        }

        public void LogMissingBottleDependencies(IBottlingDiagnostics diagnostics)
        {
            var missingDependencies = _graph.MissingDependencies();
            missingDependencies.Each(name =>
            {
                var dependentBottles = _bottles.Where(pak => pak.GetDependencies().Any(dep => dep.IsMandatory && dep.Name == name));
                dependentBottles.Each(pak => diagnostics.LogFor(pak).LogMissingDependency(name));
            });
        }

        public IEnumerable<IBottleInfo> OrderedBottles()
        {
            return _graph.Ordered();
        }

        private void guardAgainstMalformedBottles()
        {
            var missing = _bottles.Where(p => p.Name.IsEmpty());
            if (missing.Any())
            {
                var moduleTypes = missing.Select(x => x.Role).Join(", ");
                throw new ArgumentException("Bottles must have a name ({0}, {1})".ToFormat(missing.Count(), moduleTypes));
            }
        }
    }
}