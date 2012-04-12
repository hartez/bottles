using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bottles.Diagnostics;
using FubuCore;

namespace Bottles.BottleLoaders
{
    public class SolutionDirectoryBottleLoader : IBottleLoader
    {
        private readonly string _sourceRoot;

        public SolutionDirectoryBottleLoader(string sourceRoot)
        {
            _sourceRoot = sourceRoot;
        }

        public IEnumerable<IBottleInfo> Load(IBottleLog log)
        {
            var manifestReader = new BottleManifestReader(new FileSystem(), folder => folder);
            
            //how can i 'where' the manifests
               

            var pis = BottleManifest.FindManifestFilesInDirectory(_sourceRoot)
                .Select(Path.GetDirectoryName)
                .Select(manifestReader.LoadFromFolder);

            var filtered = pis.Where(pi=>BottleRoles.Module.Equals(pi.Role));

            LogWriter.Current.PrintHorizontalLine();
            LogWriter.Current.Trace("Solution Bottle Loader found:");
            LogWriter.Current.Indent(() =>
            {
                filtered.Each(p => LogWriter.Current.Trace(p.Name));
            });

            LogWriter.Current.PrintHorizontalLine();

            return filtered;
        }
    }
}