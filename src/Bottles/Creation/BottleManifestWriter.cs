using System;
using FubuCore;

namespace Bottles.Creation
{
    public class BottleManifestWriter
    {
        private readonly IFileSystem _fileSystem;
        private BottleManifest _manifest;

        public BottleManifestWriter(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        //REVIEW: why is this reading?
        public void ReadFrom(string fileName, Action<BottleManifest> onCreation)
        {
            if (_fileSystem.FileExists(fileName))
            {
                _manifest = _fileSystem.LoadFromFile<BottleManifest>(fileName);
            }
            else
            {
                _manifest = new BottleManifest();
                onCreation(_manifest);
            }
        }

        public BottleManifest Manifest
        {
            get { return _manifest; }
        }

        public void WriteTo(string fileName)
        {
            _fileSystem.WriteObjectToFile(fileName, _manifest);
        }

        public void AddAssembly(string assemblyName)
        {
            Manifest.AddAssembly(assemblyName);
        }
    }
}