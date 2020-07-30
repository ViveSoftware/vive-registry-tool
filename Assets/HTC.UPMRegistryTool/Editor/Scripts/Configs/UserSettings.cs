using HTC.Newtonsoft.Json;
using System.IO;

namespace HTC.UPMRegistryTool.Editor.Configs
{
    public class UserSettings
    {
        private const string FilePath = "UserSettings/HTCUPMRegistryToolUserSettings.json";
        private static UserSettings PrivateInstance;

        [JsonProperty]
        public bool AutoCheckEnabled = true;

        [JsonProperty] 
        public bool TermsAccepted;

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

        public void SetAutoCheckEnabled(bool enabled)
        {
            AutoCheckEnabled = enabled;
            Save();
        }

        public void SetTermsAccepted(bool accepted)
        {
            TermsAccepted = accepted;
            Save();
        }

        private void Save()
        {
            string contentString = JsonConvert.SerializeObject(this);
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            File.WriteAllText(FilePath, contentString);
        }
    }
}