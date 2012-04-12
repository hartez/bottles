using System;
using System.Collections.Generic;
using System.Reflection;

namespace Bottles.Diagnostics
{
    public interface IBottlingDiagnostics
    {
        void LogObject(object target, string provenance);
        void LogBottles(IBottleInfo bottle, IBottleLoader loader);
        void LogBootstrapperRun(IBootstrapper bootstrapper, IEnumerable<IActivator> activators);
        void LogAssembly(IBottleInfo bottle, Assembly assembly, string provenance);
        void LogDuplicateAssembly(IBottleInfo bottle, string assemblyName);
        void LogAssemblyFailure(IBottleInfo bottle, string fileName, Exception exception);
        void LogExecution(object target, Action continuation);
        IBottleLog LogFor(object target);
    }
}