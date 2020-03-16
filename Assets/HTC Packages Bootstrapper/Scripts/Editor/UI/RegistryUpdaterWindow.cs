using System;
using System.Collections.Generic;
using System.Reflection;
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
        private static Vector2 WindowSize = new Vector2(400.0f, 140.0f);
        private static MethodInfo ShowPackageManagerMethodInfo;

        [MenuItem("Tools/HTC/HTC Package Bootstrapper")]
        public static void Open()
        {
            RegistryUpdaterWindow window = GetWindow<RegistryUpdaterWindow>(true, "HTC Packages Bootstrapper");
            window.minSize = WindowSize;
            window.maxSize = WindowSize;
            window.Show();

            InitOpenPackageManagerMethod();
        }

        public static void ShowPackageManager()
        {
            if (ShowPackageManagerMethodInfo == null)
            {
                Debug.LogWarning("Show package manager method hasn't been initialized properly.");
                return;
            }

            ShowPackageManagerMethodInfo.Invoke(null, new object[]
            {
                new MenuCommand(null),
            });
        }

        private static void InitOpenPackageManagerMethod()
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type type in types)
                {
                    if (type.Name == "PackageManagerWindow")
                    {
                        MethodInfo methodInfo = type.GetMethod("ShowPackageManagerWindow", BindingFlags.NonPublic | BindingFlags.Static);
                        if (methodInfo != null)
                        {
                            ShowPackageManagerMethodInfo = methodInfo;
                            return;
                        }
                    }
                }
            }

            Debug.LogWarning("Couldn't find method \"ShowPackageManagerWindow\" in class \"PackageManagerWindow\".");
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

            ShowPackageManager();
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