using ACS.Analytics;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Analytics
{
    public class TestAnalyticsMB : MonoBehaviour
    {
        private IAnalyticsService _analytics;

        private void Inject(IAnalyticsService analytics) => 
            _analytics = analytics;

        [Button]
        private void TrackEvent(string eventName) => 
            _analytics.Event("name")
                .AddParam("paramName", "paramValue")
                .AddParam("param2", 2)
                .TrackOnce();
    }
}