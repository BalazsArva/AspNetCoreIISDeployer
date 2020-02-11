using System.Security.Principal;
using AspNetCoreIISDeployer.Application.ViewModels.Factories;

namespace AspNetCoreIISDeployer.Application.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private AppListViewModel appList;

        public MainWindowViewModel(IAppListViewModelFactory appListViewModelFactory)
        {
            AppList = appListViewModelFactory.Create();
        }

        public AppListViewModel AppList
        {
            get { return appList; }
            set
            {
                if (appList == value)
                {
                    return;
                }

                appList = value;
                NotifyPropertyChanged(nameof(AppList));
            }
        }

        public string UserMode
        {
            get
            {
                return System.Security.Principal.WindowsIdentity.GetCurrent().Owner.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid)
                    ? "Admin"
                    : "Normal user";
            }
        }
    }
}