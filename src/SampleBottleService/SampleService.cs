﻿using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using Bottles.Host.Packaging;

namespace SampleBottleService
{
    public class SampleService :
        IBottleAwareService
    {
        public IEnumerable<IActivator> Bootstrap(IBottleLog log)
        {
            //boot up IOC 

            yield return new TestActivator();
        }

        public void Stop()
        {
            //MEH
        }
    }
}
