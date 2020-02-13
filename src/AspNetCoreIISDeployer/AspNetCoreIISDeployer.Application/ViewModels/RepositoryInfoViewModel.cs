namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class RepositoryInfoViewModel : GitInfoViewModelBase
    {
        private string remoteCommit = "";

        public string RemoteCommit
        {
            get { return remoteCommit; }
            set
            {
                if (value == remoteCommit)
                {
                    return;
                }

                remoteCommit = value;
                NotifyPropertyChanged(nameof(RemoteCommit));
            }
        }
    }
}