namespace AspNetCoreIISDeployer.Application.Configuration
{
    public class IISMangementConfiguration
    {
        public string AppCmdPath { get; set; } = @"C:\Windows\system32\inetsrv\appcmd.exe";

        public string NetShPath { get; set; } = @"C:\Windows\System32\netsh.exe";
    }
}