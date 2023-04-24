using Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Cheat.CheatTracker
{
    public class CheatTrackerConfig : ServiceConfigBase
    {
        [ReadOnly]
        [HideInInspector]
        public string PackageURL = "";
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}