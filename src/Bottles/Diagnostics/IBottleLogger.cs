using Bottles.BottleLoaders.Assemblies;
using Bottles.Creation;

namespace Bottles.Diagnostics
{
    public interface IBottleLogger
    {
        void WriteAssembliesNotFound(AssemblyFiles theAssemblyFiles, BottleManifest manifest, CreateBottleInput theInput, string binFolder);
    }
}