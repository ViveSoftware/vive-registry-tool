using HTC.PackagesBootstrapper.Editor.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;

namespace HTC.PackagesBootstrapper.Editor.System
{
    [InitializeOnLoad]
    public class RegistryCheck
    {
        private const string LastCheckTimestampPath = "Temp/.HTCRegistryLastCheckTimestamp";

        private static DateTime LastCheckTimestamp;

        static RegistryCheck()
        {
            ReadTimestampFile();
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            double elapsedSeconds = (DateTime.UtcNow - LastCheckTimestamp).TotalSeconds;
            if (elapsedSeconds < Settings.Instance().CheckIntervalSeconds)
            {
                return;
            }

            if (!CheckRegistryExists())
            {
                RegistryUpdaterWindow.Open();
            }

            WriteTimestampFileNow();
        }

        private static bool CheckRegistryExists()
        {
            JObject manifestJson = Settings.Instance().LoadProjectManifest();
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

        private static void WriteTimestampFileNow()
        {
            LastCheckTimestamp = DateTime.UtcNow;

            long timestamp = LastCheckTimestamp.Ticks;
            File.WriteAllText(LastCheckTimestampPath, timestamp.ToString());
        }

        private static void ReadTimestampFile()
        {
            if (!File.Exists(LastCheckTimestampPath))
            {
                LastCheckTimestamp = new DateTime();
                return;
            }

            string timestampString = File.ReadAllText(LastCheckTimestampPath);
            long seconds = long.Parse(timestampString);

            LastCheckTimestamp = new DateTime(seconds, DateTimeKind.Utc);
        }
    }
}