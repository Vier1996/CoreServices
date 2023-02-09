using System;
using Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.GDPR.Config
{
    [Serializable]
    public class GdprConfig : ServiceConfigBase
    {
        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.gdpr";
        
        [ShowIf("@IsEnabled == true")]
        public int _globalVersionIndex = 0;
        [ShowIf("@IsEnabled == true")]
        public int _globalMajorIndex = 0;
        [ShowIf("@IsEnabled == true")]
        public int _globalMinorIndex = 1;
        
        public string GetVersion() => $"GDPR:{_globalVersionIndex}.{_globalMajorIndex}.{_globalMinorIndex}";
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}
