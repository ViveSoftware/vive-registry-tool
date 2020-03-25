using HTC.PackagesBootstrapper.Editor.System;
using System;
using System.Management.Instrumentation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using HTC.PackagesBootstrapper.Editor.Configs;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HTC.PackagesBootstrapper.Editor.UI
{
    public class RegistryUpdaterWindow : EditorWindow
    {
        private static readonly Vector2 WindowSize = new Vector2(400.0f, 200.0f);
        private static readonly string StatusSuccessClass = "success";
        private static readonly string StatusErrorClass = "error";
        private static readonly string RegistryStatusSuccessString = "Added";
        private static readonly string RegistryStatusErrorString = "Not Added";
        private static readonly string ConnectionStatusSuccessString = "OK";
        private static readonly string ConnectionStatusErrorString = "Error";

        private static MethodInfo ShowPackageManagerMethodInfo;
        
        private Toggle AutoCheckToggle;
        private Label RegistryStatusLabel;
        private Label ConnectionStatusLabel;

        [MenuItem("Tools/HTC/HTC Package Bootstrapper")]
        public static void Open()
        {
            RegistryUpdaterWindow window = GetWindow<RegistryUpdaterWindow>(true, "HTC Packages Bootstrapper");
            window.minSize = WindowSize;
            window.maxSize = WindowSize;
            window.Show();
            window.UpdateAllStatus();

            InitOpenPackageManagerMethod();
        }

        public static void ShowPackageManager()
        {
            if (ShowPackageManagerMethodInfo == null)
            {
                Debug.LogWarning("\"ShowPackageManagerMethodInfo\" hasn't been initialized properly.");
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

            RegistryStatusLabel = rootVisualElement.Query<Label>("RegistryStatusLabel").First();
            ConnectionStatusLabel = rootVisualElement.Query<Label>("ConnectionStatusLabel").First();

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
            ShowPackageManager();
            UpdateAllStatus();
        }

        private void OnRemoveButtonClicked()
        {
            ManifestUtils.RemoveRegistryFromManifest();
            UserSettings.Instance().SetAutoCheckEnabled(false);
            AutoCheckToggle.value = false;
            UpdateAllStatus();
        }

        private void OnCloseButtonClicked()
        {
            Close();
        }

        private void UpdateAllStatus()
        {
            UpdateRegistryStatus();
            UpdateConnectionStatus();
        }

        private void UpdateRegistryStatus()
        {
            RegistryStatusLabel.RemoveFromClassList(StatusSuccessClass);
            RegistryStatusLabel.RemoveFromClassList(StatusErrorClass);

            if (ManifestUtils.CheckRegistryExists())
            {
                RegistryStatusLabel.AddToClassList(StatusSuccessClass);
                RegistryStatusLabel.text = RegistryStatusSuccessString;
            }
            else
            {
                RegistryStatusLabel.AddToClassList(StatusErrorClass);
                RegistryStatusLabel.text = RegistryStatusErrorString;
            }
        }

        private void UpdateConnectionStatus()
        {
            ConnectionStatusLabel.RemoveFromClassList(StatusSuccessClass);
            ConnectionStatusLabel.RemoveFromClassList(StatusErrorClass);

            if (CheckRegistryConnection())
            {
                ConnectionStatusLabel.AddToClassList(StatusSuccessClass);
                ConnectionStatusLabel.text = ConnectionStatusSuccessString;
            }
            else
            {
                ConnectionStatusLabel.AddToClassList(StatusErrorClass);
                ConnectionStatusLabel.text = ConnectionStatusErrorString;
            }
        }

        private bool CheckRegistryConnection()
        {
            string host = Settings.Instance().RegistryHost;
            int port = Settings.Instance().RegistryPort;
            
            try
            {
                using (TcpClient client = new TcpClient())
                {
                    client.Connect(host, port);
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}