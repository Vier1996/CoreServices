using System.Collections.Generic;
using ACS.Analytics.Agent;
// ReSharper disable once RedundantUsingDirective
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace ACS.Analytics.Agents
{
    [AnalyticsAgent]
    public class FirebaseAnalyticsAgent : IAnalyticsAgent
    {
        private const string _keyPrefix = "FirebaseAnalytics:TrackedEvent:";
        
        private bool _isInitialized = false;

        public FirebaseAnalyticsAgent() => InitializeAnalytics();

        private void InitializeAnalytics()
        {
#if !UNITY_EDITOR     
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _isInitialized = true;
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
#endif
        }

        public void TrackEvent(string eventName)
        {
            if (_isInitialized == false) return;
            
            FirebaseAnalytics.LogEvent(eventName);
        }
        
        public void TrackEvent(string eventName, Dictionary<string, object> eventParams)
        {
            if (_isInitialized == false) return;
             
            if (eventParams == null)
            {
                TrackEvent(eventName);
                return;
            }

            Parameter[] analyticsParams = BuildParams(eventParams);
            FirebaseAnalytics.LogEvent(eventName, analyticsParams);
        }

        public void TrackEventOnce(string eventName)
        {
            if (_isInitialized == false ||
                NeedToSendEvent(eventName, out string prefsName) == false) return;
            
            TrackEvent(eventName);
            MarkAsSent(prefsName);
        }
        
        public void TrackEventOnce(string eventName, Dictionary<string, object> eventParams)
        {
            if (_isInitialized == false ||
                NeedToSendEvent(eventName, out string prefsName) == false) return;

            TrackEvent(eventName, eventParams);
            MarkAsSent(prefsName);
        }

        private bool NeedToSendEvent(string eventName, out string prefsKey)
        {
            prefsKey = GetPrefsKeyWithName(eventName);
            return PlayerPrefs.HasKey(prefsKey);
        }

        private void MarkAsSent(string prefsName)
        {
            PlayerPrefs.SetInt(prefsName, 1);
            PlayerPrefs.Save();
        }

        private string GetPrefsKeyWithName(string eventName) => 
            $"{_keyPrefix}{eventName}";

        private Parameter[] BuildParams(Dictionary<string, object> eventParams)
        {
            int paramIndex = 0;
            Parameter[] analyticsParams = new Parameter[eventParams.Count];

            foreach (KeyValuePair<string, object> param in eventParams)
            {
                analyticsParams[paramIndex] = new Parameter(param.Key, param.Value.ToString());
                paramIndex++;
            }

            return analyticsParams;
        }
    }
}