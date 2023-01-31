using System.Collections.Generic;
using ACS.Analytics.Analytics.Service;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace ACS.Analytics.Analytics.FirebaseService
{
    public class FirebaseAnalytics : IAnalyticsService
    {
        private const string _analytcsTrackedEventfKey = "AmplitudeAnalytics:TrackedEvent - ";
        
        private FirebaseApp firebase;

        public FirebaseAnalytics() => InitializeAnalytics();

        public void InitializeAnalytics()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }

        public void TrackEvent(string eventType) => global::Firebase.Analytics.FirebaseAnalytics.LogEvent(eventType);

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

            Parameter[] analyticsParams = BuildParams(eventParams);
            global::Firebase.Analytics.FirebaseAnalytics.LogEvent(eventType, analyticsParams);
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