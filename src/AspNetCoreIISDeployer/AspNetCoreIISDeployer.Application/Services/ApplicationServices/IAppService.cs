using System.Threading.Tasks;
using AspNetCoreIISDeployer.Application.Models;

namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public interface IAppService
    {
        Task<AppListModel> GetAppsAsync(string globalConfigurationFilePath, string userOverridesFilePath);
    }
}