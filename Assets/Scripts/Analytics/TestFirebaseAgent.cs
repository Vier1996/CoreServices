using System.Collections.Generic;
using ACS.Analytics;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace Analytics
{
    public class TestFirebaseAgent : IAnalyticsAgent
    {
        private const string _analytcsTrackedEventfKey = "AmplitudeAnalytics:TrackedEvent - ";
         
        private FirebaseApp firebase;
         
        private bool _initialized;

        public TestFirebaseAgent() => InitializeAnalytics();

        private void InitializeAnalytics()
        {
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    _initialized = true;
                }
                else
                {
                    Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                }
            });
        }

        public void TrackEvent(string eventName)
        {
            if(!_initialized)
                return;
             
            FirebaseAnalytics.LogEvent(eventName);
        }

        public void TrackEventOnce(string eventName)
        {
            if(!_initialized)
                return;
             
            (bool, string) isTrackedTuple = IsTrackedEvent(eventName);
             
            if(isTrackedTuple.Item1)
                return;

            TrackEvent(eventName);
             
            PlayerPrefs.SetInt(isTrackedTuple.Item2, 1);
            PlayerPrefs.Save();
        }

        public void TrackEvent(string eventName, Dictionary<string, object> eventParams)
        {
            if(!_initialized)
                return;
             
            if (eventParams == null)
            {
                TrackEvent(eventName);
                return;
            }

            Parameter[] analyticsParams = BuildParams(eventParams);
            FirebaseAnalytics.LogEvent(eventName, analyticsParams);
        }

        public void TrackEventOnce(string eventName, Dictionary<string, object> eventParams)
        {
            if(!_initialized)
                return;
             
            (bool, string) isTrackedTuple = IsTrackedEvent(eventName);
             
            if(isTrackedTuple.Item1)
                return;
             
            TrackEvent(eventName, eventParams);
             
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