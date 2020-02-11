using System.Collections.Generic;

namespace AspNetCoreIISDeployer.Application.Models
{
    public class AppListModel
    {
        public List<AppModel> Apps { get; set; } = new List<AppModel>();
    }
}