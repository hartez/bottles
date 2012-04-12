using System.Collections.Generic;
using Bottles.Deployment.Runtime.Content;
using FubuCore;

namespace Bottles.Deployers.Iis
{
    public class WebsiteBottleDestination : IBottleDestination
    {
        private readonly string _physicalPath;

        public WebsiteBottleDestination(string physicalPath)
        {
            _physicalPath = physicalPath;            
        }

        public virtual IEnumerable<BottleExplosionRequest> DetermineExplosionRequests(BottleManifest manifest)
        {
            switch (manifest.Role)
            {
                case BottleRoles.Binaries:
                    yield return new BottleExplosionRequest
                                     {
                                         BottleDirectory = CommonBottleFiles.BinaryFolder,
                                         BottleName = manifest.Name,
                                         DestinationDirectory = FileSystem.Combine(_physicalPath, CommonBottleFiles.BinaryFolder)
                                     };
                    break;

                case BottleRoles.Config:
                    yield return new BottleExplosionRequest()
                                     {
                                         BottleDirectory = CommonBottleFiles.ConfigFolder,
                                         BottleName = manifest.Name,
                                         DestinationDirectory = FileSystem.Combine(_physicalPath, CommonBottleFiles.ConfigFolder)
                                     };
                    break;

                case BottleRoles.Module:                    
                    yield return new BottleExplosionRequest
                                     {
                                         BottleDirectory = CommonBottleFiles.BinaryFolder,
                                         BottleName = manifest.Name,
                                         DestinationDirectory = _physicalPath.AppendPath(CommonBottleFiles.BinaryFolder)
                                     };
                    break;

                case BottleRoles.Application:
                    yield return new BottleExplosionRequest
                                     {
                                         BottleName = manifest.Name,
                                         BottleDirectory = CommonBottleFiles.BinaryFolder,
                                         DestinationDirectory = FileSystem.Combine(_physicalPath, CommonBottleFiles.BinaryFolder)
                                     };

                    yield return new BottleExplosionRequest
                                     {
                                         BottleName = manifest.Name,
                                         BottleDirectory = CommonBottleFiles.WebContentFolder,
                                         DestinationDirectory = _physicalPath
                                     };

                    break;

                default:
                    yield break;
            }
        }
    }
}