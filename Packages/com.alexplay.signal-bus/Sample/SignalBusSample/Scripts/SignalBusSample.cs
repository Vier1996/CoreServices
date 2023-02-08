using ACS.Core;
using ACS.SignalBus.SignalBus;
using Alexplay.Samples.SignalBus.Scripts.Sample;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Alexplay.Samples.SignalBus.Scripts
{
    public class SignalBusSample : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI sampleText_1;
        [SerializeField] private Button sampleButton_1;
        
        private ISignalBusService _signalBusService; 
        private int count = 0;
        
        private void Start()
        {
            _signalBusService = Core.Instance.SignalBusService;
            
            sampleText_1.text = "waiting signal...";
            sampleButton_1.onClick.AddListener(FireSignal);
            
            SubscribeToSignal();
        }

        private void FireSignal() => _signalBusService.Fire(new SampleSignal());

        private void SubscribeToSignal() => _signalBusService.Subscribe<SampleSignal>(SignalSubscriber);

        private void SignalSubscriber(SampleSignal signal) => sampleText_1.text = $"signal catched {++count} times...";

        private void OnDisable() => _signalBusService.Unsubscribe<SampleSignal>(SignalSubscriber);
    }
}
