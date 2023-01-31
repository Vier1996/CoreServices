using System;
using ACS.Analytics.Analytics.AmplitudeService;
using Packages.com.alexplay.core.Runtime.Config;
using Sirenix.OdinInspector;

namespace ACS.Analytics.Analytics.Config
{
    [Serializable]
    public class AnalyticsServicesConfig : ServiceConfigBase
    {
        [ShowIf("@IsEnabled == true")]
        public bool TrackInAmplitudeService = false;

        [ShowIf("@TrackInAmplitudeService == true")] 
        public AmplitudeAnalyticsConfig AmplitudeAnalyticsConfig;
        
        [ShowIf("@IsEnabled == true")]
        public bool TrackInFirebaseService = false;
    }
}