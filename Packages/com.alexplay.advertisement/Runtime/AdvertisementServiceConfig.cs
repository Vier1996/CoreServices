using System;
using Packages.com.alexplay.core.Runtime.Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ACS.Ads
{
    [Serializable]
    public class AdvertisementServiceConfig : ServiceConfigBase
    {
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
    }
}