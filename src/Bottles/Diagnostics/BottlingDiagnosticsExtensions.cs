using System;
using System.Collections.Generic;

namespace Bottles.Diagnostics
{
    public static class BottlingDiagnosticsExtensions
    {
        public static void LogExecutionOnEach<TItem>(this IBottlingDiagnostics diagnostics, IEnumerable<TItem> targets, Action<TItem, IBottleLog> continuation)
        {
            targets.Each(currentTarget =>
            {
                var log = diagnostics.LogFor(currentTarget);
                diagnostics.LogExecution(currentTarget, () => continuation(currentTarget, log));
            });
        }

        public static void LogBottles(this IBottlingDiagnostics diagnostics, IBottleLoader loader, IEnumerable<IBottleInfo> bottles)
        {
            bottles.Each(p =>
            {
                diagnostics.LogBottles(p, loader);
            });
        }
    }
}