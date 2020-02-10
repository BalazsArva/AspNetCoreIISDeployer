using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIISDeployer.Application.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public class AppService
    {
        /*
        public void WatchFolder(string path)
        {
            var watcher = new FileSystemWatcher(path);

            watcher.
        }
        */

            /*
        public async Task<AppListModel> GetAppsAsync(string globalConfigurationFilePath, string userOverridesFilePath)
        {
            var globalConfiguration = JsonConvert.DeserializeObject<List<AppModel>>(await File.ReadAllTextAsync(globalConfigurationFilePath));
            var userOverrides = JsonConvert
                .DeserializeObject<List<AppModel>>(await File.ReadAllTextAsync(userOverridesFilePath))
                .ToDictionary(x => x.Id);

            var userOverrides2 = JArray.Parse(await File.ReadAllTextAsync(userOverridesFilePath));

            foreach (var app in userOverrides2)
            {
                var children = app.Children();

                app.Children().Any(c => c.Path.EndsWith("/HttpPort"));

            }


            foreach (var app in globalConfiguration)
            {
                var app1 = 

                if (userOverrides.ContainsKey(app.Id))
                {
                    var userOverride = userOverrides[app.Id];

                }
            }

            return new AppListModel { Apps = globalConfiguration };
        }*/
    }
}