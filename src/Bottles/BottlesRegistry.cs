using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bottles.BottleLoaders.Assemblies;
using Bottles.Diagnostics;
using FubuCore;
using FubuCore.Reflection;

namespace Bottles
{
    public static class BottlesRegistry
    {
        private static readonly IList<Assembly> _assemblies = new List<Assembly>();
        private static readonly IList<IBottleInfo> _bottles = new List<IBottleInfo>();

        static BottlesRegistry()
        {
            /* 
             * This is a critical - KEY - concept
             * read up on it before making changes
             * http://msdn.microsoft.com/en-us/library/system.appdomain.assemblyresolve.aspx
             */
            AppDomain.CurrentDomain.AssemblyResolve += findAssemblyInLoadedBottles;
                

            GetApplicationDirectory = () => AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// Critical method in the bottles eco system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly findAssemblyInLoadedBottles(object sender, ResolveEventArgs args)
        {
            var theMissingAssemblyName = args.Name;
            return _assemblies.FirstOrDefault(assembly => theMissingAssemblyName == assembly.GetName().Name ||
                                                          theMissingAssemblyName == assembly.GetName().FullName);
        }

        //REVIEW: This really feels wrong - if its required can we make it an argument of LoadBottles("app dir", cfg=>{});
        public static Func<string> GetApplicationDirectory { get; set; }

        /// <summary>
        /// All the assemblies found in all packages
        /// </summary>
        public static IEnumerable<Assembly> PackageAssemblies
        {
            get { return _assemblies; }
        }

        /// <summary>
        /// Bottles that have been loaded
        /// </summary>
        public static IEnumerable<IBottleInfo> Bottles
        {
            get { return _bottles; }
        }

        /// <summary>
        /// The Diagnostics of the package environment
        /// </summary>
        public static BottlingDiagnostics Diagnostics { get; private set; }

        /// <summary>
        /// The entry method into the bottles environment
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="runActivators"></param>
        public static void LoadBottles(Action<IBottleFacility> configuration, bool runActivators = true)
            //consider renaming to InitializeEnvironment
            //have it return an environment object.
        {
            _bottles.Clear();

            Diagnostics = new BottlingDiagnostics();
            var record = new BottleLoadingRecord();

            Diagnostics.LogExecution(record, () =>
            {
                var facility = new BottleFacility();
                var assemblyLoader = new AssemblyLoader(Diagnostics);
                var graph = new BottleRuntimeGraph(Diagnostics, assemblyLoader, _bottles);

                var codeLocation = findCallToLoadBottles();
                graph.InProvenance(codeLocation, g =>
                {
                    //collect user configuration
                    configuration(facility);

                    //applies collected configurations
                    facility.Configure(g);
                });


                graph.DiscoverAndLoadBottles(() =>
                {
                    //clearing assemblies why? - my guess is testing.
                    // this should only really be called once.
                    _assemblies.Clear();

                    _assemblies.AddRange(assemblyLoader.Assemblies);
                    //the above assemblies are used when we need to resolve bottle assemblies
                }, runActivators);
            });

            record.Finished = DateTime.Now;
        }

        private static string findCallToLoadBottles()
        {
            var theBottleAssembly = typeof (IBottleInfo).Assembly; //bottle assembly
            var trace = new StackTrace(Thread.CurrentThread, false);

            //walk the stack looking for the first 'valid' frame to use
            for (var i = 0; i < trace.FrameCount; i++)
            {
                var frame = trace.GetFrame(i);
                var assembly = frame.GetMethod().DeclaringType.Assembly;

                if (assembly == theBottleAssembly) continue;
                if (!frame.GetMethod().HasAttribute<SkipOverForProvenanceAttribute>()) continue;


                return frame.ToDescription();
            }

            return "Unknown";
        }

        /// <summary>
        /// A static method that should be exposed, to allow you to 
        /// take an action when there has been a failure in the system.
        /// </summary>
        /// <param name="failure">The action to perform</param>
        public static void AssertNoFailures(Action failure)
        {
            if (Diagnostics.HasErrors())
            {
                failure();
            }
        }

        /// <summary>
        /// Default AssertNoFailures
        /// </summary>
        public static void AssertNoFailures()
        {
            AssertNoFailures(() =>
            {
                var writer = new StringWriter();
                writer.WriteLine("Package loading and application bootstrapping failed");
                writer.WriteLine();
                Diagnostics.EachLog((o, log) =>
                {
                    if (!log.Success)
                    {
                        writer.WriteLine(o.ToString());
                        writer.WriteLine(log.FullTraceText());
                        writer.WriteLine("------------------------------------------------------------------------------------------------");
                    }
                });

                throw new ApplicationException(writer.GetStringBuilder().ToString());
            });
        }
    }
}