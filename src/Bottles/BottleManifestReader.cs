using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FubuCore;

namespace Bottles
{
    public class BottleManifestReader : IBottleManifestReader
    {
        private readonly IFileSystem _fileSystem;
        private readonly Func<string, string> _getContentFolderFromBottleFolder;

        public BottleManifestReader(IFileSystem fileSystem, Func<string, string> getContentFolderFromBottleFolder)
        {
            _fileSystem = fileSystem;
            _getContentFolderFromBottleFolder = getContentFolderFromBottleFolder;
        }

        public IBottleInfo LoadFromFolder(string bottleDirectory)
        {
            bottleDirectory = bottleDirectory.ToFullPath();

            var manifest = _fileSystem.LoadFromFile<BottleManifest>(bottleDirectory, BottleManifest.FILE);
            var bottle = new BottleInfo(manifest.Name){
                Description = "{0} ({1})".ToFormat(manifest.Name, bottleDirectory),
                Dependencies = manifest.Dependencies
            };

            // Right here, this needs to be different
            registerFolders(bottleDirectory, bottle);

            var binPath = determineBinPath(bottleDirectory);

            bottle.Role = manifest.Role;

            readAssemblyPaths(manifest, bottle, binPath);

            return bottle;
        }

        private string determineBinPath(string bottleDirectory)
        {
            var binPath = FileSystem.Combine(bottleDirectory, "bin");
            var debugPath = FileSystem.Combine(binPath, "debug");
            if (_fileSystem.DirectoryExists(debugPath))
            {
                binPath = debugPath;
            }
            return binPath;
        }

        private void registerFolders(string bottleDirectory, BottleInfo bottle)
        {
            bottle.RegisterFolder(CommonBottleFiles.WebContentFolder, _getContentFolderFromBottleFolder(bottleDirectory));
            bottle.RegisterFolder(CommonBottleFiles.DataFolder, FileSystem.Combine(bottleDirectory, CommonBottleFiles.DataFolder));
            bottle.RegisterFolder(CommonBottleFiles.ConfigFolder, FileSystem.Combine(bottleDirectory, CommonBottleFiles.ConfigFolder));
        }

        private void readAssemblyPaths(BottleManifest manifest, BottleInfo bottle, string binPath)
        {
            var assemblyPaths = findCandidateAssemblyFiles(binPath);
            assemblyPaths.Each(path =>
            {
                var assemblyName = Path.GetFileNameWithoutExtension(path);
                if (manifest.Assemblies.Contains(assemblyName))
                {
                    bottle.RegisterAssemblyLocation(assemblyName, path);
                }
            });
        }

        private static IEnumerable<string> findCandidateAssemblyFiles(string binPath)
        {
            if (!Directory.Exists(binPath))
            {
                return new string[0];
            }

            return Directory.GetFiles(binPath).Where(IsPotentiallyAnAssembly);
        }

        public static bool IsPotentiallyAnAssembly(string file)
        {
            var extension = Path.GetExtension(file);
            return extension.Equals(".exe", StringComparison.OrdinalIgnoreCase) ||
                   extension.Equals(".dll", StringComparison.OrdinalIgnoreCase);
        }

        public override string ToString()
        {
            return "Bottle Manifest Reader (Development Mode)";
        }
    }
}