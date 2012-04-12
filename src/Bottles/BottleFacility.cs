using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Bottles.BottleLoaders.Assemblies;
using Bottles.Diagnostics;
using FubuCore;
using FubuCore.Reflection;

namespace Bottles
{
    /// <summary>
    /// Configuration DSL layer
    /// </summary>
    public class BottleFacility : IBottleFacility, IBottleRuntimeGraphConfigurer
    {
        private readonly IList<Action<BottleRuntimeGraph>> _configurableActions = new List<Action<BottleRuntimeGraph>>();


        public void Assembly(Assembly assembly)
        {
            addConfigurableAction(g => g.AddLoader(new AssemblyBottleLoader(assembly)));
        }

        public void Bootstrapper(IBootstrapper bootstrapper)
        {
           addConfigurableAction(g => g.AddBootstrapper(bootstrapper));
        }

        public void Loader(IBottleLoader loader)
        {
           addConfigurableAction(g => g.AddLoader(loader));
        }

        public void Facility(IBottleFacility facility)
        {
            addConfigurableAction(graph =>
            {
                graph.AddFacility(facility);
            });
        }

        public void Activator(IActivator activator)
        {
           addConfigurableAction(g => g.AddActivator(activator));
        }

        public void Bootstrap(Func<IBottleLog, IEnumerable<IActivator>> lambda)
        {
            var lambdaBootstrapper = new LambdaBootstrapper(lambda);
            lambdaBootstrapper.Provenance = findCallToBootstrapper();

            Bootstrapper(lambdaBootstrapper);
        }

        private static string findCallToBootstrapper()
        {
            var bottleAssembly = typeof(IBottleInfo).Assembly;
            var trace = new StackTrace(Thread.CurrentThread, false);
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;
                if (assembly != bottleAssembly && !frame.GetMethod().HasAttribute<SkipOverForProvenanceAttribute>())
                {
                    return frame.ToDescription();
                }
            }


            return "Unknown";
        }

        public void Configure(BottleRuntimeGraph graph)
        {
            _configurableActions.Each(cfgAction => cfgAction(graph));
        }

        private void addConfigurableAction(Action<BottleRuntimeGraph> action)
        {
            _configurableActions.Add(action);
        }
    }
}