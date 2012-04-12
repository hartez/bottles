using Bottles.Diagnostics;

namespace Bottles.Exploding
{
    public class ExplodeDirectory
    {
        public string PackageDirectory { get; set;}
        public string DestinationDirectory { get; set; }
        public IBottleLog Log { get; set; }

        public bool Equals(ExplodeDirectory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.PackageDirectory, PackageDirectory) && Equals(other.DestinationDirectory, DestinationDirectory);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ExplodeDirectory)) return false;
            return Equals((ExplodeDirectory) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((PackageDirectory != null ? PackageDirectory.GetHashCode() : 0)*397) ^ (DestinationDirectory != null ? DestinationDirectory.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("PackageDirectory: {0}, DestinationDirectory: {1}", PackageDirectory, DestinationDirectory);
        }
    }
}