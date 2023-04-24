using ACS.Analytics.Agent;
using ACS.Analytics.Bundle;

namespace ACS.Analytics
{
    public interface IAnalyticsService
    {
        public void RegisterAgent(IAnalyticsAgent agent);
        public IEventBundle Event(string eventName);
    }
}