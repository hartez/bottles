using System.Collections.Generic;
using System.Linq;
using Bottles.Deployers.Iis;
using Bottles.Deployment.Runtime.Content;
using Bottles.Diagnostics;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;

namespace Bottles.Tests.Deployment.Deployers
{
    [TestFixture]
    public class WebsiteBottleDestinationTester
    {
        private WebsiteBottleDestination theDestination;
        private string theRootFolder;

        [SetUp]
        public void SetUp()
        {
            theRootFolder = "deployedPath";
            theDestination = new WebsiteBottleDestination(theRootFolder);
        }

        [Test]
        public void create_request_for_binaries()
        {
            var request = theDestination.DetermineExplosionRequests(new BottleManifest()
            {
                Role = BottleRoles.Binaries,
                Name = "the bottle name"
            }).Single();

            request.BottleDirectory.ShouldEqual(CommonBottleFiles.BinaryFolder);
            request.DestinationDirectory.ShouldEqual(FileSystem.Combine(theRootFolder, CommonBottleFiles.BinaryFolder));
            request.BottleName.ShouldEqual("the bottle name");
        }

        [Test]
        public void create_request_for_config()
        {
            var request = theDestination.DetermineExplosionRequests(new BottleManifest()
            {
                Role = BottleRoles.Config,
                Name = "the bottle name"
            }).Single();

            request.BottleDirectory.ShouldEqual(CommonBottleFiles.ConfigFolder);
            request.DestinationDirectory.ShouldEqual(FileSystem.Combine(theRootFolder, CommonBottleFiles.ConfigFolder));
            request.BottleName.ShouldEqual("the bottle name");
        }

        [Test]
        public void create_request_for_module()
        {
            var requests = theDestination.DetermineExplosionRequests(new BottleManifest()
            {
                Role = BottleRoles.Module,
                Name = "the bottle name"
            });

            var secondRequet = requests.First();
            secondRequet.BottleDirectory.ShouldEqual(CommonBottleFiles.BinaryFolder);
            secondRequet.DestinationDirectory.ShouldEqual(theRootFolder.AppendPath(CommonBottleFiles.BinaryFolder));
        }

        [Test]
        public void create_requests_for_module()
        {
            var requests = theDestination.DetermineExplosionRequests(new BottleManifest()
            {
                Role = BottleRoles.Application,
                Name = "the bottle name"
            });

            var expected = new List<BottleExplosionRequest>{
                new BottleExplosionRequest(new BottleLog()){BottleName = "the bottle name", BottleDirectory = CommonBottleFiles.BinaryFolder, DestinationDirectory = FileSystem.Combine(theRootFolder, CommonBottleFiles.BinaryFolder)},
                new BottleExplosionRequest(new BottleLog()){BottleName = "the bottle name", BottleDirectory = CommonBottleFiles.WebContentFolder, DestinationDirectory = theRootFolder},
            };

            requests.ShouldHaveTheSameElementsAs(expected);
        }
    }
}