using HTC.Newtonsoft.Json;
using HTC.Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
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

        private const string FilePath = "Assets/HTC Package Registry/Editor/Resources/Settings.json";
        private static Settings PrivateInstance;

        [JsonProperty("projectManifestPath")]
        public string ProjectManifestPath;

        [JsonProperty("registry")]
        public RegistryInfo Registry;

        public string RegistryHost;
        public int RegistryPort;

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

                PrivateInstance.Init();
            }

            return PrivateInstance;
        }

        private void Init()
        {
            Match match = Regex.Match(Registry.Url, @"^https?:\/\/(.+?)(?::(\d+))?\/?$");
            RegistryHost = match.Groups[1].Value;

            int port = 0;
            RegistryPort = 80;
            if (Int32.TryParse(match.Groups[2].Value, out port))
            {
                RegistryPort = port;
            }
        }
    }
}
