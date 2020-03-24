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
            LastCheckTimestamp = ReadTimestampFile();
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            double elapsedSeconds = (DateTime.UtcNow - LastCheckTimestamp).TotalSeconds;
            if (elapsedSeconds < Settings.Instance().CheckIntervalSeconds)
            {
                return;
            }

            if (!ManifestUtils.CheckRegistryExists())
            {
                RegistryUpdaterWindow.Open();
            }

            LastCheckTimestamp = DateTime.UtcNow;
            WriteTimestampFile(LastCheckTimestamp);
        }

        private static void WriteTimestampFile(DateTime dateTime)
        {
            File.WriteAllText(LastCheckTimestampPath, dateTime.Ticks.ToString());
        }

        private static DateTime ReadTimestampFile()
        {
            if (!File.Exists(LastCheckTimestampPath))
            {
                return new DateTime();
            }

            string timestampString = File.ReadAllText(LastCheckTimestampPath);
            long seconds = long.Parse(timestampString);

            return new DateTime(seconds, DateTimeKind.Utc);
        }
    }
}