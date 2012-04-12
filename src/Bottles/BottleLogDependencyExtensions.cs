using Bottles.Diagnostics;
using FubuCore;

namespace Bottles
{
    public static class BottleLogDependencyExtensions
    {
        public static void LogMissingDependency(this IBottleLog log, string dependencyName)
        {
            log.MarkFailure("Missing required Bottle dependency named '{0}'".ToFormat(dependencyName));
        }
    }
}