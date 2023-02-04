using System;
using System.Linq;
using ACS.Core;
using ACS.Core.Internal.AlexplayCoreBootstrap;
using UnityEditor;
using UnityEngine;
using Zenject.ReflectionBaking.Mono.Cecil;
using Object = UnityEngine.Object;

namespace ACS.CoreEditor.Editor
{
    internal static class EnsureServiceInspectorDefine
    {
        private static readonly string[] DEFINES = new string[] {"COM_ALEXPLAY_NET_CORE"};
        
        private static readonly string AssetSignature = "ACS Config.asset";
        private static readonly string ResourcesFolder = "Assets/Resources";
        private static readonly string AbsoluteResourcesFolder = Application.dataPath + ResourcesFolder.Replace("Assets", "");
        private static readonly string SourcePath = ResourcesFolder + "/" + AssetSignature;

        public static AlexplayCoreKitConfig GetConfig() => AssetDatabase.LoadAssetAtPath<AlexplayCoreKitConfig>(SourcePath);

        [InitializeOnLoadMethod]
        private static void CloseCurrentCoreInstances()
        {
            CoreBootstrap[] cores = Object.FindObjectsOfType<CoreBootstrap>();

            for (int i = 0; i < cores.Length; i++)
            {
                cores[i].hideFlags = HideFlags.NotEditable;

                for (int j = 0; j < cores[i].transform.childCount; j++)
                    cores[i].transform.GetChild(j).hideFlags = HideFlags.NotEditable;
            }
        }

        [InitializeOnLoadMethod]
        private static void EnsureScriptingDefineSymbol()
        {
            var currentTarget = EditorUserBuildSettings.selectedBuildTargetGroup;

            if (currentTarget == BuildTargetGroup.Unknown)
            {
                return;
            }

            var definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget).Trim();
            var defines = definesString.Split(';');

            bool changed = false;

            foreach (var define in DEFINES)
            {
                if (defines.Contains(define) == false)
                {
                    if (definesString.EndsWith(";", StringComparison.InvariantCulture) == false)
                    {
                        definesString += ";";
                    }

                    definesString += define;
                    changed = true;
                }
            }

            if (changed)
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, definesString);
            }
        }

        [InitializeOnLoadMethod]
        private static void ValidateConfig()
        {
#if UNITY_EDITOR
            bool containsConfig = GetConfig() != null;

            if (!containsConfig)
            {
                AlexplayCoreKitConfig asset = ScriptableObject.CreateInstance<AlexplayCoreKitConfig>();

                if (!System.IO.Directory.Exists(AbsoluteResourcesFolder))
                    System.IO.Directory.CreateDirectory(AbsoluteResourcesFolder);
                
                AssetDatabase.CreateAsset(asset, SourcePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif
        }
    }
}
