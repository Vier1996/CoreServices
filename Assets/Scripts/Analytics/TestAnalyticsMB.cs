using ACS.Analytics;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Analytics
{
    public class TestAnalyticsMB : MonoBehaviour
    {
        private AnalyticsService _analytics;

        [Inject]
        private void Inject(AnalyticsService analytics)
        {
            _analytics = analytics;
            _analytics.RegisterAgent(new TestFirebaseAgent());
        }

        [Button]
        private void TrackEvent(string eventName) => 
            _analytics.TrackEvent(eventName);
    }
}