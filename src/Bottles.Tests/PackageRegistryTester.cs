using System;
using System.Collections.Generic;
using FubuTestingSupport;
using NUnit.Framework;

namespace Bottles.Tests
{
    [TestFixture]
    public class PackageRegistryTester
    {
        [Test]
        public void assert_failures_with_no_failures()
        {
            BottlesRegistry.LoadBottles(x =>
            {
                x.Bootstrap(log =>
                {
                    return new List<IActivator>();
                });
            });

            BottlesRegistry.AssertNoFailures();
        }

        [Test]
        public void assert_failures_blows_up_when_anything_in_the_diagnostics_has_a_problem()
        {
            BottlesRegistry.LoadBottles(x =>
            {
                x.Bootstrap(log =>
                {
                    throw new ApplicationException("You shall not pass");
                });
            });

            Exception<ApplicationException>.ShouldBeThrownBy(() =>
            {
                BottlesRegistry.AssertNoFailures();
            }).Message.ShouldContain("You shall not pass");
        }

        [Test]
        public void should_run_activators()
        {
            bool ran = false;
            BottlesRegistry.LoadBottles(x =>
            {
                x.Bootstrap(log =>
                {
                    return new List<IActivator>(){new LambdaActivator("x",()=>
                    {
                        ran = true;
                    })};
                });
            });
            ran.ShouldBeTrue();
        }


        [Test]
        public void should_NOT_run_activators()
        {
            bool hasNotRun = true;
            BottlesRegistry.LoadBottles(x =>
            {
                x.Bootstrap(log =>
                {
                    return new List<IActivator>(){new LambdaActivator("x",()=>
                    {
                        hasNotRun = false;
                    })};
                });
            }, runActivators:false);
            hasNotRun.ShouldBeTrue();
        }
    }
}