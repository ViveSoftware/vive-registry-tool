using HTC.PackagesBootstrapper.Editor.System;
using System;
using System.Reflection;
using HTC.PackagesBootstrapper.Editor.Configs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HTC.PackagesBootstrapper.Editor.UI
{
    public class RegistryUpdaterWindow : EditorWindow
    {
        private static Vector2 WindowSize = new Vector2(400.0f, 200.0f);
        private static Toggle AutoCheckToggle;
        private static MethodInfo ShowPackageManagerMethodInfo;

        [MenuItem("Tools/HTC/HTC Package Bootstrapper")]
        public static void Open()
        {
            // TODO: Update status

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

            AutoCheckToggle = rootVisualElement.Query<Toggle>("AutoCheck").First();
            AutoCheckToggle.RegisterCallback<MouseUpEvent>(OnAutoCheckToggled);
            AutoCheckToggle.value = UserSettings.Instance().AutoCheckEnabled;

            Button addButton = rootVisualElement.Query<Button>("Add").First();
            addButton.clickable.clicked += OnAddButtonClicked;

            Button removeButton = rootVisualElement.Query<Button>("Remove").First();
            removeButton.clickable.clicked += OnRemoveButtonClicked;

            Button closeButton = rootVisualElement.Query<Button>("Close").First();
            closeButton.clickable.clicked += OnCloseButtonClicked;
        }

        private void OnAutoCheckToggled(MouseUpEvent mouseUpEvent)
        {
            UserSettings.Instance().SetAutoCheckEnabled(AutoCheckToggle.value);
        }

        private void OnAddButtonClicked()
        {
            ManifestUtils.UpdateRegistryToManifest();
            // TODO: Update status
            ShowPackageManager();
        }

        private void OnRemoveButtonClicked()
        {
            // TODO: Update status

            ManifestUtils.RemoveRegistryFromManifest();
            UserSettings.Instance().SetAutoCheckEnabled(false);
            AutoCheckToggle.value = false;
        }

        private void OnCloseButtonClicked()
        {
            Close();
        }
    }
}