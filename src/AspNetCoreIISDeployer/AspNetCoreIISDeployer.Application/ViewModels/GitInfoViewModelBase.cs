namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public abstract class GitInfoViewModelBase : ViewModelBase
    {
        private string branch = "";
        private string commit = "";

        public string Branch
        {
            get { return branch; }
            set
            {
                if (value == branch)
                {
                    return;
                }

                branch = value;
                NotifyPropertyChanged(nameof(Branch));
            }
        }

        public string Commit
        {
            get { return commit; }
            set
            {
                if (value == commit)
                {
                    return;
                }

                commit = value;
                NotifyPropertyChanged(nameof(Commit));
            }
        }
    }
}