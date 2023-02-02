using System.Collections.Generic;

namespace ACS.Analytics
{
    public interface IAnalyticsAgent
    {
        public void SendEvent(string eventName);
        public void SendEvent(string eventName, Dictionary<string, object> @params);
        public void SendEventOnce(string eventName);
        public void SendEventOnce(string eventName, Dictionary<string, object> @params);
    }
}