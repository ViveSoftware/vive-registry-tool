using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HTC.PackagesBootstrapper.Editor.Configs
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

        private const string FilePath = "Assets/HTC Packages Bootstrapper/Editor/Resources/Settings.json";
        private static Settings PrivateInstance;

        [JsonProperty("projectManifestPath")]
        public string ProjectManifestPath;

        [JsonProperty("checkIntervalSeconds")]
        public double CheckIntervalSeconds;

        [JsonProperty("registry")]
        public RegistryInfo Registry;

        public static Settings Instance()
        {
            if (PrivateInstance == null)
            {
                if (File.Exists(FilePath))
                {
                    string settingString = File.ReadAllText(FilePath);
                    PrivateInstance = JsonConvert.DeserializeObject<Settings>(settingString);
                }
                else
                {
                    Debug.LogErrorFormat("Settings.json not found. ({0})", FilePath);
                    PrivateInstance = new Settings();
                }
            }

            return PrivateInstance;
        }
    }
}
