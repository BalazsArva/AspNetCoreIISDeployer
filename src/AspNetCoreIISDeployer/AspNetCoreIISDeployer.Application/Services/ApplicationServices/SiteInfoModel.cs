namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public class SiteInfoModel
    {
        public SiteInfoModel(string branch, string commit, string certificateThumbprint)
        {
            Branch = branch;
            Commit = commit;
            CertificateThumbprint = certificateThumbprint;
        }

        public string Branch { get; }

        public string Commit { get; }

        public string CertificateThumbprint { get; }
    }
}