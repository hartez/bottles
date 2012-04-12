using System;
using System.Collections.Generic;
using Bottles.Deployment;
using Bottles.Deployment.Runtime;
using Bottles.Diagnostics;

namespace BottleDeployers1
{
    public static class DeploymentRecorder
    {
        public static List<IDirective> Directives = new List<IDirective>();
    }

    public abstract class StubDeployer<T> : IDeployer<T> where T : IDirective
    {
        public void Execute(T directive, HostManifest host, IBottleLog log)
        {
            DeploymentRecorder.Directives.Add(directive);
        }

        public string GetDescription(T directive)
        {
            return "something";
        }
    }

    public class OneDeployer : StubDeployer<OneDirective>{}
    public class TwoDeployer : StubDeployer<TwoDirective>{}
    public class ThreeDeployer : StubDeployer<ThreeDirective>{}

    public class OneDirective : IDirective
    {
        public OneDirective()
        {
            Name = "somebody";
        }

        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class TwoDirective : IDirective
    {
        public string City { get; set; }
        public bool IsDomestic { get; set; }
    }

    public class ThreeDirective : IDirective
    {
        public int Threshold { get; set; }
        public string Direction { get; set; }
    }
}