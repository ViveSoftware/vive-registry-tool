using HTC.UPMRegistryTool.Editor.Configs;
using HTC.UPMRegistryTool.Editor.UI;
using HTC.UPMRegistryTool.Editor.Utils;
using UnityEditor;

namespace HTC.UPMRegistryTool.Editor.System
{
    [InitializeOnLoad]
    public class RegistryCheck
    {
        static RegistryCheck()
        {
            if (UserSettings.Instance().AutoCheckEnabled)
            {
                EditorApplication.update += UpdateOnce;
            }
        }

        private static void UpdateOnce()
        {
            if (!ManifestUtils.CheckRegistryExists(RegistrySettings.Instance().Registry))
            {
                RegistryUpdaterWindow.Open();
            }

            EditorApplication.update -= UpdateOnce;
        }
    }
}