using AspNetCoreIISDeployer.Application.Models;

namespace AspNetCoreIISDeployer.Application.ViewModels.Factories
{
    public interface IAppViewModelFactory
    {
        AppViewModel Create(AppModel appModel);
    }
}