using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using FubuCore;

namespace Bottles.Diagnostics
{
    public class BottlingDiagnostics : LoggingSession, IBottlingDiagnostics
    {
        public void LogBottles(IBottleInfo bottle, IBottleLoader loader)
        {
            LogObject(bottle, "Loaded by " + loader);
            LogFor(loader).AddChild(bottle);
        }

        public void LogBootstrapperRun(IBootstrapper bootstrapper, IEnumerable<IActivator> activators)
        {
            var provenance = "Loaded by Bootstrapper:  " + bootstrapper;
            var bootstrapperLog = LogFor(bootstrapper);

            activators.Each(a =>
            {
                LogObject(a, provenance);
                bootstrapperLog.AddChild(a);
            });
        }

        public void LogAssembly(IBottleInfo bottle, Assembly assembly, string provenance)
        {
            try
            {
                var versionInfo = getVersion(assembly);
                
                
                LogObject(assembly, provenance);
                var packageLog = LogFor(bottle);
                packageLog.Trace("Loaded assembly '{0}' v{1}".ToFormat(assembly.GetName().FullName,versionInfo.FileVersion));
                packageLog.AddChild(assembly);
            }
            catch (Exception ex)
            {
                throw new Exception("Trying to log assembly '{0}' in bottle '{1}' at {2}".ToFormat(assembly.FullName, bottle.Name, assembly.Location), ex);
            }
        }

        private static FileVersionInfo getVersion(Assembly assembly)
        {
            try
            {
                return FileVersionInfo.GetVersionInfo(assembly.Location);
            }
            catch (Exception)
            {
                //grrr
                //blowing up at the moment
                return (FileVersionInfo)Activator.CreateInstance(typeof (FileVersionInfo), BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.CreateInstance, null, new object[]{"name"}, null);
            }
        }

        // just in log to bottle
        public void LogDuplicateAssembly(IBottleInfo bottle, string assemblyName)
        {
            LogFor(bottle).Trace("Assembly '{0}' was ignored because it is already loaded".ToFormat(assemblyName));
        }

        public void LogAssemblyFailure(IBottleInfo bottle, string fileName, Exception exception)
        {
            var log = LogFor(bottle);
            log.MarkFailure(exception);
            log.Trace("Failed to load assembly at '{0}'".ToFormat(fileName));
        }



        // TODO -- think about this little puppy
        public static string GetTypeName(object target)
        {
            if (target is IBootstrapper) return typeof (IBootstrapper).Name;
            if (target is IActivator) return typeof (IActivator).Name;
            if (target is IBottleLoader) return typeof (IBottleLoader).Name;
            if (target is IBottleFacility) return typeof (IBottleFacility).Name;
            if (target is IBottleInfo) return typeof (IBottleInfo).Name;
            if (target is Assembly) return typeof (Assembly).Name;

            return target.GetType().Name;
        }
    }
}