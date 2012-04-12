namespace Bottles
{
    public interface IBottleManifestReader
    {
        IBottleInfo LoadFromFolder(string bottleDirectory);
    }
}