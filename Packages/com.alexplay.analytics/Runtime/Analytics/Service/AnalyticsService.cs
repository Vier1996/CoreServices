using System.Collections.Generic;
using ACS.Analytics.Analytics.AmplitudeService;
using ACS.Analytics.Analytics.Config;
using ACS.Analytics.Analytics.FirebaseService;

namespace ACS.Analytics.Analytics.Service
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly AnalyticsServicesConfig _config;
        private readonly List<IAnalyticsService> _analyticsServicesList;

        public AnalyticsService(AnalyticsServicesConfig config)
        {
            _config = config;
            _analyticsServicesList = new List<IAnalyticsService>();
        }

        public void PrepareService()
        {
            if(_config.TrackInAmplitudeService == true)
                _analyticsServicesList.Add(new AmplitudeAnalytics(_config.AmplitudeAnalyticsConfig));
            
            if(_config.TrackInFirebaseService == true)
                _analyticsServicesList.Add(new FirebaseAnalytics());
        }
        
        public void TrackEvent(string eventType)
        {
            for (int i = 0; i < _analyticsServicesList.Count; i++) 
                _analyticsServicesList[i].TrackEvent(eventType);
        }

        public void TrackOnlyOnceEvent(string eventType)
        {
            for (int i = 0; i < _analyticsServicesList.Count; i++) 
                _analyticsServicesList[i].TrackOnlyOnceEvent(eventType);
        }

        public void TrackEvent(string eventType, Dictionary<string, object> eventParams)
        {
            for (int i = 0; i < _analyticsServicesList.Count; i++) 
                _analyticsServicesList[i].TrackEvent(eventType, eventParams);
        }

        public void TrackOnlyOnceEvent(string eventType, Dictionary<string, object> eventParams)
        {
            for (int i = 0; i < _analyticsServicesList.Count; i++) 
                _analyticsServicesList[i].TrackOnlyOnceEvent(eventType, eventParams);
        }
    }
}
