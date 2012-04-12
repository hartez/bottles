using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Bottles.Exploding;
using FubuCore;

namespace Bottles.BottleLoaders.Assemblies
{
    /// <summary>
    /// Reperesents a bottle that is contained in a .dll
    /// </summary>
    [DebuggerDisplay("{Name}")]
    public class AssemblyBottleInfo : IBottleInfo
    {
        private readonly Assembly _assembly;
        private readonly BottleFiles _files = new BottleFiles();

        private AssemblyBottleInfo(Assembly assembly)
        {
            _assembly = assembly;
        }

        public static AssemblyBottleInfo CreateFor(Assembly assembly)
        {
            var bottle = new AssemblyBottleInfo(assembly);
            var exploder = BottleExploder.GetBottleExploder(new FileSystem());
            
            exploder.ExplodeAssembly(BottlesRegistry.GetApplicationDirectory(), assembly, bottle.Files);

            return bottle;
        }

        // TODO -- remove duplication with AssemblyLoader
        public static AssemblyBottleInfo CreateFor(string fileName)
        {
            var assembly = loadBottleAssemblyFromAppBinPath(fileName);
            return CreateFor(assembly);
        }

        private static Assembly loadBottleAssemblyFromAppBinPath(string file)
        {
            var assemblyName = Path.GetFileNameWithoutExtension(file);
            var appBinPath = AppDomain.CurrentDomain.SetupInformation.PrivateBinPath ?? AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (!Path.GetDirectoryName(file).EqualsIgnoreCase(appBinPath))
            {
                var destFileName = FileSystem.Combine(appBinPath, Path.GetFileName(file));
                if (shouldUpdateFile(file, destFileName))
                {
                    File.Copy(file, destFileName, true);
                }
            }
            return Assembly.Load(assemblyName);
        }

        private static bool shouldUpdateFile(string source, string destination)
        {
            return !File.Exists(destination) || File.GetLastWriteTimeUtc(source) > File.GetLastWriteTimeUtc(destination);
        }

        public BottleFiles Files
        {
            get { return _files; }
        }

        public string Role { get; set; }

        public string Name
        {
            get { return "Assembly:  " + _assembly.FullName; }
        }

        public void LoadAssemblies(IAssemblyRegistration loader)
        {
            loader.Use(_assembly);
        }

        public void ForFolder(string folderName, Action<string> onFound)
        {
            _files.ForFolder(folderName, onFound);
        }

        public void ForData(string searchPattern, Action<string, Stream> dataCallback)
        {
            _files.ForData(searchPattern, dataCallback);
        }

        public IEnumerable<Dependency> GetDependencies()
        {
            yield break;
        }
    }
}