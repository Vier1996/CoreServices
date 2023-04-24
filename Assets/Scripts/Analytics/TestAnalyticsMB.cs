using ACS.Analytics;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Analytics
{
    public class TestAnalyticsMB : MonoBehaviour
    {
        private IAnalyticsService _analytics;

        [Inject]
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