using HTC.PackagesBootstrapper.Editor.Configs;
using HTC.PackagesBootstrapper.Editor.UI;
using System;
using System.IO;
using UnityEditor;

namespace HTC.PackagesBootstrapper.Editor.System
{
    [InitializeOnLoad]
    public class RegistryCheck
    {
        private const string LastCheckTimestampPath = "Temp/.HTCRegistryLastCheckTimestamp";

        static RegistryCheck()
        {
            if (UserSettings.Instance().AutoCheckEnabled)
            {
                EditorApplication.update += Update;
            }
        }

        private static void Update()
        {
            double elapsedSeconds = (DateTime.UtcNow - UserSettings.Instance().GetLastCheckTime()).TotalSeconds;
            if (elapsedSeconds < Settings.Instance().CheckIntervalSeconds)
            {
                return;
            }

            if (!ManifestUtils.CheckRegistryExists())
            {
                RegistryUpdaterWindow.Open();
            }

            UserSettings.Instance().SetLastCheckTimestamp(DateTime.UtcNow);
        }
    }
}