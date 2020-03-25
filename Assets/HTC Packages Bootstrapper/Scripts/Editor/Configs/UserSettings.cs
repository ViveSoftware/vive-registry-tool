using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace HTC.PackagesBootstrapper.Editor.Configs
{
    public class UserSettings
    {
        private const string FilePath = "Temp/HTCPackagesBootstrapperUserSettings.json";
        private static UserSettings PrivateInstance;

        [JsonProperty]
        public long LastCheckTimestamp = 0;

        [JsonProperty]
        public bool AutoCheckEnabled = true;

        public static UserSettings Instance()
        {
            if (PrivateInstance == null)
            {
                if (File.Exists(FilePath))
                {
                    string settingString = File.ReadAllText(FilePath);
                    PrivateInstance = JsonConvert.DeserializeObject<UserSettings>(settingString);
                }
                else
                {
                    PrivateInstance = new UserSettings();
                    PrivateInstance.Save();
                }
            }

            return PrivateInstance;
        }

        public DateTime GetLastCheckTime()
        {
            return new DateTime(LastCheckTimestamp, DateTimeKind.Utc);
        }

        public void SetLastCheckTimestamp(DateTime dateTime)
        {
            LastCheckTimestamp = dateTime.Ticks;
            Save();
        }

        public void SetAutoCheckEnabled(bool enabled)
        {
            AutoCheckEnabled = enabled;
            Save();
        }

        private void Save()
        {
            string contentString = JsonConvert.SerializeObject(this);
            File.WriteAllText(FilePath, contentString);
        }
    }
}