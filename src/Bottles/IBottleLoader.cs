using System.Collections.Generic;
using Bottles.Diagnostics;

namespace Bottles
{
    public interface IBottleLoader
    {
        IEnumerable<IBottleInfo> Load(IBottleLog log);
    }
}