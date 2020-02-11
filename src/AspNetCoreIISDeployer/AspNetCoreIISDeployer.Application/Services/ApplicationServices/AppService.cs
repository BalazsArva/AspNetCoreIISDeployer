using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreIISDeployer.Application.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AspNetCoreIISDeployer.Application.Services.ApplicationServices
{
    public class AppService : IAppService
    {
        public async Task<AppListModel> GetAppsAsync(string globalConfigurationFilePath, string userOverridesFilePath)
        {
            var globalConfiguration = JsonConvert.DeserializeObject<List<AppModel>>(await File.ReadAllTextAsync(globalConfigurationFilePath));
            var globalConfigurationLookup = globalConfiguration.ToDictionary(x => x.Id);

            var userOverrides = JArray.Parse(await File.ReadAllTextAsync(userOverridesFilePath));

            // TODO: Error handling, missing Id, file not found, multiple entries with same id, etc.
            foreach (var appUserConfig in userOverrides.OfType<JObject>())
            {
                var appId = GetId(appUserConfig);
                var appGlobalConfig = globalConfigurationLookup.ContainsKey(appId) ? globalConfigurationLookup[appId] : null;

                if (appGlobalConfig is null)
                {
                    continue;
                }

                // TODO: Refactorings. Merge properties better, maybe handle PascalCase JSONs as well.
                appGlobalConfig.HttpPort = GetPropertyValueOrDefault<int>(appUserConfig, "httpPort", appGlobalConfig.HttpPort);
                appGlobalConfig.HttpsPort = GetPropertyValueOrDefault<int>(appUserConfig, "httpsPort", appGlobalConfig.HttpsPort);

                appGlobalConfig.AppPoolName = GetPropertyValueOrDefault<string>(appUserConfig, "appPoolName", appGlobalConfig.AppPoolName);
                appGlobalConfig.BuildConfiguration = GetPropertyValueOrDefault<string>(appUserConfig, "buildConfiguration", appGlobalConfig.BuildConfiguration);
                appGlobalConfig.CertificateThumbprint = GetPropertyValueOrDefault<string>(appUserConfig, "certificateThumbprint", appGlobalConfig.CertificateThumbprint);
                appGlobalConfig.Environment = GetPropertyValueOrDefault<string>(appUserConfig, "environment", appGlobalConfig.Environment);
                appGlobalConfig.ProjectPath = GetPropertyValueOrDefault<string>(appUserConfig, "projectPath", appGlobalConfig.ProjectPath);
                appGlobalConfig.PublishPath = GetPropertyValueOrDefault<string>(appUserConfig, "publishPath", appGlobalConfig.PublishPath);
                appGlobalConfig.SiteName = GetPropertyValueOrDefault<string>(appUserConfig, "siteName", appGlobalConfig.SiteName);
            }

            return new AppListModel { Apps = globalConfiguration };
        }

        private TProperty GetPropertyValueOrDefault<TProperty>(JObject jObject, string key, TProperty defaultValue)
        {
            JTokenType tokenType;

            if (typeof(TProperty) == typeof(int))
            {
                tokenType = JTokenType.Integer;
            }
            else if (typeof(TProperty) == typeof(string))
            {
                tokenType = JTokenType.String;
            }
            else
            {
                // TODO: Better exception
                throw new NotSupportedException($"Property type '{typeof(TProperty).Name}' is not supported.");
            }

            var property = jObject.Property(key);
            if (property != null && property.Value.Type == tokenType)
            {
                return property.Value.Value<TProperty>();
            }

            return defaultValue;
        }

        private string GetId(JToken app)
        {
            // TODO: Hadle when not found
            return app.Value<string>("id");

            /*
            var children = app.Children();

            foreach (var child in children)
            {
                if (child is JProperty appProperty)
                {
                    if (appProperty.Name == "id")
                    {
                        return appProperty.Value<string>();
                    }
                }
            }

            return null;
            */
        }
    }
}