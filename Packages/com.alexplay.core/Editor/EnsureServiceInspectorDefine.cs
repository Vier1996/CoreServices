using System;
using System.Collections.Generic;
using System.Linq;
using ACS.Core;
using ACS.Core.Internal.AlexplayCoreBootstrap;
using Constants;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ACS.CoreEditor.Editor
{
    internal static class EnsureServiceInspectorDefine
    {
        private static readonly string[] DEFINES = new string[] { "COM_ALEXPLAY_NET_CORE" };

        public static AlexplayCoreKitConfig GetConfig()
        {
#if UNITY_EDITOR
            return AssetDatabase.LoadAssetAtPath<AlexplayCoreKitConfig>(ACSConst.SourcePath);
#endif
        }

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
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, definesString);
        }

        [InitializeOnLoadMethod]
        private static void ValidateConfig()
        {
#if UNITY_EDITOR
            AlexplayCoreKitConfig config = GetConfig();

            if (config == null)
            {
                config = ScriptableObject.CreateInstance<AlexplayCoreKitConfig>();

                if (!System.IO.Directory.Exists(ACSConst.AbsoluteResourcesFolder))
                    System.IO.Directory.CreateDirectory(ACSConst.AbsoluteResourcesFolder);
                
                AssetDatabase.CreateAsset(config, ACSConst.SourcePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
#endif
        }
        
        [InitializeOnLoadMethod]
        private static void CheckServicesOnEnsuring()
        {
            List<string> servicesDefines = new List<string>();

#if !COM_ALEXPLAY_NET_DATA
            servicesDefines.Add("COM_ALEXPLAY_NET_DATA");
#endif
#if !COM_ALEXPLAY_NET_SIGNAL_BUS
            servicesDefines.Add("COM_ALEXPLAY_NET_SIGNAL_BUS");
#endif
#if !COM_ALEXPLAY_NET_DIALOG
            servicesDefines.Add("COM_ALEXPLAY_NET_DIALOG");
#endif
#if !COM_ALEXPLAY_NET_OBJECT_POOL
            servicesDefines.Add("COM_ALEXPLAY_NET_OBJECT_POOL");
#endif
#if !COM_ALEXPLAY_NET_ADS
            servicesDefines.Add("COM_ALEXPLAY_NET_ADS");
#endif
#if !COM_ALEXPLAY_NET_GDPR
            servicesDefines.Add("COM_ALEXPLAY_NET_GDPR");
#endif
#if !COM_ALEXPLAY_NET_AUDIO
            servicesDefines.Add("COM_ALEXPLAY_NET_AUDIO");
#endif
#if !COM_ALEXPLAY_NET_PURCHASE
            servicesDefines.Add("COM_ALEXPLAY_NET_PURCHASE");
#endif
#if !COM_ALEXPLAY_NET_ANALYTICS
            servicesDefines.Add("COM_ALEXPLAY_NET_ANALYTICS");
#endif
#if !COM_ALEXPLAY_NET_FBRC
            servicesDefines.Add("COM_ALEXPLAY_NET_FBRC");
#endif

            var currentTarget = EditorUserBuildSettings.selectedBuildTargetGroup;

            if (currentTarget == BuildTargetGroup.Unknown)
            {
                return;
            }
            
            string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(currentTarget).Trim();
            string clearDefines = "";
            
            List<string> defines = definesString.Split(';').ToList();

            bool changed = false;

            for (int i = 0; i < servicesDefines.Count; i++)
            {
                if (defines.Contains(servicesDefines[i]))
                {
                    defines.Remove(servicesDefines[i]);
                    changed = true;
                }
            }

            if (changed)
            {   
                for (int i = 0; i < defines.Count; i++) 
                    clearDefines += defines[i] + ";";
            
                PlayerSettings.SetScriptingDefineSymbolsForGroup(currentTarget, clearDefines);
            }
        }
    }
}
