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

        [ShowIf("@IsEnabled == true")] public bool EnableAutoSave = false;
        [ShowIf("@IsEnabled == true && EnableAutoSave")] public int AutoSaveDelay = 60;
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}
