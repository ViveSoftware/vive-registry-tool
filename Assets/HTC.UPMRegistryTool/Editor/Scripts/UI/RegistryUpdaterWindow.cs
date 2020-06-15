﻿using HTC.UPMRegistryTool.Editor.Configs;
using HTC.UPMRegistryTool.Editor.System;
using System;
using System.Net.Sockets;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace HTC.UPMRegistryTool.Editor.UI
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

        private static MethodInfo ShowPackageManagerMethodInfo = null;

        [SerializeField] private StyleSheet UIStyle = null;
        [SerializeField] private VisualTreeAsset UITemplate = null;
        
        private Toggle AutoCheckToggle;
        private Label RegistryStatusLabel;
        private Label ConnectionStatusLabel;

        [MenuItem("Window/HTC/HTC Package Registry")]
        public static void Open()
        {
            RegistryUpdaterWindow window = GetWindow<RegistryUpdaterWindow>(true, "HTC UPM Registry Tool");
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
                Debug.LogWarning("ShowPackageManager() method hadn't been initialized properly. Please open package manager manually.");
                return;
            }

            try
            {
                ShowPackageManagerMethodInfo.Invoke(null, new object[]
                {
#if UNITY_2019_3_OR_NEWER
                    "",
#else
                new MenuCommand(null),
#endif
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private static void InitOpenPackageManagerMethod()
        {
            try
            {
                Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly assembly in assemblies)
                {
                    Type type = assembly.GetType("UnityEditor.PackageManager.UI.PackageManagerWindow");
                    if (type == null)
                    {
                        continue;
                    }

                    MethodInfo methodInfo = null;
#if UNITY_2019_3_OR_NEWER
                    methodInfo = type.GetMethod("OpenPackageManager", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
#else
                    methodInfo = type.GetMethod("ShowPackageManagerWindow", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
#endif
                    if (methodInfo != null)
                    {
                        ShowPackageManagerMethodInfo = methodInfo;
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            if (ShowPackageManagerMethodInfo == null)
            {
                Debug.LogWarning("ShowPackageManager() required method not found.");
            }
        }

        private void OnEnable()
        {
            rootVisualElement.styleSheets.Add(UIStyle);
            UITemplate.CloneTree(rootVisualElement);

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
            catch
            {
                return false;
            }
        }
    }
}