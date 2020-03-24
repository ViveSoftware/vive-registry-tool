using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace HTC.PackagesBootstrapper.Editor.System
{
    public static class ManifestUtils
    {
        public static bool CheckRegistryExists()
        {
            JObject manifestJson = LoadProjectManifest();
            if (!manifestJson.ContainsKey("scopedRegistries"))
            {
                return false;
            }

            IList<JToken> registries = (IList<JToken>) manifestJson["scopedRegistries"];
            foreach (JToken registryToken in registries)
            {
                Settings.RegistryInfo registry = JsonConvert.DeserializeObject<Settings.RegistryInfo>(registryToken.ToString());
                if (Settings.Instance().Registry.Equals(registry))
                {
                    return true;
                }
            }

            return false;
        }

        public static void UpdateRegistryToManifest()
        {
            JObject manifestJson = LoadProjectManifest();
            if (!manifestJson.ContainsKey("scopedRegistries"))
            {
                manifestJson.Add("scopedRegistries", new JArray());
            }

            IList<JToken> registries = (IList<JToken>) manifestJson["scopedRegistries"];

            // Remove all old registries from HTC
            for (int i = registries.Count - 1; i >= 0 ; i--)
            {
                JToken registryToken = registries[i];
                Settings.RegistryInfo registry = JsonConvert.DeserializeObject<Settings.RegistryInfo>(registryToken.ToString());
                if (registry.Name == Settings.Instance().Registry.Name)
                {
                    registries.RemoveAt(i);
                }
            }

            // Add registry
            JToken newToken = JToken.Parse(JsonConvert.SerializeObject(Settings.Instance().Registry));
            registries.Add(newToken);
            
            WriteProjectManifest(manifestJson.ToString());
        }

        private static JObject LoadProjectManifest()
        {
            string manifestString = File.ReadAllText(Settings.Instance().ProjectManifestPath);
            JObject manifestJson = JObject.Parse(manifestString);

            return manifestJson;
        }

        private static void WriteProjectManifest(string content)
        {
            File.WriteAllText(Settings.Instance().ProjectManifestPath, content);
        }
    }
}