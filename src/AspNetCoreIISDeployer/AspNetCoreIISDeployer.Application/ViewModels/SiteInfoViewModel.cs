namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class SiteInfoViewModel : ViewModelBase
    {
        private string certificateThumbprint = "";

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
    }
}