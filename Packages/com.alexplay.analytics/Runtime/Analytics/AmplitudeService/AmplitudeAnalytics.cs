using System.Collections.Generic;
using ACS.Analytics.Analytics.Service;
using UnityEngine;

namespace ACS.Analytics.Analytics.AmplitudeService
{
    public class AmplitudeAnalytics : IAnalyticsService
    {
        private const string _analytcsTrackedEventfKey = "AmplitudeAnalytics:TrackedEvent - ";
        
        private readonly AmplitudeAnalyticsConfig _config;
        private Amplitude _amplitude;

        public AmplitudeAnalytics(AmplitudeAnalyticsConfig config)
        {
            _config = config;
            
            InitializeAnalytics();
        }

        public void InitializeAnalytics()
        {
            _amplitude = Amplitude.getInstance();
            _amplitude.setServerUrl(_config.ServerURL);
            _amplitude.logging = true;
            _amplitude.trackSessionEvents(true);
            _amplitude.init(_config.ApiKey);
        }

        public void TrackEvent(string eventType) => _amplitude.logEvent(eventType);
        
        public void TrackOnlyOnceEvent(string eventType)
        {
            (bool, string) isTrackedTuple = IsTrackedEvent(eventType);
            
            if(isTrackedTuple.Item1)
                return;

            TrackEvent(eventType);
            
            PlayerPrefs.SetInt(isTrackedTuple.Item2, 1);
            PlayerPrefs.Save();
        }

        public void TrackEvent(string eventType, Dictionary<string, object> eventParams)
        {
            if (eventParams == null)
            {
                TrackEvent(eventType);
                return;
            }
            
            _amplitude.logEvent(eventType, eventParams);
        }

        public void TrackOnlyOnceEvent(string eventType, Dictionary<string, object> eventParams)
        {
            (bool, string) isTrackedTuple = IsTrackedEvent(eventType);
            
            if(isTrackedTuple.Item1)
                return;
            
            TrackEvent(eventType, eventParams);
            
            PlayerPrefs.SetInt(isTrackedTuple.Item2, 1);
            PlayerPrefs.Save();
        }

        private (bool, string) IsTrackedEvent(string eventType)
        {
            string key = _analytcsTrackedEventfKey + eventType;
            return (PlayerPrefs.HasKey(key), key);
        }
    }
}