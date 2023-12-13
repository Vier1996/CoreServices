using System;
using Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Ads
{
    [Serializable]
    public class AdvertisementServiceConfig : ServiceConfigBase
    {
        [ReadOnly]
        [HideInInspector] 
        public string PackageURL = "https://github.com/Vier1996/CoreServices.git?path=Packages/com.alexplay.advertisement";

        [ShowIf("@IsEnabled == true")]
        public string AndroidIdentifier;
        [ShowIf("@IsEnabled == true")]
        public string IosIdentifier;

        [ShowIf("@IsEnabled == true")]
        public AdvertisementOptions Options = new AdvertisementOptions();

        [ShowIf("@IsEnabled == true")] public bool HandleImpression = false;
        [ShowIf("@IsEnabled == true")] public bool IsDebug = false;
        [ShowIf("@IsEnabled == true")] public float RewardDelay = 1f;
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}