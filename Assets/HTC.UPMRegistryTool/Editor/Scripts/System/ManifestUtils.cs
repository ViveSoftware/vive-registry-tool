using HTC.UPMRegistryTool.Editor.Configs;
using HTC.Newtonsoft.Json;
using HTC.Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;

namespace HTC.UPMRegistryTool.Editor.System
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
            RemoveRegistry(manifestJson);
            if (!manifestJson.ContainsKey("scopedRegistries"))
            {
                manifestJson.Add("scopedRegistries", new JArray());
            }

            // Add registry
            IList<JToken> registries = (IList<JToken>) manifestJson["scopedRegistries"];
            JToken newToken = JToken.Parse(JsonConvert.SerializeObject(Settings.Instance().Registry));
            registries.Add(newToken);
            
            SaveProjectManifest(manifestJson.ToString());
        }

        public static void RemoveRegistryFromManifest()
        {
            JObject manifestJson = LoadProjectManifest();
            RemoveRegistry(manifestJson);
            SaveProjectManifest(manifestJson.ToString());
        }

        private static void RemoveRegistry(JObject json)
        {
            if (!json.ContainsKey("scopedRegistries"))
            {
                return;
            }

            IList<JToken> registries = (IList<JToken>) json["scopedRegistries"];
            for (int i = registries.Count - 1; i >= 0 ; i--)
            {
                JToken registryToken = registries[i];
                Settings.RegistryInfo registry = JsonConvert.DeserializeObject<Settings.RegistryInfo>(registryToken.ToString());
                if (registry.Name == Settings.Instance().Registry.Name)
                {
                    registries.RemoveAt(i);
                }
            }
        }

        private static JObject LoadProjectManifest()
        {
            string manifestString = File.ReadAllText(Settings.Instance().ProjectManifestPath);
            JObject manifestJson = JObject.Parse(manifestString);

            return manifestJson;
        }

        private static void SaveProjectManifest(string content)
        {
            File.WriteAllText(Settings.Instance().ProjectManifestPath, content);
        }
    }
}