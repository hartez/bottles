using Bottles.Diagnostics;

namespace Bottles.BottleLoaders.Assemblies
{
    /// <summary>
    /// Loads assemblies
    /// </summary>
    public interface IAssemblyLoader
    {
        void ReadBottle(IBottleInfo bottle, IBottleLog log);
    }
}