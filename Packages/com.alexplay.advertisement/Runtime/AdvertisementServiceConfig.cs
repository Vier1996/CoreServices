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

        [ShowIf("@IsEnabled == true"), Tooltip("In seconds")]
        public float InterstitialsTimeout;
        [ShowIf("@IsEnabled == true"), Tooltip("In seconds")]
        public float FreeInterstitialsAtStart;
        [ShowIf("@IsEnabled == true"), Tooltip("In seconds")] 
        public float RewardedTimeout;
        
        [Button] private void UpdatePackage() => UpdatePackage(PackageURL);
    }
}