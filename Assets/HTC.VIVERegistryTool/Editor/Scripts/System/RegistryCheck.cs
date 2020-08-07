using HTC.VIVERegistryTool.Editor.Configs;
using HTC.VIVERegistryTool.Editor.UI;
using HTC.VIVERegistryTool.Editor.Utils;
using UnityEditor;

namespace HTC.VIVERegistryTool.Editor.System
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