using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HTC.PackagesBootstrapper.Editor.System
{
    public class Settings
    {
        public struct RegistryInfo
        {
            [JsonProperty("name")]
            public string Name;

            [JsonProperty("url")]
            public string Url;

            [JsonProperty("scopes")]
            public IList<string> Scopes;

            public bool Equals(RegistryInfo otherInfo)
            {
                if (Name != otherInfo.Name || Url != otherInfo.Url)
                {
                    return false;
                }

                if (Scopes == null || otherInfo.Scopes == null)
                {
                    return false;
                }

                if (Scopes.Count != otherInfo.Scopes.Count)
                {
                    return false;
                }

                for (int i = 0; i < Scopes.Count; i++)
                {
                    if (Scopes[i] != otherInfo.Scopes[i])
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        [JsonProperty("projectManifestPath")]
        public string ProjectManifestPath;

        [JsonProperty("checkIntervalSeconds")]
        public double CheckIntervalSeconds;

        [JsonProperty("registry")]
        public RegistryInfo Registry;

        private static Settings PrivateInstance;
        private const string SettingsPath = "Settings";

        public static Settings Instance()
        {
            if (PrivateInstance == null)
            {
                TextAsset settingsAsset = Resources.Load<TextAsset>(SettingsPath);
                PrivateInstance = JsonConvert.DeserializeObject<Settings>(settingsAsset.ToString());
            }

            return PrivateInstance;
        }

        public JObject LoadProjectManifest()
        {
            string manifestString = File.ReadAllText(ProjectManifestPath);
            JObject manifestJson = JObject.Parse(manifestString);

            return manifestJson;
        }

        public void WriteProjectManifest(string content)
        {
            File.WriteAllText(ProjectManifestPath, content);
        }
    }
}
