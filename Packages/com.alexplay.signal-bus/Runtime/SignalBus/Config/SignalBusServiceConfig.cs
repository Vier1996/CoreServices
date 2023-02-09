using System;
using Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.SignalBus.SignalBus.Config
{
    [Serializable]
    public class SignalBusServiceConfig : ServiceConfigBase
    {
        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.signal-bus";
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}
