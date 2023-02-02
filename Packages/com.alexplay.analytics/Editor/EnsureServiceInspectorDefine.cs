#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;

namespace Editor
{
    internal static class EnsureServiceInspectorDefine 
    {
        private static readonly string[] DEFINES = new string[] { "COM_ALEXPLAY_NET_ANALYTICS" };

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
    }
}
#endif
