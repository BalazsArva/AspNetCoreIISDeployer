using System;

namespace AspNetCoreIISDeployer.Application.Services.DotNet
{
    public class DotNetVersion
    {
        public DotNetVersion(int major, int minor)
        {
            Major = major;
            Minor = minor;
        }

        public int Major { get; }

        public int Minor { get; }

        public bool IsCompatible(string versionString)
        {
            if (versionString is null)
            {
                throw new ArgumentNullException(nameof(versionString));
            }

            return versionString.StartsWith($"{Major}.{Minor}");
        }

        public override string ToString()
        {
            return $"{Major}.{Minor}";
        }
    }
}