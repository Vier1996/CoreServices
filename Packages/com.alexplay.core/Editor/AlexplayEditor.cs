using System;
using System.Collections.Generic;
using ACS.Core;
using ACS.Core.Internal.AlexplayCoreBootstrap;
using Constants;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace ACS.CoreEditor.Editor
{
    public class AlexplayEditor : OdinMenuEditorWindow
    {
        private static AlexplayCoreKitConfig _coreConfig;
        private OdinMenuTree _menuTree;

        private readonly Dictionary<string, string> _menuItemNames = new Dictionary<string, string>()
        {
            { "Core", ACSConst.CorePackageAssetPath },
#if COM_ALEXPLAY_NET_ADS
            { "Ads Service", ACSConst.AdsPackageAssetPath },
#endif
#if COM_ALEXPLAY_NET_AUDIO
            { "Audio Service", ACSConst.AudioPackageAssetPath },
#endif
#if COM_ALEXPLAY_NET_DATA
            { "Data Service", ACSConst.DataPackageAssetPath },
#endif
#if COM_ALEXPLAY_NET_DIALOG
            { "Dialog Service", ACSConst.DialogPackageAssetPath },
#endif
#if COM_ALEXPLAY_NET_GDPR
            { "Gdpr Service", ACSConst.GDPRPackageAssetPath },
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL
            { "Object Pool Service", ACSConst.ObjectPoolPackageAssetPath },
#endif
#if COM_ALEXPLAY_NET_SIGNAL_BUS
            { "Signal Bus Service", ACSConst.SignalBusPackageAssetPath },
#endif
#if COM_ALEXPLAY_NET_PURCHASE
            { "Purchase Service", ACSConst.PurchasePackageAssetPath },
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
            { "Analytics Service", ACSConst.AnalyticsPackageAssetPath },
#endif
#if COM_ALEXPLAY_NET_FBRC
            {"FBRC", ACSConst.FBRCPackageAssetPath}
#endif
        };
        
        #region Constants

        private const string DocumentationURL =
            "https://docs.google.com/document/d/1PORrcDtGcskwvLADl1r7Vcs8eJlcicsKJyL8jW8-c-k/edit?usp=sharing";

        private const string kMenuItemPath = "Tools/Alexplay/";

        #endregion

        [MenuItem(kMenuItemPath + "/Configuration")]
        private static void OpenWindow()
        {
            AlexplayEditor window = GetWindow<AlexplayEditor>();
            window.titleContent = new GUIContent("Alexplay Core Configuration");
            window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        }
        
        [MenuItem("GameObject/Alexplay/Core", false, 10)]
        public static void CreateCore(MenuCommand menuCommand)
        {
            if(FindObjectOfType<SceneContext>() == null)
                CreateSceneContext();
            
            if(FindObjectOfType<CoreBootstrap>() != null)
                return;
            
            CoreBootstrap coreBootstrap = Resources.Load<CoreBootstrap>("CoreBootstrapper");
            Object corePrefab = PrefabUtility.InstantiatePrefab(coreBootstrap);
            
            corePrefab.name = "Alexplay - [CORE]";
            corePrefab.hideFlags = HideFlags.NotEditable;

            coreBootstrap = ((CoreBootstrap) corePrefab);
            coreBootstrap.transform.SetAsLastSibling();

            for (int i = 0; i < coreBootstrap.transform.childCount; i++)
                coreBootstrap.transform.GetChild(i).hideFlags = HideFlags.NotEditable;

            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
        
        private static void CreateSceneContext()
        {
            var root = new GameObject("SceneContext").AddComponent<SceneContext>();
            Selection.activeGameObject = root.gameObject;

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            if ((_coreConfig = EnsureServiceInspectorDefine.GetConfig()) == null)
                throw new ArgumentException("Seems like you haven't any configuration file, " +
                                            "please restart or recompile UnityEditor");
            
            OdinMenuTree menuTree = new OdinMenuTree();
            
            menuTree.Add("Core", new AlexplayCoreHomePage(), EditorIcons.House);

#if COM_ALEXPLAY_NET_ADS
            menuTree.Add("Ads Service", _coreConfig._advertisementSettings, EditorIcons.Money);
#endif
#if COM_ALEXPLAY_NET_AUDIO
            menuTree.Add("Audio Service", _coreConfig._audioSettings, EditorIcons.Sound);
#endif
#if COM_ALEXPLAY_NET_DATA
            menuTree.Add("Data Service", _coreConfig._dataSettings, EditorIcons.GridBlocks);
#endif
#if COM_ALEXPLAY_NET_DIALOG
            menuTree.Add("Dialog Service", _coreConfig._dialogsSettings, EditorIcons.Podium);
#endif
#if COM_ALEXPLAY_NET_GDPR
            menuTree.Add("Gdpr Service", _coreConfig._gdprSettings, EditorIcons.GridImageTextList);
#endif
#if COM_ALEXPLAY_NET_OBJECT_POOL
            menuTree.Add("Object Pool Service", _coreConfig._objectPoolSettings, EditorIcons.Eject);
#endif
#if COM_ALEXPLAY_NET_SIGNAL_BUS
            menuTree.Add("Signal Bus Service", _coreConfig._signalBusSettings, EditorIcons.WifiSignal);
#endif
#if COM_ALEXPLAY_NET_PURCHASE
            menuTree.Add("Purchase Service", _coreConfig._purchaseSettings, EditorIcons.GKey);
            _coreConfig._purchaseSettings.Validate();
#endif
#if COM_ALEXPLAY_NET_ANALYTICS
            menuTree.Add("Analytics Service", _coreConfig._analyticsSettings, EditorIcons.Clouds);
#endif
#if COM_ALEXPLAY_NET_FBRC
            menuTree.Add("FBRC", _coreConfig._fbrcSettings, EditorIcons.SettingsCog);
#endif
            
            _menuTree = menuTree;
            
            Events.registeredPackages += EventsOnRegisteredPackages;

            EventsOnRegisteredPackages(null);
            
            return _menuTree;
        }

        private void EventsOnRegisteredPackages(PackageRegistrationEventArgs callback)
        {
            int index = 0;
            
            foreach (var menuItem in _menuItemNames)
            {
                _menuTree.MenuItems[index].Name = SetupTitle(menuItem.Key, menuItem.Value);
                index++;
            }
        }

        private string SetupTitle(string title, string assetPath)
        {
            UnityEditor.PackageManager.PackageInfo info = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(assetPath);
            return title + " [" + info.version + "]";
        }

        #region Menu items

        protected void OnValidate()
        {
            if(_coreConfig != null)
                EditorUtility.SetDirty(_coreConfig);
            
#if COM_ALEXPLAY_NET_PURCHASE
            if(_coreConfig != null && _coreConfig._purchaseSettings != null)
                _coreConfig._purchaseSettings.Validate();
#endif
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Events.registeredPackages -= EventsOnRegisteredPackages;
            
            if(_coreConfig != null)
                EditorUtility.SetDirty(_coreConfig);
            
            AssetDatabase.SaveAssets();
        }

        public static void OpenDocumentation() => Application.OpenURL(DocumentationURL);

        #endregion
    }
}
