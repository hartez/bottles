using System;
using System.Collections.Generic;
using System.IO;
using Bottles.BottleLoaders.Assemblies;

namespace Bottles
{
    public interface IBottleInfo
    {
        string Name { get; }
        string Role { get; set; }
        void LoadAssemblies(IAssemblyRegistration loader);

        void ForFolder(string folderName, Action<string> onFound);
        void ForData(string searchPattern, Action<string, Stream> dataCallback);

        IEnumerable<Dependency> GetDependencies();
    }
}