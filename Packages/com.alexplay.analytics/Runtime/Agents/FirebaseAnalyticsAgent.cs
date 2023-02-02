using System.Collections.Generic;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace ACS.Analytics.Agents
{
    public class FirebaseAnalyticsAgent : IAnalyticsAgent
    {
        private const string _keyPrefix = "FirebaseAnalytics:TrackedEvent:";
        
        private bool _isInitialized;

        public FirebaseAnalyticsAgent() => InitializeAnalytics();

        private void InitializeAnalytics()
        {
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
        }

        public void SendEvent(string eventName)
        {
            if (_isInitialized == false) return;
             
            FirebaseAnalytics.LogEvent(eventName);
        }

        public void SendEvent(string eventName, Dictionary<string, object> eventParams)
        {
            if (_isInitialized == false) return;
             
            if (eventParams == null)
            {
                SendEvent(eventName);
                return;
            }

            Parameter[] analyticsParams = BuildParams(eventParams);
            FirebaseAnalytics.LogEvent(eventName, analyticsParams);
        }

        public void SendEventOnce(string eventName)
        {
            if (_isInitialized == false ||
                NeedToSendEvent(eventName, out string prefsName) == false) return;
            
            SendEvent(eventName);
            MarkAsSent(prefsName);
        }

        public void SendEventOnce(string eventName, Dictionary<string, object> eventParams)
        {
            if (_isInitialized == false ||
                NeedToSendEvent(eventName, out string prefsName) == false) return;

            SendEvent(eventName, eventParams);
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