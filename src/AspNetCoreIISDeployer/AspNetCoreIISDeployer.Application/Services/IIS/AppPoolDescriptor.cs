namespace AspNetCoreIISDeployer.Application.Services.IIS
{
    public class AppPoolDescriptor
    {
        public AppPoolDescriptor(string name, AppPoolState state)
        {
            Name = name;
            State = state;
        }

        public string Name { get; }

        public AppPoolState State { get; }
    }
}