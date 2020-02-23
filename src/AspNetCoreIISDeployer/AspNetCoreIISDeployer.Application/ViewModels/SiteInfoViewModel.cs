namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class SiteInfoViewModel : ViewModelBase
    {
        private string certificateThumbprint = "";
        private string environment = "";

        public string CertificateThumbprint
        {
            get { return certificateThumbprint; }
            set
            {
                if (value == certificateThumbprint)
                {
                    return;
                }

                certificateThumbprint = value;
                NotifyPropertyChanged(nameof(CertificateThumbprint));
            }
        }

        public string Environment
        {
            get { return environment; }
            set
            {
                if (value == environment)
                {
                    return;
                }

                environment = value;
                NotifyPropertyChanged(nameof(Environment));
            }
        }
    }
}