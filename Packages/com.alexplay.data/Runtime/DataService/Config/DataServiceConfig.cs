using System;
using Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Data.DataService.Config
{
    [Serializable]
    public class DataServiceConfig : ServiceConfigBase
    {
        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.data";

#if UNITY_EDITOR
        [ShowIf("@IsEnabled == true")] 
        public bool IgnoreCrypt;
#endif
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}
