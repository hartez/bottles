using System;

namespace Bottles.Diagnostics
{
    public static class IBottleLogExtensions
    {
        public static void TrapErrors(this IBottleLog log, Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                log.MarkFailure(e);
            }
        }
    }
}