using System.Collections.Generic;

namespace ACS.Analytics.Analytics.Service
{
    public interface IAnalyticsService
    {
        public void TrackEvent(string eventType);
        public void TrackOnlyOnceEvent(string eventType);
        public void TrackEvent(string eventType, Dictionary<string, object> eventParams);
        public void TrackOnlyOnceEvent(string eventType, Dictionary<string, object> eventParams);
    }
}