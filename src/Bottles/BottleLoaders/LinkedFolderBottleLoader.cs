using System;
using System.Collections.Generic;
using System.Linq;
using Bottles.Diagnostics;
using FubuCore;

namespace Bottles.BottleLoaders
{
    /// <summary>
    /// Wondering is this should be a Fubu specific thing. I think not, seems damn useful
    /// </summary>
    public class LinkedFolderBottleLoader : IBottleLoader
    {
        private readonly string _applicationDirectory;
        private readonly IFileSystem _fileSystem = new FileSystem();
        private readonly BottleManifestReader _reader;

        public LinkedFolderBottleLoader(string applicationDirectory, Func<string, string> getContentFolderFromBottleFolder)
        {
            _applicationDirectory = applicationDirectory;
            _reader = new BottleManifestReader(_fileSystem, getContentFolderFromBottleFolder);
        }


        public IEnumerable<IBottleInfo> Load(IBottleLog log)
        {
            var bottles = new List<IBottleInfo>();

            var manifestFile = FileSystem.Combine(_applicationDirectory, LinkManifest.FILE);
            var manifest = _fileSystem.LoadFromFile<LinkManifest>(manifestFile);
            if (manifest == null)
            {
                log.Trace("No bottle manifest found at {0}", manifestFile);
                return bottles;
            }

            if (manifest.LinkedFolders.Any())
            {
                log.Trace("Loading linked folders via the bottle manifest at " + _applicationDirectory);
                manifest.LinkedFolders.Each(folder =>
                {
                    var linkedFolder = FileSystem.Combine(_applicationDirectory, folder).ToFullPath();
                    log.Trace("  - linking folder " + linkedFolder);

                    var bottle = _reader.LoadFromFolder(linkedFolder);
                    bottles.Add(bottle);
                });
            }
            else
            {
                log.Trace("No linked folders found in the bottle manifest file at " + _applicationDirectory);
            }

            return bottles;
        }
    }
}