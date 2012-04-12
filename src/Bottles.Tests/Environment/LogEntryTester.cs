using Bottles.Diagnostics;
using Bottles.Environment;
using FubuTestingSupport;
using NUnit.Framework;

namespace Bottles.Tests.Environment
{
    [TestFixture]
    public class LogEntryTester
    {
        [Test]
        public void from_package_log_successful()
        {
            var packageLog = new BottleLog();
            packageLog.Trace("some stuff");
            packageLog.Success.ShouldBeTrue();

            var log = LogEntry.FromBottleLog(this, packageLog);
            log.Success.ShouldBeTrue();
            log.TraceText.ShouldEqual(packageLog.FullTraceText().Trim());
            log.Description.ShouldEqual(this.ToString());
        }

        [Test]
        public void from_package_log_failure()
        {
            var packageLog = new BottleLog();
            packageLog.Trace("some stuff");
            packageLog.MarkFailure("it broke");
            packageLog.Success.ShouldBeFalse();

            var log = LogEntry.FromBottleLog(this, packageLog);
            log.Success.ShouldBeFalse();
            log.TraceText.ShouldEqual(packageLog.FullTraceText().Trim());
            log.Description.ShouldEqual(this.ToString());
        }
    }
}