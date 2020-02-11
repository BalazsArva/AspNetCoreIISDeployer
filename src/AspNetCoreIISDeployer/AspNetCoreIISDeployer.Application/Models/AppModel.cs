namespace AspNetCoreIISDeployer.Application.Models
{
    public class AppModel
    {
        public string Id { get; set; }

        public int HttpPort { get; set; }

        public int HttpsPort { get; set; }

        public string AppPoolName { get; set; }

        public string BuildConfiguration { get; set; }

        public string CertificateThumbprint { get; set; }

        public string Environment { get; set; }

        public string ProjectPath { get; set; }

        public string PublishPath { get; set; }

        public string SiteName { get; set; }
    }
}