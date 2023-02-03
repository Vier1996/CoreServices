using System.Collections.Generic;
using ACS.Analytics;
using ACS.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alexplay.Samples.Analytics.Scripts
{
    public class AnalyticsSample : MonoBehaviour
    {
        [SerializeField] private string _sampleEvent;
        
        [SerializeField] private Button _trackSampleEvent;
        [SerializeField] private Button _trackSampleParamsEvent;
        [SerializeField] private TextMeshProUGUI _info;
        
        private AnalyticsService _analyticsService;
        private Dictionary<string, object> _sampleParams = new Dictionary<string, object>()
        {
            {"sample_param_1", 1111},
            {"sample_param_2", "sample"},
            {"sample_param_3", 8.91f}
        };

        private void Start()
        {
            _analyticsService = Core.Instance.AnalyticsService;
            
            _trackSampleEvent.onClick.AddListener(() =>
            {
                _analyticsService.TrackEvent(_sampleEvent);
                _info.text = $"Tracked event:[{_sampleEvent}]";
            });
            
            _trackSampleParamsEvent.onClick.AddListener(() =>
            {
                _analyticsService.TrackEvent(_sampleEvent, _sampleParams);
                _info.text = $"Tracked event:[{_sampleEvent}] with params";
            });
        }
    }
}
