using System.Collections.Generic;
using HTC.PackagesBootstrapper.Editor.System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HTC.PackagesBootstrapper.Editor.UI
{
    public class RegistryUpdaterWindow : EditorWindow
    {
        private static Vector2 WindowSize = new Vector2(250.0f, 320.0f);

        [MenuItem("Tools/HTC/HTC Package Bootstrapper")]
        public static void Open()
        {
            RegistryUpdaterWindow window = GetWindow<RegistryUpdaterWindow>(true, "HTC Packages Bootstrapper");
            window.minSize = WindowSize;
            window.maxSize = WindowSize;
            window.Show();
        }

        private void OnEnable()
        {
            StyleSheet style = Resources.Load<StyleSheet>("UI/uss/RegistryUpdater");
            rootVisualElement.styleSheets.Add(style);

            VisualTreeAsset template = Resources.Load<VisualTreeAsset>("UI/uxml/RegistryUpdater");
            template.CloneTree(rootVisualElement);

            Button confirmButton = rootVisualElement.Query<Button>("confirm").First();
            confirmButton.clickable.clicked += OnConfirmButtonClicked;
        }

        private void OnConfirmButtonClicked()
        {
            UpdateRegistryToManifest();
            Close();
        }

        private static void UpdateRegistryToManifest()
        {
            JObject manifestJson = Settings.Instance().LoadProjectManifest();
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
            
            Settings.Instance().WriteProjectManifest(manifestJson.ToString());
        }
    }
}