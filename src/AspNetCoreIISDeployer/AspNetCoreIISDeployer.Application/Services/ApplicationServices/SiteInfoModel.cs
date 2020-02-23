namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public class SiteInfoModel
    {
        public SiteInfoModel(string environment, string branch, string commit, string certificateThumbprint)
        {
            Environment = environment;
            Branch = branch;
            Commit = commit;
            CertificateThumbprint = certificateThumbprint;
        }

        public string Environment { get; }

        public string Branch { get; }

        public string Commit { get; }

        public string CertificateThumbprint { get; }
    }
}