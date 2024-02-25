using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ACS.Cheat.CheatTracker.Utilities
{
    public class EnsureServiceInspectorDefine
    {
        private static readonly string[] DEFINES = new string[] { "COM_ALEXPLAY_NET_CHEAT_TRACKER" };

#if UNITY_EDITOR
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
#endif
    }
}