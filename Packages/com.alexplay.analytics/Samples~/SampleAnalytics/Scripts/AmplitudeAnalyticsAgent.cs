using System.Collections.Generic;
using ACS.Analytics;
using UnityEngine;

namespace Alexplay.Samples.Analytics.Scripts
{
    [AnalyticsAgentAttribute]
    public class AmplitudeAnalyticsAgent : IAnalyticsAgent
    {
        public bool CanTrack { get; set; }
        
        private const string _analytcsTrackedEventfKey = "AmplitudeAnalytics:TrackedEvent - ";
        
        private Amplitude _amplitude;

        public AmplitudeAnalyticsAgent() => InitializeAnalytics();

        private void InitializeAnalytics()
        {
            _amplitude = Amplitude.getInstance();
            _amplitude.setServerUrl("");
            _amplitude.logging = true;
            _amplitude.trackSessionEvents(true);
            _amplitude.init("");
        }


        public void SendEvent(string eventName) => _amplitude.logEvent(eventName);

        public void SendEvent(string eventName, Dictionary<string, object> eventParams)
        {
            if (eventParams == null)
            {
                SendEvent(eventName);
                return;
            }
            
            _amplitude.logEvent(eventName, eventParams);
        }

        public void SendEventOnce(string eventName)
        {
            SendEvent(eventName);
            MarkAsSent(eventName);
        }

        public void SendEventOnce(string eventName, Dictionary<string, object> eventParams)
        {
            SendEvent(eventName, eventParams);
            MarkAsSent(eventName);
        }

        private void MarkAsSent(string prefsName)
        {
            PlayerPrefs.SetInt(prefsName, 1);
            PlayerPrefs.Save();
        }
    }
}