using System;
using Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.ObjectPool.ObjectPool.Config
{
    [Serializable]
    public class ObjectPoolConfig : ServiceConfigBase
    {
        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.object-pool";

        [ShowIf("@IsEnabled == true")]
        public bool AddressablePool = false;
        [ShowIf("@IsEnabled == true")]
        public bool StandardPool = false;
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}
