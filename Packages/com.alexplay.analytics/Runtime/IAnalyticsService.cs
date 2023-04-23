using System.Collections.Generic;

namespace ACS.Analytics
{
    public interface IAnalyticsService
    {
        void RegisterAgent(IAnalyticsAgent agent);
        void TrackEvent(string eventName);
        void TrackEvent(string eventName, string paramName, string paramValue);
        void TrackEvent(string eventName, Dictionary<string, object> @params);
        void TrackEventOnce(string eventName);
        void TrackEventOnce(string eventName, string paramName, string paramValue);
        void TrackEventOnce(string eventName, Dictionary<string, object> @params);
    }
}