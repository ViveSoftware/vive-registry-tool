using HTC.PackagesBootstrapper.Editor.Configs;
using HTC.PackagesBootstrapper.Editor.UI;
using UnityEditor;

namespace HTC.PackagesBootstrapper.Editor.System
{
    [InitializeOnLoad]
    public class RegistryCheck
    {
        static RegistryCheck()
        {
            if (UserSettings.Instance().AutoCheckEnabled)
            {
                if (!ManifestUtils.CheckRegistryExists())
                {
                    RegistryUpdaterWindow.Open();
                }
            }
        }
    }
}