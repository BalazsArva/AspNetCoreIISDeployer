namespace AspNetCoreIISDeployer.Application.Services.IIS
{
    public class SiteDescriptor
    {
        public SiteDescriptor(string name, SiteState state)
        {
            Name = name;
            State = state;
        }

        public string Name { get; }

        public SiteState State { get; }
    }
}